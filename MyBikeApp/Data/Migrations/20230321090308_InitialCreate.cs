using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBikeApp.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartStations",
                table: "Station",
                newName: "ReturnStationName");

            migrationBuilder.RenameColumn(
                name: "EndStation",
                table: "Station",
                newName: "Return");

            migrationBuilder.AddColumn<int>(
                name: "CoveredDistanceM",
                table: "Station",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Departure",
                table: "Station",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DepartureStationId",
                table: "Station",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "DepartureStationName",
                table: "Station",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DurationSec",
                table: "Station",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReturnStationId",
                table: "Station",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoveredDistanceM",
                table: "Station");

            migrationBuilder.DropColumn(
                name: "Departure",
                table: "Station");

            migrationBuilder.DropColumn(
                name: "DepartureStationId",
                table: "Station");

            migrationBuilder.DropColumn(
                name: "DepartureStationName",
                table: "Station");

            migrationBuilder.DropColumn(
                name: "DurationSec",
                table: "Station");

            migrationBuilder.DropColumn(
                name: "ReturnStationId",
                table: "Station");

            migrationBuilder.RenameColumn(
                name: "ReturnStationName",
                table: "Station",
                newName: "StartStations");

            migrationBuilder.RenameColumn(
                name: "Return",
                table: "Station",
                newName: "EndStation");
        }
    }
}
