using Microsoft.EntityFrameworkCore.Migrations;

namespace V1.Infrastructure.Migrations
{
    public partial class CreateWeeklySummaryViews : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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
            migrationBuilder.Sql("DROP VIEW weekly_summaries");
            migrationBuilder.Sql("DROP VIEW non_productive_pay_elements");
            migrationBuilder.Sql("DROP VIEW productive_pay_elements");
            migrationBuilder.Sql("DROP VIEW summaries");
        }
    }
}
