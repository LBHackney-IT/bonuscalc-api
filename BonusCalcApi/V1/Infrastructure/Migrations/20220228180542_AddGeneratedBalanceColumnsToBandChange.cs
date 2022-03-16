using Microsoft.EntityFrameworkCore.Migrations;

namespace V1.Infrastructure.Migrations
{
    public partial class AddGeneratedBalanceColumnsToBandChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "balance_duration",
                table: "band_changes",
                type: "numeric(10,4)",
                precision: 10,
                scale: 4,
                nullable: false,
                computedColumnSql: "ROUND(GREATEST(LEAST(max_value * utilisation, total_value * (NOT fixed_band)::int) -  band_value * utilisation, 0) / 60, 4)",
                stored: true);

            migrationBuilder.AddColumn<decimal>(
                name: "balance_value",
                table: "band_changes",
                type: "numeric(10,4)",
                precision: 10,
                scale: 4,
                nullable: false,
                computedColumnSql: "GREATEST(LEAST(max_value * utilisation, total_value * (NOT fixed_band)::int) -  band_value * utilisation, 0)",
                stored: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "balance_duration",
                table: "band_changes");

            migrationBuilder.DropColumn(
                name: "balance_value",
                table: "band_changes");
        }
    }
}
