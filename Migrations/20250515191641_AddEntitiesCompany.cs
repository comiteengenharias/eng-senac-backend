using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EngenhariasSenac.Migrations
{
    /// <inheritdoc />
    public partial class AddEntitiesCompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BusinessAssessment",
                columns: table => new
                {
                    CodAssessment = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentEvaluator = table.Column<int>(type: "int", nullable: false),
                    CompanyEvaluated = table.Column<int>(type: "int", nullable: false),
                    Assessment = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Picture = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessAssessment", x => x.CodAssessment);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    CodCompany = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.CodCompany);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusinessAssessment");

            migrationBuilder.DropTable(
                name: "Companies");
        }
    }
}
