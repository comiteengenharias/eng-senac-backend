using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EngenhariasSenac.Migrations
{
    /// <inheritdoc />
    public partial class AddEntitiesLecture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Lectures",
                columns: table => new
                {
                    CodLecture = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Speaker = table.Column<int>(type: "int", nullable: false),
                    DatetimeStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DatetimeEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Room = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LinkLive = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lectures", x => x.CodLecture);
                });

            migrationBuilder.CreateTable(
                name: "LectureSpeakers",
                columns: table => new
                {
                    CodSpeaker = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fullname = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Cpf = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CurriculumPath = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Linkedin = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LectureSpeakers", x => x.CodSpeaker);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Lectures");

            migrationBuilder.DropTable(
                name: "LectureSpeakers");
        }
    }
}
