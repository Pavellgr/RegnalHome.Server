using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegnalHome.Server.Migrations
{
    public partial class asd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IpAddress",
                table: "ThermSensors",
                newName: "Address");

            migrationBuilder.RenameColumn(
                name: "IpAddress",
                table: "ThermCus",
                newName: "Address");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Address",
                table: "ThermSensors",
                newName: "IpAddress");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "ThermCus",
                newName: "IpAddress");
        }
    }
}
