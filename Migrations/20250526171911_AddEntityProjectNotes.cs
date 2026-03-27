using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EngenhariasSenac.Migrations
{
    /// <inheritdoc />
    public partial class AddEntityProjectNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProjectNotes",
                columns: table => new
                {
                    CodAssessment = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeacherEvaluator = table.Column<int>(type: "int", nullable: false),
                    EvaluatedProject = table.Column<int>(type: "int", nullable: false),
                    Assessment = table.Column<string>(type: "nvarchar(MAX)", nullable: false),
                    Picture = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    EvaluateType = table.Column<string>(type: "varchar(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectNotes", x => x.CodAssessment);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectNotes");
        }
    }
}
