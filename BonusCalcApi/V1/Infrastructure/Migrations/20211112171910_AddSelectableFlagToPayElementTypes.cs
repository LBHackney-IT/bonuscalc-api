using Microsoft.EntityFrameworkCore.Migrations;

namespace V1.Infrastructure.Migrations
{
    public partial class AddSelectableFlagToPayElementTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "selectable",
                table: "pay_element_types",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            // Set initial values for existing non-editable pay element types
            migrationBuilder.Sql("UPDATE pay_element_types SET selectable = TRUE WHERE id < 200");

            // Ensure adjustable pay element types are marked as productive
            migrationBuilder.Sql("UPDATE pay_element_types SET productive = TRUE WHERE adjustment = TRUE");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "selectable",
                table: "pay_element_types");
        }
    }
}
