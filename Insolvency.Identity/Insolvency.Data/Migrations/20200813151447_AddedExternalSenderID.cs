using Microsoft.EntityFrameworkCore.Migrations;

namespace Insolvency.Data.Migrations
{
    public partial class AddedExternalSenderID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalSenderSystemId",
                table: "Messages",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalSenderSystemId",
                table: "Messages");
        }
    }
}
