using Microsoft.EntityFrameworkCore.Migrations;

namespace V1.Infrastructure.Migrations
{
    public partial class AddClosedByToWeeks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "closed_by",
                table: "weeks",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "closed_by",
                table: "weeks");
        }
    }
}
