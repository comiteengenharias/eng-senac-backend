using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EngenhariasSenac.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnCommentInBusinessAssessment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "BusinessAssessment",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comment",
                table: "BusinessAssessment");
        }
    }
}
