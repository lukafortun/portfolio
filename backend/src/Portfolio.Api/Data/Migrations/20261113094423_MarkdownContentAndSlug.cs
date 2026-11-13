using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portfolio.Api.Data.Migrations
{
    /// <summary>
    /// Description → Summary (card text) + Content (Markdown page), and a unique Slug per project.
    /// Edited by hand so existing data is kept: see the SQL steps.
    /// </summary>
    public partial class MarkdownContentAndSlug : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "ProjectTranslations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Summary",
                table: "ProjectTranslations",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            // The old description becomes the page content, and its beginning the summary.
            migrationBuilder.Sql("""
                UPDATE "ProjectTranslations"
                SET "Content" = "Description",
                    "Summary" = LEFT("Description", 500);
                """);

            migrationBuilder.DropColumn(
                name: "Description",
                table: "ProjectTranslations");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Projects",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            // Slug from the French title: "Mon Projet 2" → "mon-projet-2".
            // Empty or duplicate results fall back to "project-<id>", so the unique index can be created.
            migrationBuilder.Sql("""
                UPDATE "Projects" AS p
                SET "Slug" = s.slug
                FROM (
                    SELECT base."Id",
                           CASE WHEN base.slug = '' OR COUNT(*) OVER (PARTITION BY base.slug) > 1
                                THEN 'project-' || base."Id"
                                ELSE base.slug
                           END AS slug
                    FROM (
                        SELECT pr."Id",
                               TRIM(BOTH '-' FROM LEFT(
                                   TRIM(BOTH '-' FROM LOWER(REGEXP_REPLACE(COALESCE(t."Title", ''), '[^a-zA-Z0-9]+', '-', 'g'))),
                                   90)) AS slug
                        FROM "Projects" AS pr
                        LEFT JOIN "ProjectTranslations" AS t
                               ON t."ProjectId" = pr."Id" AND t."Language" = 'fr'
                    ) AS base
                ) AS s
                WHERE p."Id" = s."Id";
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Slug",
                table: "Projects",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Projects_Slug",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Projects");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ProjectTranslations",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("""
                UPDATE "ProjectTranslations"
                SET "Description" = LEFT(CASE WHEN "Content" = '' THEN "Summary" ELSE "Content" END, 4000);
                """);

            migrationBuilder.DropColumn(
                name: "Content",
                table: "ProjectTranslations");

            migrationBuilder.DropColumn(
                name: "Summary",
                table: "ProjectTranslations");
        }
    }
}
