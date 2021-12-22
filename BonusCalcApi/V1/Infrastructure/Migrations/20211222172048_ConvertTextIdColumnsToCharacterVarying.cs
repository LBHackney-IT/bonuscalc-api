using Microsoft.EntityFrameworkCore.Migrations;

namespace V1.Infrastructure.Migrations
{
    public partial class ConvertTextIdColumnsToCharacterVarying : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            DropViews(migrationBuilder);

            migrationBuilder.DropForeignKey(
                name: "fk_timesheets_weeks_week_id",
                table: "timesheets");

            migrationBuilder.DropForeignKey(
                name: "fk_weeks_bonus_periods_bonus_period_id",
                table: "weeks");

            migrationBuilder.AlterColumn<string>(
                name: "bonus_period_id",
                table: "weeks",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "id",
                table: "weeks",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "week_id",
                table: "timesheets",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "id",
                table: "bonus_periods",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

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

            CreateViews(migrationBuilder);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            DropViews(migrationBuilder);

            migrationBuilder.DropForeignKey(
                name: "fk_timesheets_weeks_week_id",
                table: "timesheets");

            migrationBuilder.DropForeignKey(
                name: "fk_weeks_bonus_periods_bonus_period_id",
                table: "weeks");

            migrationBuilder.AlterColumn<string>(
                name: "bonus_period_id",
                table: "weeks",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "id",
                table: "weeks",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "week_id",
                table: "timesheets",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "id",
                table: "bonus_periods",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.AddForeignKey(
                name: "fk_timesheets_weeks_week_id",
                table: "timesheets",
                column: "week_id",
                principalTable: "weeks",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_weeks_bonus_periods_bonus_period_id",
                table: "weeks",
                column: "bonus_period_id",
                principalTable: "bonus_periods",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            CreateViews(migrationBuilder);
        }

        protected static void CreateViews(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE VIEW summaries AS
SELECT
    CONCAT(t.operative_id, '/', w.bonus_period_id) AS id,
    t.operative_id,
    w.bonus_period_id
FROM
    timesheets AS t
INNER JOIN
    weeks AS w
ON
    t.week_id = w.id
GROUP BY
    t.operative_id,
    w.bonus_period_id
            ");

            migrationBuilder.Sql(@"
CREATE VIEW weekly_summaries AS
SELECT
    CONCAT(t.operative_id, '/', w.bonus_period_id, '/', w.id) AS id,
    CONCAT(t.operative_id, '/', w.bonus_period_id) AS summary_id,
    w.id AS week_id,
    t.operative_id,
	w.number,
	w.start_at,
	w.closed_at,
	COALESCE(p.value, 0)::numeric(10,4) AS productive_value,
	COALESCE(np.duration, 0)::numeric(10,4) AS non_productive_duration,
	COALESCE(np.value, 0)::numeric(10,4) AS non_productive_value,
	(COALESCE(p.value, 0) + COALESCE(np.value, 0))::numeric(10,4) AS total_value,
	t.utilisation,
	ROUND(
        AVG(COALESCE(p.value, 0) + COALESCE(np.value, 0))
        OVER (
            PARTITION BY w.bonus_period_id, t.operative_id
            ORDER BY w.number ASC
    ), 4)::numeric(10,4) AS projected_value,
    ROUND(
        AVG(t.utilisation)
        OVER (
            PARTITION BY w.bonus_period_id, t.operative_id
            ORDER BY w.number ASC
    ), 4)::numeric(5,4) AS average_utilisation
FROM
    weeks AS w
INNER JOIN
    timesheets AS t
ON
    w.id = t.week_id
LEFT JOIN
    productive_pay_elements AS p
ON
    t.id = p.timesheet_id
LEFT JOIN
    non_productive_pay_elements AS np
ON t.id = np.timesheet_id
            ");

            migrationBuilder.Sql(@"
CREATE VIEW work_elements AS
SELECT
    pe.id,
    pet.description AS type,
    pe.work_order,
    pe.address,
    pe.comment AS description,
    t.operative_id,
    o.name AS operative_name,
    t.week_id,
    pe.value,
    pe.closed_at,
    pe.search_vector
FROM
    pay_elements AS pe
INNER JOIN
    pay_element_types AS pet
ON
    pe.pay_element_type_id = pet.id
INNER JOIN
    timesheets AS t
ON
    pe.timesheet_id = t.id
INNER JOIN
    operatives AS o
ON
    t.operative_id = o.id;
            ");

            migrationBuilder.Sql(@"
CREATE VIEW operative_summaries AS
SELECT
    ws.operative_id AS id,
    ws.week_id,
    o.name,
    o.trade_id,
    t.description AS trade_description,
    o.scheme_id,
    ws.productive_value,
    ws.non_productive_duration,
    ws.non_productive_value,
    ws.total_value,
    ws.utilisation,
    ws.projected_value,
    ws.average_utilisation
FROM
    weekly_summaries AS ws
INNER JOIN
    operatives AS o
ON
    ws.operative_id = o.id
INNER JOIN
    trades AS t
ON
    o.trade_id = t.id
            ");
        }

        protected static void DropViews(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW operative_summaries");
            migrationBuilder.Sql("DROP VIEW work_elements");
            migrationBuilder.Sql("DROP VIEW weekly_summaries");
            migrationBuilder.Sql("DROP VIEW summaries");
        }
    }
}
