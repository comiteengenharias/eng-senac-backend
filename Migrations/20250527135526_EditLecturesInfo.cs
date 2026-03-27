using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EngenhariasSenac.Migrations
{
    /// <inheritdoc />
    public partial class EditLecturesInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "LectureSpeakers",
                columns: new[] { "CodSpeaker", "Fullname", "Linkedin" },
                values: new object[,]
                {
                    { 7, "Fernando Matijewitsch", "https://www.linkedin.com/in/fernando-matijewitsch/" },
                    { 8, "Luiz Augusto", "https://www.linkedin.com/in/econofisico/" }
                });

            migrationBuilder.UpdateData(
                table: "Lectures",
                keyColumn: "CodLecture",
                keyValue: 2,
                columns: new[] { "DatetimeEnd", "DatetimeStart", "Description", "Picture", "Speaker", "Title" },
                values: new object[] { new DateTime(2024, 6, 3, 20, 10, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 6, 3, 19, 10, 0, 0, DateTimeKind.Unspecified), "CREA JOVEM", "/public/pictures/lectures/jose_baptista.jpg", 3, "CREA JOVEM" });

            migrationBuilder.UpdateData(
                table: "Lectures",
                keyColumn: "CodLecture",
                keyValue: 3,
                columns: new[] { "Description", "Picture", "Room", "Speaker", "Title" },
                values: new object[] { "Go Gamers - ESPM Escola Superior de Propaganda e Marketing", "/public/pictures/lectures/fernando-matijewitsch.jpg", "H433", 7, "Marketing Aplicado às Engenharias" });

            migrationBuilder.UpdateData(
                table: "Lectures",
                keyColumn: "CodLecture",
                keyValue: 4,
                columns: new[] { "Description", "Picture", "Speaker", "Title" },
                values: new object[] { "Tecnologia e Aplicações", "/public/pictures/lectures/flavio_wallis.jpg", 2, "Tecnologia e Aplicações" });

            migrationBuilder.UpdateData(
                table: "Lectures",
                keyColumn: "CodLecture",
                keyValue: 5,
                columns: new[] { "DatetimeEnd", "DatetimeStart", "Description", "Picture", "Speaker", "Title" },
                values: new object[] { new DateTime(2024, 6, 3, 22, 10, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 6, 3, 21, 10, 0, 0, DateTimeKind.Unspecified), "Soluções para Teste e Medição de RF", "/public/pictures/lectures/jose_reis.jpg", 4, "Soluções p. Teste e Med. de RF" });

            migrationBuilder.UpdateData(
                table: "Lectures",
                keyColumn: "CodLecture",
                keyValue: 6,
                columns: new[] { "DatetimeEnd", "DatetimeStart", "Description", "Picture", "Speaker", "Title" },
                values: new object[] { new DateTime(2024, 6, 4, 20, 10, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 6, 4, 19, 10, 0, 0, DateTimeKind.Unspecified), "Impressão 3D na manufatura 4.0", "/public/pictures/lectures/joao_macluf.jpg", 5, "Impressao 3D na manufatura 4.0" });

            migrationBuilder.InsertData(
                table: "Lectures",
                columns: new[] { "CodLecture", "DatetimeEnd", "DatetimeStart", "Description", "Picture", "Room", "Speaker", "Title" },
                values: new object[,]
                {
                    { 7, new DateTime(2024, 6, 4, 22, 10, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 6, 4, 21, 10, 0, 0, DateTimeKind.Unspecified), "Engenharia e energia: O futuro que já começou", "/public/pictures/lectures/eduardo_emiliano.jpg", "H324", 6, "Engenharia e energia: O futuro que já começou" },
                    { 8, new DateTime(2024, 6, 5, 20, 50, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 6, 5, 19, 10, 0, 0, DateTimeKind.Unspecified), "Venturus - O que é computação quântica e pra que serve?", "/public/pictures/lectures/luiz-augusto.jpg", "A definir", 8, "O que é computação quântica e pra que serve?" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "LectureSpeakers",
                keyColumn: "CodSpeaker",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "LectureSpeakers",
                keyColumn: "CodSpeaker",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Lectures",
                keyColumn: "CodLecture",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Lectures",
                keyColumn: "CodLecture",
                keyValue: 8);

            migrationBuilder.UpdateData(
                table: "Lectures",
                keyColumn: "CodLecture",
                keyValue: 2,
                columns: new[] { "DatetimeEnd", "DatetimeStart", "Description", "Picture", "Speaker", "Title" },
                values: new object[] { new DateTime(2024, 6, 3, 22, 10, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 6, 3, 21, 10, 0, 0, DateTimeKind.Unspecified), "Tecnologia e Aplicações", "/public/pictures/lectures/flavio_wallis.jpg", 2, "Tecnologia e Aplicações" });

            migrationBuilder.UpdateData(
                table: "Lectures",
                keyColumn: "CodLecture",
                keyValue: 3,
                columns: new[] { "Description", "Picture", "Room", "Speaker", "Title" },
                values: new object[] { "CREA JOVEM", "/public/pictures/lectures/jose_baptista.jpg", "H324", 3, "CREA JOVEM" });

            migrationBuilder.UpdateData(
                table: "Lectures",
                keyColumn: "CodLecture",
                keyValue: 4,
                columns: new[] { "Description", "Picture", "Speaker", "Title" },
                values: new object[] { "Soluções para Teste e Medição de RF", "/public/pictures/lectures/jose_reis.jpg", 4, "Soluções p. Teste e Med. de RF" });

            migrationBuilder.UpdateData(
                table: "Lectures",
                keyColumn: "CodLecture",
                keyValue: 5,
                columns: new[] { "DatetimeEnd", "DatetimeStart", "Description", "Picture", "Speaker", "Title" },
                values: new object[] { new DateTime(2024, 6, 4, 20, 10, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 6, 4, 19, 10, 0, 0, DateTimeKind.Unspecified), "Impressão 3D na manufatura 4.0", "/public/pictures/lectures/joao_macluf.jpg", 5, "Impressao 3D na manufatura 4.0" });

            migrationBuilder.UpdateData(
                table: "Lectures",
                keyColumn: "CodLecture",
                keyValue: 6,
                columns: new[] { "DatetimeEnd", "DatetimeStart", "Description", "Picture", "Speaker", "Title" },
                values: new object[] { new DateTime(2024, 6, 4, 22, 10, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 6, 4, 21, 10, 0, 0, DateTimeKind.Unspecified), "Engenharia e energia: O futuro que já começou", "/public/pictures/lectures/eduardo_emiliano.jpg", 6, "Engenharia e energia: O futuro que já começou" });
        }
    }
}
