using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EngenhariasSenac.Migrations
{
    /// <inheritdoc />
    public partial class AddLectures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LectureQuizes");

            migrationBuilder.DropColumn(
                name: "Cpf",
                table: "LectureSpeakers");

            migrationBuilder.DropColumn(
                name: "CurriculumPath",
                table: "LectureSpeakers");

            migrationBuilder.RenameColumn(
                name: "LinkLive",
                table: "Lectures",
                newName: "Picture");

            migrationBuilder.InsertData(
                table: "LectureSpeakers",
                columns: new[] { "CodSpeaker", "Fullname", "Linkedin" },
                values: new object[,]
                {
                    { 1, "Eng. Mirna Bonfim", "https://www.linkedin.com" },
                    { 2, "Eng. Flavio Wallis", "https://www.linkedin.com" },
                    { 3, "Eng. Jose Renato Baptista", "https://www.linkedin.com" },
                    { 4, "Eng. Jose Reis", "https://www.linkedin.com" },
                    { 5, "João Macluf", "https://www.linkedin.com" },
                    { 6, "Eng. Eduardo Emiliano", "https://www.linkedin.com" }
                });

            migrationBuilder.InsertData(
                table: "Lectures",
                columns: new[] { "CodLecture", "DatetimeEnd", "DatetimeStart", "Description", "Picture", "Room", "Speaker", "Title" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 6, 3, 20, 10, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 6, 3, 19, 10, 0, 0, DateTimeKind.Unspecified), "Engenharia de Produção e Suas Aplicações", "/public/pictures/lectures/mirna_bonfim.jpg", "H324", 1, "Engenharia de Produção e Suas Aplicações" },
                    { 2, new DateTime(2024, 6, 3, 22, 10, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 6, 3, 21, 10, 0, 0, DateTimeKind.Unspecified), "Tecnologia e Aplicações", "/public/pictures/lectures/flavio_wallis.jpg", "H324", 2, "Tecnologia e Aplicações" },
                    { 3, new DateTime(2024, 6, 3, 20, 10, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 6, 3, 19, 10, 0, 0, DateTimeKind.Unspecified), "CREA JOVEM", "/public/pictures/lectures/jose_baptista.jpg", "H324", 3, "CREA JOVEM" },
                    { 4, new DateTime(2024, 6, 3, 22, 10, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 6, 3, 21, 10, 0, 0, DateTimeKind.Unspecified), "Soluções para Teste e Medição de RF", "/public/pictures/lectures/jose_reis.jpg", "H324", 4, "Soluções p. Teste e Med. de RF" },
                    { 5, new DateTime(2024, 6, 4, 20, 10, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 6, 4, 19, 10, 0, 0, DateTimeKind.Unspecified), "Impressão 3D na manufatura 4.0", "/public/pictures/lectures/joao_macluf.jpg", "H324", 5, "Impressao 3D na manufatura 4.0" },
                    { 6, new DateTime(2024, 6, 4, 22, 10, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 6, 4, 21, 10, 0, 0, DateTimeKind.Unspecified), "Engenharia e energia: O futuro que já começou", "/public/pictures/lectures/eduardo_emiliano.jpg", "H324", 6, "Engenharia e energia: O futuro que já começou" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "LectureSpeakers",
                keyColumn: "CodSpeaker",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "LectureSpeakers",
                keyColumn: "CodSpeaker",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "LectureSpeakers",
                keyColumn: "CodSpeaker",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "LectureSpeakers",
                keyColumn: "CodSpeaker",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "LectureSpeakers",
                keyColumn: "CodSpeaker",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "LectureSpeakers",
                keyColumn: "CodSpeaker",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Lectures",
                keyColumn: "CodLecture",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Lectures",
                keyColumn: "CodLecture",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Lectures",
                keyColumn: "CodLecture",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Lectures",
                keyColumn: "CodLecture",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Lectures",
                keyColumn: "CodLecture",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Lectures",
                keyColumn: "CodLecture",
                keyValue: 6);

            migrationBuilder.RenameColumn(
                name: "Picture",
                table: "Lectures",
                newName: "LinkLive");

            migrationBuilder.AddColumn<string>(
                name: "Cpf",
                table: "LectureSpeakers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CurriculumPath",
                table: "LectureSpeakers",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "LectureQuizes",
                columns: table => new
                {
                    CodQuiz = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LectureNavigationCodLecture = table.Column<int>(type: "int", nullable: false),
                    AnswerOptions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lecture = table.Column<int>(type: "int", nullable: false),
                    Question = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
                name: "IX_LectureQuizes_LectureNavigationCodLecture",
                table: "LectureQuizes",
                column: "LectureNavigationCodLecture");
        }
    }
}
