using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeAnalyzerService.Backend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProjectName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RuleName = table.Column<string>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Category = table.Column<string>(type: "TEXT", nullable: false),
                    IsEnabledByDefault = table.Column<bool>(type: "INTEGER", nullable: false),
                    DefaultSeverity = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Analyses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FK_ProjectAnalysis_Analysis = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Analyses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Analyses_Projects_FK_ProjectAnalysis_Analysis",
                        column: x => x.FK_ProjectAnalysis_Analysis,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RuleViolations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    FK_RuleViolation_Rule = table.Column<int>(type: "INTEGER", nullable: false),
                    PluginId = table.Column<string>(type: "TEXT", nullable: false),
                    Message = table.Column<string>(type: "TEXT", nullable: false),
                    TargetLanguage = table.Column<string>(type: "TEXT", nullable: false),
                    Location_Path = table.Column<string>(type: "TEXT", nullable: false),
                    Location_StartLine = table.Column<int>(type: "INTEGER", nullable: false),
                    Location_EndLine = table.Column<int>(type: "INTEGER", nullable: false),
                    Location_FileExtension = table.Column<string>(type: "TEXT", nullable: false),
                    Severity = table.Column<string>(type: "TEXT", nullable: false),
                    FK_Analysis_RuleViolation = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RuleViolations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RuleViolations_Analyses_FK_Analysis_RuleViolation",
                        column: x => x.FK_Analysis_RuleViolation,
                        principalTable: "Analyses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RuleViolations_Rules_FK_RuleViolation_Rule",
                        column: x => x.FK_RuleViolation_Rule,
                        principalTable: "Rules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Analyses_FK_ProjectAnalysis_Analysis",
                table: "Analyses",
                column: "FK_ProjectAnalysis_Analysis");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ProjectName",
                table: "Projects",
                column: "ProjectName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rules_RuleName",
                table: "Rules",
                column: "RuleName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RuleViolations_FK_Analysis_RuleViolation",
                table: "RuleViolations",
                column: "FK_Analysis_RuleViolation");

            migrationBuilder.CreateIndex(
                name: "IX_RuleViolations_FK_RuleViolation_Rule",
                table: "RuleViolations",
                column: "FK_RuleViolation_Rule");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RuleViolations");

            migrationBuilder.DropTable(
                name: "Analyses");

            migrationBuilder.DropTable(
                name: "Rules");

            migrationBuilder.DropTable(
                name: "Projects");
        }
    }
}
