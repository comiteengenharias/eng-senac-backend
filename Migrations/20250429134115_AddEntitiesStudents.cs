using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EngenhariasSenac.Migrations
{
    /// <inheritdoc />
    public partial class AddEntitiesStudents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProjectTeams",
                columns: table => new
                {
                    CodTeam = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Token = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    GroupName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Semester = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Leader = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTeams", x => x.CodTeam);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    CodStudents = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdSenac = table.Column<int>(type: "int", nullable: true),
                    Fullname = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    InstitutionalEmail = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Cellphone = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ProjectTeam = table.Column<int>(type: "int", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Semester = table.Column<int>(type: "int", nullable: true),
                    PointMaterial = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.CodStudents);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectTeams");

            migrationBuilder.DropTable(
                name: "Students");
        }
    }
}
