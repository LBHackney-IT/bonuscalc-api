using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace V1.Infrastructure.Migrations
{
    public partial class CreateTimesheet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "timesheets",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    operative_id = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    week_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_timesheets", x => x.id);
                    table.ForeignKey(
                        name: "fk_timesheets_operatives_operative_id",
                        column: x => x.operative_id,
                        principalTable: "operatives",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_timesheets_weeks_week_id",
                        column: x => x.week_id,
                        principalTable: "weeks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_timesheets_operative_id_week_id",
                table: "timesheets",
                columns: new[] { "operative_id", "week_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_timesheets_week_id",
                table: "timesheets",
                column: "week_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "timesheets");
        }
    }
}
