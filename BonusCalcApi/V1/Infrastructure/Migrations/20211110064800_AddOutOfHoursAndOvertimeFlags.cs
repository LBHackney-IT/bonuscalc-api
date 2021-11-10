using Microsoft.EntityFrameworkCore.Migrations;

namespace V1.Infrastructure.Migrations
{
    public partial class AddOutOfHoursAndOvertimeFlags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "out_of_hours",
                table: "pay_element_types",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "overtime",
                table: "pay_element_types",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "out_of_hours",
                table: "pay_element_types");

            migrationBuilder.DropColumn(
                name: "overtime",
                table: "pay_element_types");
        }
    }
}
