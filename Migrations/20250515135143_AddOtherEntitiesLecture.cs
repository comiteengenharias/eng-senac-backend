using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EngenhariasSenac.Migrations
{
    /// <inheritdoc />
    public partial class AddOtherEntitiesLecture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LectureCertificate",
                columns: table => new
                {
                    CodCertificate = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Lecture = table.Column<int>(type: "int", nullable: false),
                    Student = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DatetimeGenerate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Path = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LectureNavigationCodLecture = table.Column<int>(type: "int", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "LectureLogs",
                columns: table => new
                {
                    CodLog = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Student = table.Column<int>(type: "int", nullable: false),
                    Datetime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Room = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LogType = table.Column<int>(type: "int", nullable: false),
                    StudentNavigationCodStudents = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LectureLogs", x => x.CodLog);
                    table.ForeignKey(
                        name: "FK_LectureLogs_Students_StudentNavigationCodStudents",
                        column: x => x.StudentNavigationCodStudents,
                        principalTable: "Students",
                        principalColumn: "CodStudents",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LectureQuizes",
                columns: table => new
                {
                    CodQuiz = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Lecture = table.Column<int>(type: "int", nullable: false),
                    Question = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnswerOptions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LectureNavigationCodLecture = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LectureQuizes", x => x.CodQuiz);
                    table.ForeignKey(
                        name: "FK_LectureQuizes_Lectures_LectureNavigationCodLecture",
                        column: x => x.LectureNavigationCodLecture,
                        principalTable: "Lectures",
                        principalColumn: "CodLecture",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LectureCertificate_LectureNavigationCodLecture",
                table: "LectureCertificate",
                column: "LectureNavigationCodLecture");

            migrationBuilder.CreateIndex(
                name: "IX_LectureLogs_StudentNavigationCodStudents",
                table: "LectureLogs",
                column: "StudentNavigationCodStudents");

            migrationBuilder.CreateIndex(
                name: "IX_LectureQuizes_LectureNavigationCodLecture",
                table: "LectureQuizes",
                column: "LectureNavigationCodLecture");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LectureCertificate");

            migrationBuilder.DropTable(
                name: "LectureLogs");

            migrationBuilder.DropTable(
                name: "LectureQuizes");
        }
    }
}
