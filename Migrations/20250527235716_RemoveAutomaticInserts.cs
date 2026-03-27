using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EngenhariasSenac.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAutomaticInserts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "CodCompany",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "CodCompany",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "CodCompany",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "CodCompany",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "CodCompany",
                keyValue: 5);

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

            migrationBuilder.DeleteData(
                table: "Lectures",
                keyColumn: "CodLecture",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Lectures",
                keyColumn: "CodLecture",
                keyValue: 8);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "CodCompany", "Description", "Name", "Picture" },
                values: new object[,]
                {
                    { 1, "Empresa especializada em soluções de mobilidade elétrica e urbana.", "Eforce Mobilidade", "/public/pictures/business/eforce_mobilidade.jpg" },
                    { 2, "Indústria focada em usinagem de precisão e corte CNC de alta performance.", "Fiber CNC", "/public/pictures/business/fiber_cnc.jpg" },
                    { 3, "Grupo empresarial com atuação no setor de distribuição e atacado.", "Grupo CBD", "/public/pictures/business/grupo_cbd.jpg" },
                    { 4, "Líder em componentes de movimentação com polímeros de alto desempenho.", "Igus", "/public/pictures/business/igus.jpg" },
                    { 5, "Empresa referência em climatização e engenharia ambiental.", "Munclair", "/public/pictures/business/munclair.jpg" }
                });

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
                    { 6, "Eng. Eduardo Emiliano", "https://www.linkedin.com" },
                    { 7, "Fernando Matijewitsch", "https://www.linkedin.com/in/fernando-matijewitsch/" },
                    { 8, "Luiz Augusto", "https://www.linkedin.com/in/econofisico/" }
                });

            migrationBuilder.InsertData(
                table: "Lectures",
                columns: new[] { "CodLecture", "DatetimeEnd", "DatetimeStart", "Description", "Picture", "Room", "Speaker", "Title" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 6, 2, 20, 10, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 6, 2, 19, 10, 0, 0, DateTimeKind.Unspecified), "Engenharia de Produção e Suas Aplicações", "/public/pictures/lectures/mirna_bonfim.jpg", "H324", 1, "Engenharia de Produção e Suas Aplicações" },
                    { 2, new DateTime(2024, 6, 2, 20, 10, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 6, 2, 19, 10, 0, 0, DateTimeKind.Unspecified), "CREA JOVEM", "/public/pictures/lectures/jose_baptista.jpg", "H324", 3, "CREA JOVEM" },
                    { 3, new DateTime(2024, 6, 3, 22, 10, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 6, 3, 21, 10, 0, 0, DateTimeKind.Unspecified), "Tecnologia e Aplicações", "/public/pictures/lectures/flavio_wallis.jpg", "H324", 2, "Tecnologia e Aplicações" },
                    { 4, new DateTime(2024, 6, 3, 22, 10, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 6, 3, 21, 10, 0, 0, DateTimeKind.Unspecified), "Soluções para Teste e Medição de RF", "/public/pictures/lectures/jose_reis.jpg", "H324", 4, "Soluções p. Teste e Med. de RF" },
                    { 5, new DateTime(2024, 6, 4, 20, 10, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 6, 4, 19, 10, 0, 0, DateTimeKind.Unspecified), "Impressão 3D na manufatura 4.0", "/public/pictures/lectures/joao_macluf.jpg", "H324", 5, "Impressao 3D na manufatura 4.0" },
                    { 6, new DateTime(2024, 6, 4, 22, 10, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 6, 4, 21, 10, 0, 0, DateTimeKind.Unspecified), "Engenharia e energia: O futuro que já começou", "/public/pictures/lectures/eduardo_emiliano.jpg", "H324", 6, "Engenharia e energia: O futuro que já começou" },
                    { 7, new DateTime(2024, 6, 5, 20, 50, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 6, 5, 19, 10, 0, 0, DateTimeKind.Unspecified), "Venturus - O que é computação quântica e pra que serve?", "/public/pictures/lectures/luiz-augusto.jpg", "A definir", 8, "O que é computação quântica e pra que serve?" },
                    { 8, new DateTime(2024, 6, 5, 22, 40, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 6, 5, 20, 50, 0, 0, DateTimeKind.Unspecified), "Go Gamers - ESPM Escola Superior de Propaganda e Marketing", "/public/pictures/lectures/fernando-matijewitsch.jpg", "H433", 7, "Marketing Aplicado às Engenharias" }
                });
        }
    }
}
