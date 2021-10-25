using Microsoft.EntityFrameworkCore.Migrations;

namespace V1.Infrastructure.Migrations
{
    public partial class AddWeekdayDurationColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "friday",
                table: "pay_elements",
                type: "numeric(10,4)",
                precision: 10,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "monday",
                table: "pay_elements",
                type: "numeric(10,4)",
                precision: 10,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "saturday",
                table: "pay_elements",
                type: "numeric(10,4)",
                precision: 10,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "sunday",
                table: "pay_elements",
                type: "numeric(10,4)",
                precision: 10,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "thursday",
                table: "pay_elements",
                type: "numeric(10,4)",
                precision: 10,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "tuesday",
                table: "pay_elements",
                type: "numeric(10,4)",
                precision: 10,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "wednesday",
                table: "pay_elements",
                type: "numeric(10,4)",
                precision: 10,
                scale: 4,
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "friday",
                table: "pay_elements");

            migrationBuilder.DropColumn(
                name: "monday",
                table: "pay_elements");

            migrationBuilder.DropColumn(
                name: "saturday",
                table: "pay_elements");

            migrationBuilder.DropColumn(
                name: "sunday",
                table: "pay_elements");

            migrationBuilder.DropColumn(
                name: "thursday",
                table: "pay_elements");

            migrationBuilder.DropColumn(
                name: "tuesday",
                table: "pay_elements");

            migrationBuilder.DropColumn(
                name: "wednesday",
                table: "pay_elements");
        }
    }
}
