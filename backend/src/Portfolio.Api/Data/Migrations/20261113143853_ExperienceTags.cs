using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portfolio.Api.Data.Migrations
{
    /// <summary>
    /// Experience tags move from a JSON column (Experiences.Tags) to the Tags table shared with the projects.
    /// Edited by hand: the generated version dropped the column first, losing the existing tags.
    /// </summary>
    public partial class ExperienceTags : Migration
    {
        /// <summary>
        /// Same normalization as TagService.Normalize, in SQL: "#Spring Boot " → "spring-boot".
        /// {0} is the raw tag.
        /// </summary>
        private const string NormalizedTag = @"lower(regexp_replace(btrim(ltrim(btrim({0}), '#')), '\s+', '-', 'g'))";

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExperienceTags",
                columns: table => new
                {
                    ExperienceId = table.Column<int>(type: "integer", nullable: false),
                    TagId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExperienceTags", x => new { x.ExperienceId, x.TagId });
                    table.ForeignKey(
                        name: "FK_ExperienceTags_Experiences_ExperienceId",
                        column: x => x.ExperienceId,
                        principalTable: "Experiences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExperienceTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExperienceTags_TagId",
                table: "ExperienceTags",
                column: "TagId");

            // Copy the existing tags before dropping the JSON column.
            // 1. One normalized name per (entry, tag), from the JSON array (jsonb_array_elements_text: one row per item).
            var existing = $"""
                SELECT DISTINCT x."Id" AS "ExperienceId", {string.Format(NormalizedTag, "item")} AS "Name"
                FROM "Experiences" x
                CROSS JOIN LATERAL jsonb_array_elements_text(x."Tags"::jsonb) AS item
                """;

            // 2. Create the tags that don't exist yet (a project may already use some of them).
            migrationBuilder.Sql($"""
                INSERT INTO "Tags" ("Name")
                SELECT DISTINCT e."Name" FROM ({existing}) e
                WHERE e."Name" <> ''
                ON CONFLICT ("Name") DO NOTHING;
                """);

            // 3. Link each entry to its tags.
            migrationBuilder.Sql($"""
                INSERT INTO "ExperienceTags" ("ExperienceId", "TagId")
                SELECT e."ExperienceId", t."Id"
                FROM ({existing}) e
                JOIN "Tags" t ON t."Name" = e."Name";
                """);

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Experiences");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Back to the JSON column: "[]" by default ("" is not valid JSON), then filled from the links.
            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "Experiences",
                type: "text",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.Sql("""
                UPDATE "Experiences" x
                SET "Tags" = (
                    SELECT json_agg(t."Name" ORDER BY t."Name")::text
                    FROM "ExperienceTags" xt
                    JOIN "Tags" t ON t."Id" = xt."TagId"
                    WHERE xt."ExperienceId" = x."Id")
                WHERE EXISTS (SELECT 1 FROM "ExperienceTags" xt WHERE xt."ExperienceId" = x."Id");
                """);

            migrationBuilder.DropTable(
                name: "ExperienceTags");

            // Tags only the experience entries used are left in "Tags" without links;
            // the API deletes unused tags on the next project or entry change.
        }
    }
}
