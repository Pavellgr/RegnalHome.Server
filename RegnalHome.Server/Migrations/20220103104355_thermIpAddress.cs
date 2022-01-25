using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegnalHome.Server.Migrations
{
    public partial class thermIpAddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "ThermSensors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "ThermCus",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "ThermSensors");

            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "ThermCus");
        }
    }
}
