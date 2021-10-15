using Microsoft.EntityFrameworkCore.Migrations;

namespace V1.Infrastructure.Migrations
{
    public partial class RenameIds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "week_id",
                table: "weeks",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "bonus_period_id",
                table: "bonus_periods",
                newName: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id",
                table: "weeks",
                newName: "week_id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "bonus_periods",
                newName: "bonus_period_id");
        }
    }
}
