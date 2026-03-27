using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EngenhariasSenac.Migrations
{
    /// <inheritdoc />
    public partial class AddEntitiesLoginLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudentLoginLog",
                columns: table => new
                {
                    CodStudentLoginLog = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodStudent = table.Column<int>(type: "int", nullable: false),
                    DtTmAccess = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DtTmEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentLoginLog", x => x.CodStudentLoginLog);
                });

            migrationBuilder.CreateTable(
                name: "TeacherLoginLog",
                columns: table => new
                {
                    CodTeacherLoginLog = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodTeacher = table.Column<int>(type: "int", nullable: false),
                    DtTmAccess = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DtTmEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherLoginLog", x => x.CodTeacherLoginLog);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentLoginLog");

            migrationBuilder.DropTable(
                name: "TeacherLoginLog");
        }
    }
}
