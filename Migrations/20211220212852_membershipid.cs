using Microsoft.EntityFrameworkCore.Migrations;

namespace assignment2.Migrations
{
    public partial class membershipid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdId",
                table: "Advertisement");

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "CommunityMemberships",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "CommunityId",
                table: "Advertisement",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Id",
                table: "CommunityMemberships");

            migrationBuilder.DropColumn(
                name: "CommunityId",
                table: "Advertisement");

            migrationBuilder.AddColumn<int>(
                name: "AdId",
                table: "Advertisement",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
