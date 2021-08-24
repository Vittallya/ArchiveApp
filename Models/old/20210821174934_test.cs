using Microsoft.EntityFrameworkCore.Migrations;

namespace Models.Migrations
{
    public partial class test : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nationality",
                table: "Peoples");

            migrationBuilder.AddColumn<short>(
                name: "NatioId",
                table: "Peoples",
                type: "smallint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Natio",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Natio", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Peoples_NatioId",
                table: "Peoples",
                column: "NatioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Peoples_Natio_NatioId",
                table: "Peoples",
                column: "NatioId",
                principalTable: "Natio",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Peoples_Natio_NatioId",
                table: "Peoples");

            migrationBuilder.DropTable(
                name: "Natio");

            migrationBuilder.DropIndex(
                name: "IX_Peoples_NatioId",
                table: "Peoples");

            migrationBuilder.DropColumn(
                name: "NatioId",
                table: "Peoples");

            migrationBuilder.AddColumn<string>(
                name: "Nationality",
                table: "Peoples",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
