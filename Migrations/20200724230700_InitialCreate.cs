using Microsoft.EntityFrameworkCore.Migrations;

namespace dotnet_function_sqldb.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "deviceData",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Message = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_deviceData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "lookupData",
                columns: table => new
                {
                    id = table.Column<string>(nullable: false),
                    message = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lookupData", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "deviceData");

            migrationBuilder.DropTable(
                name: "lookupData");
        }
    }
}
