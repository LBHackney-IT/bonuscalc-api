using Microsoft.EntityFrameworkCore.Migrations;

namespace V1.Infrastructure.Migrations
{
    public partial class RenameBonusPeriodPeriodToNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "period",
                table: "bonus_periods",
                newName: "number");

            migrationBuilder.RenameIndex(
                name: "ix_bonus_periods_year_period",
                table: "bonus_periods",
                newName: "ix_bonus_periods_year_number");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "number",
                table: "bonus_periods",
                newName: "period");

            migrationBuilder.RenameIndex(
                name: "ix_bonus_periods_year_number",
                table: "bonus_periods",
                newName: "ix_bonus_periods_year_period");
        }
    }
}
