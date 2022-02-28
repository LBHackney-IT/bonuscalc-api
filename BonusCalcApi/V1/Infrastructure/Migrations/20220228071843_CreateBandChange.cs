using BonusCalcApi.V1.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace V1.Infrastructure.Migrations
{
    public partial class CreateBandChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:band_change_decision", "approved,rejected");

            migrationBuilder.CreateTable(
                name: "band_changes",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(17)", maxLength: 17, nullable: false),
                    operative_id = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    bonus_period_id = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    trade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    scheme = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    band_value = table.Column<decimal>(type: "numeric(10,4)", precision: 10, scale: 4, nullable: false),
                    max_value = table.Column<decimal>(type: "numeric(10,4)", precision: 10, scale: 4, nullable: false),
                    sick_duration = table.Column<decimal>(type: "numeric(10,4)", precision: 10, scale: 4, nullable: false),
                    total_value = table.Column<decimal>(type: "numeric(10,4)", precision: 10, scale: 4, nullable: false),
                    utilisation = table.Column<decimal>(type: "numeric(10,4)", precision: 10, scale: 4, nullable: false),
                    fixed_band = table.Column<bool>(type: "boolean", nullable: false),
                    salary_band = table.Column<int>(type: "integer", nullable: false),
                    projected_band = table.Column<int>(type: "integer", nullable: false),
                    supervisor_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    supervisor_email_address = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    supervisor_decision = table.Column<BandChangeDecision>(type: "band_change_decision", nullable: true),
                    supervisor_reason = table.Column<string>(type: "text", nullable: true),
                    supervisor_salary_band = table.Column<int>(type: "integer", nullable: true),
                    manager_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    manager_email_address = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    manager_decision = table.Column<BandChangeDecision>(type: "band_change_decision", nullable: true),
                    manager_reason = table.Column<string>(type: "text", nullable: true),
                    manager_salary_band = table.Column<int>(type: "integer", nullable: true),
                    final_band = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_band_changes", x => x.id);
                    table.ForeignKey(
                        name: "fk_band_changes_bonus_periods_bonus_period_id",
                        column: x => x.bonus_period_id,
                        principalTable: "bonus_periods",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_band_changes_operatives_operative_id",
                        column: x => x.operative_id,
                        principalTable: "operatives",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_band_changes_bonus_period_id",
                table: "band_changes",
                column: "bonus_period_id");

            migrationBuilder.CreateIndex(
                name: "ix_band_changes_operative_id",
                table: "band_changes",
                column: "operative_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "band_changes");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:Enum:band_change_decision", "approved,rejected");
        }
    }
}
