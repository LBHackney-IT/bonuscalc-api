using Microsoft.EntityFrameworkCore.Migrations;

namespace V1.Infrastructure.Migrations
{
    public partial class AddNonProductiveFlagToPayElementTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "non_productive",
                table: "pay_element_types",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            // Flag current non-productive pay element types
            migrationBuilder.Sql(@"
                UPDATE pay_element_types SET non_productive = TRUE
                WHERE productive = FALSE AND adjustment = FALSE
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "non_productive",
                table: "pay_element_types");
        }
    }
}
