using Microsoft.EntityFrameworkCore.Migrations;

namespace V1.Infrastructure.Migrations
{
    public partial class CreateOperative : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "operatives",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    trade = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    section = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    scheme = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    salary_band = table.Column<int>(nullable: false),
                    fixed_band = table.Column<bool>(nullable: false),
                    is_archived = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_operatives", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "operatives");
        }
    }
}
