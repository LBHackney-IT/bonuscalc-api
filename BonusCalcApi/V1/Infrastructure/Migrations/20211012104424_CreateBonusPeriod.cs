using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace V1.Infrastructure.Migrations
{
    public partial class CreateBonusPeriod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "bonus_periods",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    start_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    year = table.Column<int>(type: "integer", nullable: false),
                    period = table.Column<int>(type: "integer", nullable: false),
                    closed_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bonus_periods", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_bonus_periods_start_at",
                table: "bonus_periods",
                column: "start_at",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_bonus_periods_year_period",
                table: "bonus_periods",
                columns: new[] { "year", "period" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bonus_periods");
        }
    }
}
