using Microsoft.EntityFrameworkCore.Migrations;

namespace V1.Infrastructure.Migrations
{
    public partial class AddSmvPerHourToPayElementTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "smv_per_hour",
                table: "pay_element_types",
                type: "integer",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE pay_element_types
                SET smv_per_hour = 60
                WHERE description = 'Apprentice'
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "smv_per_hour",
                table: "pay_element_types");
        }
    }
}
