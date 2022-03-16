using Microsoft.EntityFrameworkCore.Migrations;

namespace V1.Infrastructure.Migrations
{
    public partial class AddMaxValueToScheme : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "max_value",
                table: "schemes",
                type: "numeric(10,4)",
                precision: 10,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.Sql(@"
                UPDATE schemes AS s
                SET max_value = (
                  SELECT MAX(value) * 13
                  FROM pay_bands AS pb
                  WHERE pb.scheme_id = s.id
                )
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "max_value",
                table: "schemes");
        }
    }
}
