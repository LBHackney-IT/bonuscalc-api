using Microsoft.EntityFrameworkCore.Migrations;

namespace V1.Infrastructure.Migrations
{
    public partial class AddReportSentAtToSummaryViews : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            DropViews(migrationBuilder);

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
    ), 4)::numeric(5,4) AS average_utilisation,
    t.report_sent_at
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
    ws.average_utilisation,
    ws.report_sent_at
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            DropViews(migrationBuilder);

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
            migrationBuilder.Sql("DROP VIEW weekly_summaries");
        }
    }
}
