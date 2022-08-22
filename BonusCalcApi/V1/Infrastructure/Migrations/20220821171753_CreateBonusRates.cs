using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace V1.Infrastructure.Migrations
{
    public partial class CreateBonusRates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "bonus_rates",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    rates = table.Column<List<decimal>>(type: "numeric(8,2)[]", precision: 8, scale: 2, nullable: false, defaultValueSql: "ARRAY[]::numeric(8,2)[]")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bonus_rates", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bonus_rates");
        }
    }
}
