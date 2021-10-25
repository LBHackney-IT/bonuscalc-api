using Microsoft.EntityFrameworkCore.Migrations;

namespace V1.Infrastructure.Migrations
{
    public partial class DropPayElementWeekDay : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "week_day",
                table: "pay_elements");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "week_day",
                table: "pay_elements",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
