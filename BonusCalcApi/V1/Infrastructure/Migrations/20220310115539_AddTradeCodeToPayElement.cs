using Microsoft.EntityFrameworkCore.Migrations;

namespace V1.Infrastructure.Migrations
{
    public partial class AddTradeCodeToPayElement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "trade_code",
                table: "pay_elements",
                type: "character varying(3)",
                maxLength: 3,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_pay_elements_trade_code",
                table: "pay_elements",
                column: "trade_code");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_pay_elements_trade_code",
                table: "pay_elements");

            migrationBuilder.DropColumn(
                name: "trade_code",
                table: "pay_elements");
        }
    }
}
