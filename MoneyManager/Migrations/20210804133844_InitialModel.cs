using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MoneyManager.Migrations
{
    public partial class InitialModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MoneyUsages",
                columns: table => new
                {
                    GradId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CardType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UsedLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MoneyUsed = table.Column<long>(type: "bigint", nullable: false),
                    UsedType = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoneyUsages", x => x.GradId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MoneyUsages");
        }
    }
}
