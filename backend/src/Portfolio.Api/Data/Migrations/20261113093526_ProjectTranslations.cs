using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portfolio.Api.Data.Migrations
{
    /// <summary>
    /// Moves Title/Description from Projects to ProjectTranslations (one row per language).
    /// Edited by hand: existing projects are copied as the "fr" translation before the columns are dropped.
    /// </summary>
    public partial class ProjectTranslations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProjectTranslations",
                columns: table => new
                {
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    Language = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTranslations", x => new { x.ProjectId, x.Language });
                    table.ForeignKey(
                        name: "FK_ProjectTranslations_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.Sql("""
                INSERT INTO "ProjectTranslations" ("ProjectId", "Language", "Title", "Description")
                SELECT "Id", 'fr', "Title", "Description" FROM "Projects";
                """);

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Projects");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Projects",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Projects",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            // Bring back the "fr" text (other languages are lost).
            migrationBuilder.Sql("""
                UPDATE "Projects" AS p
                SET "Title" = t."Title", "Description" = t."Description"
                FROM "ProjectTranslations" AS t
                WHERE t."ProjectId" = p."Id" AND t."Language" = 'fr';
                """);

            migrationBuilder.DropTable(
                name: "ProjectTranslations");
        }
    }
}
