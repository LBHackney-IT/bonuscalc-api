using Microsoft.EntityFrameworkCore.Migrations;

namespace V1.Infrastructure.Migrations
{
    public partial class AddGeneratedColumnsToPayBands : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "smv_per_hour",
                table: "pay_bands",
                type: "numeric(20,14)",
                precision: 20,
                scale: 14,
                nullable: false,
                computedColumnSql: "value / 36",
                stored: true);

            migrationBuilder.AddColumn<decimal>(
                name: "total_value",
                table: "pay_bands",
                type: "numeric(10,4)",
                precision: 10,
                scale: 4,
                nullable: false,
                computedColumnSql: "value * 13",
                stored: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "smv_per_hour",
                table: "pay_bands");

            migrationBuilder.DropColumn(
                name: "total_value",
                table: "pay_bands");
        }
    }
}
