using Microsoft.EntityFrameworkCore.Migrations;

namespace V1.Infrastructure.Migrations
{
    public partial class TruncateOperativeTradeId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // The data in staging is fake so this is just so that we can
            // apply the migrations and add a foreign in a subsequent migration.
            migrationBuilder.Sql("UPDATE operatives SET trade_id = 'EL'");
            migrationBuilder.Sql("INSERT INTO trades VALUES ('EL', 'Electrician')");

            migrationBuilder.AlterColumn<string>(
                name: "trade_id",
                table: "operatives",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "trade_id",
                table: "operatives",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3);
        }
    }
}
