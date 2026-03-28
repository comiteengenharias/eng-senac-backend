using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EngenhariasSenac.Migrations
{
    /// <inheritdoc />
    public partial class AddNewFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeachersPanel");

            migrationBuilder.DropColumn(
                name: "PresentationDay",
                table: "ProjectTeams");

            migrationBuilder.DropColumn(
                name: "PresentationsRoom",
                table: "ProjectTeams");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PresentationDay",
                table: "ProjectTeams",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PresentationsRoom",
                table: "ProjectTeams",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "TeachersPanel",
                columns: table => new
                {
                    CodPanel = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodTeacher = table.Column<int>(type: "int", nullable: false),
                    PresentationDay = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PresentationsRoom = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeachersPanel", x => x.CodPanel);
                    table.ForeignKey(
                        name: "FK_TeachersPanel_Teachers_CodTeacher",
                        column: x => x.CodTeacher,
                        principalTable: "Teachers",
                        principalColumn: "CodTeacher",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeachersPanel_CodTeacher",
                table: "TeachersPanel",
                column: "CodTeacher");
        }
    }
}
