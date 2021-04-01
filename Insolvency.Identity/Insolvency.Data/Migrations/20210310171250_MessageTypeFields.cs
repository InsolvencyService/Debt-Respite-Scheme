using Microsoft.EntityFrameworkCore.Migrations;

namespace Insolvency.Data.Migrations
{
    public partial class MessageTypeFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MessageType",
                table: "Messages",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MessageVersion",
                table: "Messages",
                maxLength: 200,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MessageType",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "MessageVersion",
                table: "Messages");
        }
    }
}
