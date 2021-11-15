using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace V1.Infrastructure.Migrations
{
    public partial class ConvertTimesheetIdToString : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "new_id",
                table: "timesheets",
                type: "character varying(17)",
                maxLength: 17,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "new_timesheet_id",
                table: "pay_elements",
                type: "character varying(17)",
                maxLength: 17,
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE timesheets
                SET new_id = CONCAT(operative_id, '/', week_id)
            ");

            migrationBuilder.Sql(@"
                UPDATE pay_elements AS p
                SET new_timesheet_id = t.new_id
                FROM timesheets AS t
                WHERE p.timesheet_id = t.id
            ");

            migrationBuilder.AlterColumn<string>(
                name: "new_id",
                table: "timesheets",
                type: "character varying(17)",
                maxLength: 17,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(17)",
                oldMaxLength: 17,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "new_timesheet_id",
                table: "pay_elements",
                type: "character varying(17)",
                maxLength: 17,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(17)",
                oldMaxLength: 17,
                oldNullable: true);

            migrationBuilder.DropForeignKey(
                name: "fk_pay_elements_timesheets_timesheet_id",
                table: "pay_elements");

            migrationBuilder.DropPrimaryKey(
                name: "pk_timesheets",
                table: "timesheets");

            migrationBuilder.Sql("DROP VIEW weekly_summaries");
            migrationBuilder.Sql("DROP VIEW productive_pay_elements");
            migrationBuilder.Sql("DROP VIEW non_productive_pay_elements");

            migrationBuilder.DropColumn(
                name: "id",
                table: "timesheets");

            migrationBuilder.DropColumn(
                name: "timesheet_id",
                table: "pay_elements");

            migrationBuilder.RenameColumn(
                name: "new_timesheet_id",
                table: "pay_elements",
                newName: "timesheet_id");

            migrationBuilder.RenameColumn(
                name: "new_id",
                table: "timesheets",
                newName: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_timesheets",
                table: "timesheets",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_pay_elements_timesheets_timesheet_id",
                table: "pay_elements",
                column: "timesheet_id",
                principalTable: "timesheets",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql(@"
CREATE VIEW productive_pay_elements AS
SELECT
    p.timesheet_id,
    SUM(p.value)::numeric(10,4) AS value
FROM
    pay_elements AS p
INNER JOIN
    pay_element_types AS t
ON
    p.pay_element_type_id = t.id
WHERE
    t.productive = TRUE
GROUP BY
    p.timesheet_id
            ");

            migrationBuilder.Sql(@"
CREATE VIEW non_productive_pay_elements AS
SELECT
    p.timesheet_id,
    SUM(p.duration)::numeric(10,4) AS duration,
    SUM(p.value)::numeric(10,4) AS value
FROM
    pay_elements AS p
INNER JOIN
    pay_element_types AS t
ON
    p.pay_element_type_id = t.id
WHERE
    t.non_productive = TRUE
GROUP BY
    p.timesheet_id
            ");

            migrationBuilder.Sql(@"
CREATE VIEW weekly_summaries AS
SELECT
    CONCAT(t.operative_id, '/', w.bonus_period_id, '/', w.id) AS id,
    CONCAT(t.operative_id, '/', w.bonus_period_id) AS summary_id,
	w.number,
	w.start_at,
	w.closed_at,
	COALESCE(p.value, 0)::numeric(10,4) AS productive_value,
	COALESCE(np.duration, 0)::numeric(10,4) AS non_productive_duration,
	COALESCE(np.value, 0)::numeric(10,4) AS non_productive_value,
	(COALESCE(p.value, 0) + COALESCE(np.value, 0))::numeric(10,4) AS total_value,
	ROUND(
        AVG(COALESCE(p.value, 0) + COALESCE(np.value, 0))
        OVER (
            PARTITION BY w.bonus_period_id, t.operative_id
            ORDER BY w.number ASC
    ), 4)::numeric(10,4) AS projected_value
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "old_id",
                table: "timesheets",
                type: "integer",
                nullable: false)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "old_timesheet_id",
                table: "pay_elements",
                type: "integer",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE pay_elements AS p
                SET old_timesheet_id = t.old_id
                FROM timesheets AS t
                WHERE p.timesheet_id = t.id
            ");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "pay_element_types",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.DropForeignKey(
                name: "fk_pay_elements_timesheets_timesheet_id",
                table: "pay_elements");

            migrationBuilder.DropPrimaryKey(
                name: "pk_timesheets",
                table: "timesheets");

            migrationBuilder.Sql("DROP VIEW weekly_summaries");
            migrationBuilder.Sql("DROP VIEW productive_pay_elements");
            migrationBuilder.Sql("DROP VIEW non_productive_pay_elements");

            migrationBuilder.DropColumn(
                name: "id",
                table: "timesheets");

            migrationBuilder.DropColumn(
                name: "timesheet_id",
                table: "pay_elements");

            migrationBuilder.RenameColumn(
                name: "old_timesheet_id",
                table: "pay_elements",
                newName: "timesheet_id");

            migrationBuilder.RenameColumn(
                name: "old_id",
                table: "timesheets",
                newName: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_timesheets",
                table: "timesheets",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_pay_elements_timesheets_timesheet_id",
                table: "pay_elements",
                column: "timesheet_id",
                principalTable: "timesheets",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql(@"
CREATE VIEW productive_pay_elements AS
SELECT
    p.timesheet_id,
    SUM(p.value)::numeric(10,4) AS value
FROM
    pay_elements AS p
INNER JOIN
    pay_element_types AS t
ON
    p.pay_element_type_id = t.id
WHERE
    t.productive = TRUE
GROUP BY
    p.timesheet_id
            ");

            migrationBuilder.Sql(@"
CREATE VIEW non_productive_pay_elements AS
SELECT
    p.timesheet_id,
    SUM(p.duration)::numeric(10,4) AS duration,
    SUM(p.value)::numeric(10,4) AS value
FROM
    pay_elements AS p
INNER JOIN
    pay_element_types AS t
ON
    p.pay_element_type_id = t.id
WHERE
    t.non_productive = TRUE
GROUP BY
    p.timesheet_id
            ");

            migrationBuilder.Sql(@"
CREATE VIEW weekly_summaries AS
SELECT
    CONCAT(t.operative_id, '/', w.bonus_period_id, '/', w.id) AS id,
    CONCAT(t.operative_id, '/', w.bonus_period_id) AS summary_id,
	w.number,
	w.start_at,
	w.closed_at,
	COALESCE(p.value, 0)::numeric(10,4) AS productive_value,
	COALESCE(np.duration, 0)::numeric(10,4) AS non_productive_duration,
	COALESCE(np.value, 0)::numeric(10,4) AS non_productive_value,
	(COALESCE(p.value, 0) + COALESCE(np.value, 0))::numeric(10,4) AS total_value,
	ROUND(
        AVG(COALESCE(p.value, 0) + COALESCE(np.value, 0))
        OVER (
            PARTITION BY w.bonus_period_id, t.operative_id
            ORDER BY w.number ASC
    ), 4)::numeric(10,4) AS projected_value
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
        }
    }
}
