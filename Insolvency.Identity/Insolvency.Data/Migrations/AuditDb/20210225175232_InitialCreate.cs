using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Insolvency.Data.Migrations.AuditDb
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditMessage",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    ActionName = table.Column<string>(nullable: true),
                    ControllerName = table.Column<string>(nullable: true),
                    ClientId = table.Column<string>(nullable: true),
                    OrganisationId = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    SenderName = table.Column<string>(nullable: true),
                    PayLoad = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditMessage", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditMessage");
        }
    }
}
