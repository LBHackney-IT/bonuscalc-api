using Microsoft.EntityFrameworkCore.Migrations;

namespace V1.Infrastructure.Migrations
{
    public partial class AddTradeRelationshipToOperatives : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_operatives_trade_id",
                table: "operatives",
                column: "trade_id");

            migrationBuilder.AddForeignKey(
                name: "fk_operatives_trades_trade_id",
                table: "operatives",
                column: "trade_id",
                principalTable: "trades",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_operatives_trades_trade_id",
                table: "operatives");

            migrationBuilder.DropIndex(
                name: "ix_operatives_trade_id",
                table: "operatives");
        }
    }
}
