using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EngenhariasSenac.Migrations
{
    /// <inheritdoc />
    public partial class NewInitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LectureCertificate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LectureCertificate",
                columns: table => new
                {
                    CodCertificate = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LectureNavigationCodLecture = table.Column<int>(type: "int", nullable: false),
                    DatetimeGenerate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Lecture = table.Column<int>(type: "int", nullable: false),
                    Path = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Student = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LectureCertificate", x => x.CodCertificate);
                    table.ForeignKey(
                        name: "FK_LectureCertificate_Lectures_LectureNavigationCodLecture",
                        column: x => x.LectureNavigationCodLecture,
                        principalTable: "Lectures",
                        principalColumn: "CodLecture",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LectureCertificate_LectureNavigationCodLecture",
                table: "LectureCertificate",
                column: "LectureNavigationCodLecture");
        }
    }
}
