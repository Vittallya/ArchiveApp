using Microsoft.EntityFrameworkCore.Migrations;

namespace Models.Migrations
{
    public partial class outerKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Organ",
                table: "Protocols");

            migrationBuilder.DropColumn(
                name: "Social",
                table: "Protocols");

            migrationBuilder.DropColumn(
                name: "Education",
                table: "Peoples");

            migrationBuilder.DropColumn(
                name: "Family",
                table: "Peoples");

            migrationBuilder.DropColumn(
                name: "Party",
                table: "Peoples");

            migrationBuilder.AddColumn<short>(
                name: "OrganId",
                table: "Protocols",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "SocialId",
                table: "Protocols",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "EducationId",
                table: "Peoples",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "FamilyTypeId",
                table: "Peoples",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "PartyId",
                table: "Peoples",
                type: "smallint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Education",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Education", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FamilyType",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FamilyType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Organ",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organ", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Party",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Party", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Social",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Social", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Protocols_OrganId",
                table: "Protocols",
                column: "OrganId");

            migrationBuilder.CreateIndex(
                name: "IX_Protocols_SocialId",
                table: "Protocols",
                column: "SocialId");

            migrationBuilder.CreateIndex(
                name: "IX_Peoples_EducationId",
                table: "Peoples",
                column: "EducationId");

            migrationBuilder.CreateIndex(
                name: "IX_Peoples_FamilyTypeId",
                table: "Peoples",
                column: "FamilyTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Peoples_PartyId",
                table: "Peoples",
                column: "PartyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Peoples_Education_EducationId",
                table: "Peoples",
                column: "EducationId",
                principalTable: "Education",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Peoples_FamilyType_FamilyTypeId",
                table: "Peoples",
                column: "FamilyTypeId",
                principalTable: "FamilyType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Peoples_Party_PartyId",
                table: "Peoples",
                column: "PartyId",
                principalTable: "Party",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Protocols_Organ_OrganId",
                table: "Protocols",
                column: "OrganId",
                principalTable: "Organ",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Protocols_Social_SocialId",
                table: "Protocols",
                column: "SocialId",
                principalTable: "Social",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Peoples_Education_EducationId",
                table: "Peoples");

            migrationBuilder.DropForeignKey(
                name: "FK_Peoples_FamilyType_FamilyTypeId",
                table: "Peoples");

            migrationBuilder.DropForeignKey(
                name: "FK_Peoples_Party_PartyId",
                table: "Peoples");

            migrationBuilder.DropForeignKey(
                name: "FK_Protocols_Organ_OrganId",
                table: "Protocols");

            migrationBuilder.DropForeignKey(
                name: "FK_Protocols_Social_SocialId",
                table: "Protocols");

            migrationBuilder.DropTable(
                name: "Education");

            migrationBuilder.DropTable(
                name: "FamilyType");

            migrationBuilder.DropTable(
                name: "Organ");

            migrationBuilder.DropTable(
                name: "Party");

            migrationBuilder.DropTable(
                name: "Social");

            migrationBuilder.DropIndex(
                name: "IX_Protocols_OrganId",
                table: "Protocols");

            migrationBuilder.DropIndex(
                name: "IX_Protocols_SocialId",
                table: "Protocols");

            migrationBuilder.DropIndex(
                name: "IX_Peoples_EducationId",
                table: "Peoples");

            migrationBuilder.DropIndex(
                name: "IX_Peoples_FamilyTypeId",
                table: "Peoples");

            migrationBuilder.DropIndex(
                name: "IX_Peoples_PartyId",
                table: "Peoples");

            migrationBuilder.DropColumn(
                name: "OrganId",
                table: "Protocols");

            migrationBuilder.DropColumn(
                name: "SocialId",
                table: "Protocols");

            migrationBuilder.DropColumn(
                name: "EducationId",
                table: "Peoples");

            migrationBuilder.DropColumn(
                name: "FamilyTypeId",
                table: "Peoples");

            migrationBuilder.DropColumn(
                name: "PartyId",
                table: "Peoples");

            migrationBuilder.AddColumn<string>(
                name: "Organ",
                table: "Protocols",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Social",
                table: "Protocols",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Education",
                table: "Peoples",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Family",
                table: "Peoples",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Party",
                table: "Peoples",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
