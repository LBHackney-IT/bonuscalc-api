using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace V1.Infrastructure.Migrations
{
    public partial class AddForeignKeyBetweenTradeAndBonusRates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_trades_rate_code",
                table: "trades",
                column: "rate_code");

            migrationBuilder.AddForeignKey(
                name: "fk_trades_bonus_rates_bonus_rate_id",
                table: "trades",
                column: "rate_code",
                principalTable: "bonus_rates",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_trades_bonus_rates_bonus_rate_id",
                table: "trades");

            migrationBuilder.DropIndex(
                name: "ix_trades_rate_code",
                table: "trades");
        }
    }
}
