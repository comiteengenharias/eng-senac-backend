using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EngenhariasSenac.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Students_IdSenac",
                table: "Students",
                column: "IdSenac",
                unique: true,
                filter: "[IdSenac] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Students_InstitutionalEmail",
                table: "Students",
                column: "InstitutionalEmail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTeams_Token",
                table: "ProjectTeams",
                column: "Token",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Students_IdSenac",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_InstitutionalEmail",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_ProjectTeams_Token",
                table: "ProjectTeams");
        }
    }
}
