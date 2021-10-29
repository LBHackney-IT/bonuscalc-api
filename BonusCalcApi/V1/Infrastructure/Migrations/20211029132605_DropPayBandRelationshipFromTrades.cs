using Microsoft.EntityFrameworkCore.Migrations;

namespace V1.Infrastructure.Migrations
{
    public partial class DropPayBandRelationshipFromTrades : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_pay_bands_trades_trade_id",
                table: "pay_bands");

            migrationBuilder.DropIndex(
                name: "ix_pay_bands_trade_id_band",
                table: "pay_bands");

            migrationBuilder.DropColumn(
                name: "trade_id",
                table: "pay_bands");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "trade_id",
                table: "pay_bands",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "ix_pay_bands_trade_id_band",
                table: "pay_bands",
                columns: new[] { "trade_id", "band" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_pay_bands_trades_trade_id",
                table: "pay_bands",
                column: "trade_id",
                principalTable: "trades",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
