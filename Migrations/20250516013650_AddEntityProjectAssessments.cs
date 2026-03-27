using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EngenhariasSenac.Migrations
{
    /// <inheritdoc />
    public partial class AddEntityProjectAssessments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProjectAssessments",
                columns: table => new
                {
                    CodAssessment = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentEvaluator = table.Column<int>(type: "int", nullable: false),
                    EvaluatedProject = table.Column<int>(type: "int", nullable: false),
                    Assessment = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Picture = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectAssessments", x => x.CodAssessment);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectAssessments");
        }
    }
}
