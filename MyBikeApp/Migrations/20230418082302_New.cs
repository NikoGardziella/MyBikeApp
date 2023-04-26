using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBikeApp.Migrations
{
    public partial class New : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
       /*     migrationBuilder.RenameColumn(
                name: "DurationSec",
                table: "Journey",
                newName: "Duration");

            migrationBuilder.RenameColumn(
                name: "CoveredDistanceM",
                table: "Journey",
                newName: "CoveredDistance");

            migrationBuilder.AddColumn<string>(
                name: "Lat",
                table: "Station",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Lon",
                table: "Station",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "LoginViewModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginViewModel", x => x.Id);
                }); 
       */
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoginViewModel");

            migrationBuilder.DropColumn(
                name: "Lat",
                table: "Station");

            migrationBuilder.DropColumn(
                name: "Lon",
                table: "Station");

            migrationBuilder.RenameColumn(
                name: "Duration",
                table: "Journey",
                newName: "DurationSec");

            migrationBuilder.RenameColumn(
                name: "CoveredDistance",
                table: "Journey",
                newName: "CoveredDistanceM");
        }
    }
}
