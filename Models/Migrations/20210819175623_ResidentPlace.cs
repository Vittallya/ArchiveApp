using Microsoft.EntityFrameworkCore.Migrations;

namespace Models.Migrations
{
    public partial class ResidentPlace : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResidentPlace",
                table: "Peoples");

            migrationBuilder.AddColumn<string>(
                name: "ResidentPlace",
                table: "Protocols",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResidentPlace",
                table: "Protocols");

            migrationBuilder.AddColumn<string>(
                name: "ResidentPlace",
                table: "Peoples",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);
        }
    }
}
