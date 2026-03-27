using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EngenhariasSenac.Migrations
{
    /// <inheritdoc />
    public partial class EditLecureLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LectureLogs_Students_StudentNavigationCodStudents",
                table: "LectureLogs");

            migrationBuilder.DropIndex(
                name: "IX_LectureLogs_StudentNavigationCodStudents",
                table: "LectureLogs");

            migrationBuilder.DropColumn(
                name: "StudentNavigationCodStudents",
                table: "LectureLogs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StudentNavigationCodStudents",
                table: "LectureLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_LectureLogs_StudentNavigationCodStudents",
                table: "LectureLogs",
                column: "StudentNavigationCodStudents");

            migrationBuilder.AddForeignKey(
                name: "FK_LectureLogs_Students_StudentNavigationCodStudents",
                table: "LectureLogs",
                column: "StudentNavigationCodStudents",
                principalTable: "Students",
                principalColumn: "CodStudents",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
