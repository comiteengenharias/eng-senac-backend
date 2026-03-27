using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EngenhariasSenac.Migrations
{
    /// <inheritdoc />
    public partial class EditEntityCompanies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Companies",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Picture",
                table: "Companies",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "Picture",
                table: "Companies");
        }
    }
}
