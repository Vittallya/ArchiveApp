using Microsoft.EntityFrameworkCore.Migrations;

namespace Models.Migrations
{
    public partial class NoOuterTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Peoples_Education_EducationId",
                table: "Peoples");

            migrationBuilder.DropForeignKey(
                name: "FK_Peoples_FamilyType_FamilyId",
                table: "Peoples");

            migrationBuilder.DropForeignKey(
                name: "FK_Peoples_Nationalities_NationalityId",
                table: "Peoples");

            migrationBuilder.DropForeignKey(
                name: "FK_Peoples_Party_PartyId",
                table: "Peoples");

            migrationBuilder.DropForeignKey(
                name: "FK_Protocols_Organs_OrganId",
                table: "Protocols");

            migrationBuilder.DropForeignKey(
                name: "FK_Protocols_Social_SocialId",
                table: "Protocols");

            migrationBuilder.DropTable(
                name: "Education");

            migrationBuilder.DropTable(
                name: "FamilyType");

            migrationBuilder.DropTable(
                name: "Nationalities");

            migrationBuilder.DropTable(
                name: "Organs");

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
                name: "IX_Peoples_FamilyId",
                table: "Peoples");

            migrationBuilder.DropIndex(
                name: "IX_Peoples_NationalityId",
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
                name: "FamilyId",
                table: "Peoples");

            migrationBuilder.DropColumn(
                name: "NationalityId",
                table: "Peoples");

            migrationBuilder.DropColumn(
                name: "PartyId",
                table: "Peoples");

            migrationBuilder.AlterColumn<string>(
                name: "ProtocolNumber",
                table: "Protocols",
                type: "nvarchar(85)",
                maxLength: 85,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

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

            migrationBuilder.AlterColumn<bool>(
                name: "Gender",
                table: "Peoples",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

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
                name: "Nationality",
                table: "Peoples",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Party",
                table: "Peoples",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "Nationality",
                table: "Peoples");

            migrationBuilder.DropColumn(
                name: "Party",
                table: "Peoples");

            migrationBuilder.AlterColumn<string>(
                name: "ProtocolNumber",
                table: "Protocols",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(85)",
                oldMaxLength: 85,
                oldNullable: true);

            migrationBuilder.AddColumn<short>(
                name: "OrganId",
                table: "Protocols",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "SocialId",
                table: "Protocols",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AlterColumn<bool>(
                name: "Gender",
                table: "Peoples",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<short>(
                name: "EducationId",
                table: "Peoples",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "FamilyId",
                table: "Peoples",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "NationalityId",
                table: "Peoples",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "PartyId",
                table: "Peoples",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.CreateTable(
                name: "Education",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Kind = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FamilyType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Nationalities",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nationalities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Organs",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Party",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                    Kind = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                name: "IX_Peoples_FamilyId",
                table: "Peoples",
                column: "FamilyId");

            migrationBuilder.CreateIndex(
                name: "IX_Peoples_NationalityId",
                table: "Peoples",
                column: "NationalityId");

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
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Peoples_FamilyType_FamilyId",
                table: "Peoples",
                column: "FamilyId",
                principalTable: "FamilyType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Peoples_Nationalities_NationalityId",
                table: "Peoples",
                column: "NationalityId",
                principalTable: "Nationalities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Peoples_Party_PartyId",
                table: "Peoples",
                column: "PartyId",
                principalTable: "Party",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Protocols_Organs_OrganId",
                table: "Protocols",
                column: "OrganId",
                principalTable: "Organs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Protocols_Social_SocialId",
                table: "Protocols",
                column: "SocialId",
                principalTable: "Social",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
