using Microsoft.EntityFrameworkCore.Migrations;

namespace Insolvency.Data.Migrations
{
    public partial class AddedExternalID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalId",
                table: "Messages",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "Messages");
        }
    }
}
