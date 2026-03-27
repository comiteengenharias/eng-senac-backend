using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EngenhariasSenac.Migrations
{
    /// <inheritdoc />
    public partial class AddCommitteeEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Committee",
                columns: table => new
                {
                    CodId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdSenac = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Committee", x => x.CodId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Committee_IdSenac",
                table: "Committee",
                column: "IdSenac",
                unique: true);

            // Insert Committee Leaders
            migrationBuilder.InsertData(
                table: "Committee",
                columns: new[] { "IdSenac", "Role" },
                values: new object[,]
                {
                    { 1142328924, "Leader" },
                    { 1142926459, "Leader" },
                    { 1142538660, "Leader" },
                    { 1142503685, "Leader" },
                    { 1142534146, "Leader" }
                });

            // Insert Committee Members
            migrationBuilder.InsertData(
                table: "Committee",
                columns: new[] { "IdSenac", "Role" },
                values: new object[,]
                {
                    { 1142225721, "Member" },
                    { 1142698182, "Member" },
                    { 1142777018, "Member" },
                    { 1143223415, "Member" },
                    { 1143216158, "Member" },
                    { 1143237066, "Member" },
                    { 1142471583, "Member" },
                    { 1143209943, "Member" },
                    { 1142290738, "Member" },
                    { 1143016243, "Member" },
                    { 1142438199, "Member" },
                    { 1142706610, "Member" },
                    { 1142576991, "Member" },
                    { 1143219977, "Member" },
                    { 1142896852, "Member" },
                    { 1143216834, "Member" },
                    { 1143189189, "Member" },
                    { 1142072960, "Member" },
                    { 1141987461, "Member" },
                    { 1143133939, "Member" },
                    { 1142954009, "Member" },
                    { 1197375819, "Member" },
                    { 1142592588, "Member" },
                    { 1142471563, "Member" },
                    { 1143177618, "Member" },
                    { 5337455, "Member" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Committee");
        }
    }
}
