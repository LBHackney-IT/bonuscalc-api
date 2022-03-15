using Microsoft.EntityFrameworkCore.Migrations;

namespace V1.Infrastructure.Migrations
{
    public partial class AddCostCodeToPayElement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "cost_code",
                table: "pay_elements",
                type: "character varying(5)",
                maxLength: 5,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_pay_elements_cost_code",
                table: "pay_elements",
                column: "cost_code");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_pay_elements_cost_code",
                table: "pay_elements");

            migrationBuilder.DropColumn(
                name: "cost_code",
                table: "pay_elements");
        }
    }
}
