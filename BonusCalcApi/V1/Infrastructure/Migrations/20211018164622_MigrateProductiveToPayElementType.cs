using Microsoft.EntityFrameworkCore.Migrations;

namespace V1.Infrastructure.Migrations
{
    public partial class MigrateProductiveToPayElementType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "productive",
                table: "pay_elements");

            migrationBuilder.AddColumn<bool>(
                name: "adjustment",
                table: "pay_element_types",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "productive",
                table: "pay_element_types",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "adjustment",
                table: "pay_element_types");

            migrationBuilder.DropColumn(
                name: "productive",
                table: "pay_element_types");

            migrationBuilder.AddColumn<bool>(
                name: "productive",
                table: "pay_elements",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
