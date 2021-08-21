using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Models.Migrations
{
    public partial class edits : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProtocolDateTime",
                table: "Protocols");

            migrationBuilder.DropColumn(
                name: "BirthDate",
                table: "Peoples");

            migrationBuilder.AddColumn<short>(
                name: "ProtocolYear",
                table: "Protocols",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "BirthYear",
                table: "Peoples",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<string>(
                name: "ResidentPlace",
                table: "Peoples",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProtocolYear",
                table: "Protocols");

            migrationBuilder.DropColumn(
                name: "BirthYear",
                table: "Peoples");

            migrationBuilder.DropColumn(
                name: "ResidentPlace",
                table: "Peoples");

            migrationBuilder.AddColumn<DateTime>(
                name: "ProtocolDateTime",
                table: "Protocols",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "BirthDate",
                table: "Peoples",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
