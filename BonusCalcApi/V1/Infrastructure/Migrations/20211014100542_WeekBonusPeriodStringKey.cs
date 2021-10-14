using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace V1.Infrastructure.Migrations
{
    public partial class WeekBonusPeriodStringKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_timesheets_weeks_week_id",
                table: "timesheets");

            migrationBuilder.DropForeignKey(
                name: "fk_weeks_bonus_periods_bonus_period_id",
                table: "weeks");

            migrationBuilder.DropPrimaryKey(
                name: "pk_weeks",
                table: "weeks");

            migrationBuilder.DropPrimaryKey(
                name: "pk_bonus_periods",
                table: "bonus_periods");

            migrationBuilder.DropColumn(
                name: "id",
                table: "weeks");

            migrationBuilder.DropColumn(
                name: "id",
                table: "bonus_periods");

            migrationBuilder.AlterColumn<string>(
                name: "bonus_period_id",
                table: "weeks",
                type: "text",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "week_id",
                table: "weeks",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "week_id",
                table: "timesheets",
                type: "text",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "bonus_period_id",
                table: "bonus_periods",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "pk_weeks",
                table: "weeks",
                column: "week_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_bonus_periods",
                table: "bonus_periods",
                column: "bonus_period_id");

            migrationBuilder.AddForeignKey(
                name: "fk_timesheets_weeks_week_id",
                table: "timesheets",
                column: "week_id",
                principalTable: "weeks",
                principalColumn: "week_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_weeks_bonus_periods_bonus_period_id",
                table: "weeks",
                column: "bonus_period_id",
                principalTable: "bonus_periods",
                principalColumn: "bonus_period_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_timesheets_weeks_week_id",
                table: "timesheets");

            migrationBuilder.DropForeignKey(
                name: "fk_weeks_bonus_periods_bonus_period_id",
                table: "weeks");

            migrationBuilder.DropPrimaryKey(
                name: "pk_weeks",
                table: "weeks");

            migrationBuilder.DropPrimaryKey(
                name: "pk_bonus_periods",
                table: "bonus_periods");

            migrationBuilder.DropColumn(
                name: "week_id",
                table: "weeks");

            migrationBuilder.DropColumn(
                name: "bonus_period_id",
                table: "bonus_periods");

            migrationBuilder.AlterColumn<int>(
                name: "bonus_period_id",
                table: "weeks",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "id",
                table: "weeks",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "week_id",
                table: "timesheets",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "id",
                table: "bonus_periods",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "pk_weeks",
                table: "weeks",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_bonus_periods",
                table: "bonus_periods",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_timesheets_weeks_week_id",
                table: "timesheets",
                column: "week_id",
                principalTable: "weeks",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_weeks_bonus_periods_bonus_period_id",
                table: "weeks",
                column: "bonus_period_id",
                principalTable: "bonus_periods",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
