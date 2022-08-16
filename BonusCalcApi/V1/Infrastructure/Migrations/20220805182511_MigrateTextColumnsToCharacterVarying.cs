using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace V1.Infrastructure.Migrations
{
    public partial class MigrateTextColumnsToCharacterVarying : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW operative_projections");
            migrationBuilder.Sql("DROP VIEW operative_summaries");
            migrationBuilder.Sql("DROP VIEW operative_totals");
            migrationBuilder.Sql("DROP VIEW summaries");
            migrationBuilder.Sql("DROP VIEW weekly_summaries");

            migrationBuilder.Sql(@"
                CREATE VIEW weekly_summaries AS
                SELECT
                    CONCAT(t.operative_id, '/', w.bonus_period_id, '/', w.id)::character varying(28) AS id,
                    CONCAT(t.operative_id, '/', w.bonus_period_id)::character varying(17) AS summary_id,
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
                CREATE VIEW summaries AS
                SELECT
                    CONCAT(t.operative_id, '/', w.bonus_period_id)::character varying(17) AS id,
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
                CREATE VIEW operative_totals AS
                SELECT
                  CONCAT(ts.operative_id, '/', w.bonus_period_id)::character varying(17) AS id,
                  ts.operative_id,
                  w.bonus_period_id,
                  ROUND(SUM(COALESCE(slpe.duration, 0::numeric)), 4)::numeric(10, 4) AS sick_duration,
                  ROUND(SUM(COALESCE(tpe.value, 0::numeric)), 4)::numeric(10, 4) AS total_value,
                  ROUND(AVG(ts.utilisation), 4)::numeric(5,4) AS utilisation
                FROM
                    weeks AS w
                INNER JOIN
                    timesheets AS ts
                ON
                    w.id = ts.week_id
                LEFT JOIN
                    sick_leave_pay_elements AS slpe
                ON
                    ts.id = slpe.timesheet_id
                LEFT JOIN
                    total_pay_elements AS tpe
                ON
                    ts.id = tpe.timesheet_id
                GROUP BY
                  ts.operative_id,
                  w.bonus_period_id
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
                    o.is_archived,
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

            migrationBuilder.Sql(@"
                CREATE VIEW operative_projections AS
                SELECT
                  ot.id,
                  ot.operative_id,
                  ot.bonus_period_id,
                  CONCAT(t.description, ' (', t.id, ')') AS trade,
                  s.description AS scheme,
                  COALESCE(MAX(pb.total_value), s.min_value)::numeric(10,4) AS band_value,
                  s.max_value,
                  ot.sick_duration,
                  ot.total_value,
                  ot.utilisation,
                  o.fixed_band,
                  o.salary_band,
                  COALESCE(MAX(pb.band), 1) AS projected_band,
                  spvr.name AS supervisor_name,
                  spvr.email_address AS supervisor_email_address,
                  mgr.name AS manager_name,
                  mgr.email_address AS manager_email_address
                FROM
                    operative_totals AS ot
                INNER JOIN
                    operatives AS o
                ON
                    ot.operative_id = o.id
                INNER JOIN
                    trades AS t
                ON
                    o.trade_id = t.id
                INNER JOIN
                    schemes AS s
                ON
                    o.scheme_id = s.id
                LEFT JOIN
                    people AS spvr
                ON
                    o.supervisor_id = spvr.id
                LEFT JOIN
                    people AS mgr
                ON
                    o.manager_id = mgr.id
                LEFT JOIN
                    pay_bands AS pb
                ON
                    s.id = pb.scheme_id
                    AND (pb.total_value * ot.utilisation) <= ot.total_value
                WHERE
                    o.is_archived = FALSE
                GROUP BY
                  ot.id,
                  ot.operative_id,
                  ot.bonus_period_id,
                  t.description,
                  t.id,
                  s.description,
                  s.min_value,
                  s.max_value,
                  ot.sick_duration,
                  ot.total_value,
                  ot.utilisation,
                  o.fixed_band,
                  o.salary_band,
                  spvr.name,
                  spvr.email_address,
                  mgr.name,
                  mgr.email_address
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW operative_projections");
            migrationBuilder.Sql("DROP VIEW operative_summaries");
            migrationBuilder.Sql("DROP VIEW operative_totals");
            migrationBuilder.Sql("DROP VIEW summaries");
            migrationBuilder.Sql("DROP VIEW weekly_summaries");

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
                CREATE VIEW operative_totals AS
                SELECT
                  CONCAT(ts.operative_id, '/', w.bonus_period_id) AS id,
                  ts.operative_id,
                  w.bonus_period_id,
                  ROUND(SUM(COALESCE(slpe.duration, 0::numeric)), 4)::numeric(10, 4) AS sick_duration,
                  ROUND(SUM(COALESCE(tpe.value, 0::numeric)), 4)::numeric(10, 4) AS total_value,
                  ROUND(AVG(ts.utilisation), 4)::numeric(5,4) AS utilisation
                FROM
                    weeks AS w
                INNER JOIN
                    timesheets AS ts
                ON
                    w.id = ts.week_id
                LEFT JOIN
                    sick_leave_pay_elements AS slpe
                ON
                    ts.id = slpe.timesheet_id
                LEFT JOIN
                    total_pay_elements AS tpe
                ON
                    ts.id = tpe.timesheet_id
                GROUP BY
                  ts.operative_id,
                  w.bonus_period_id
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
                    o.is_archived,
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

            migrationBuilder.Sql(@"
                CREATE VIEW operative_projections AS
                SELECT
                  ot.id,
                  ot.operative_id,
                  ot.bonus_period_id,
                  CONCAT(t.description, ' (', t.id, ')') AS trade,
                  s.description AS scheme,
                  COALESCE(MAX(pb.total_value), s.min_value)::numeric(10,4) AS band_value,
                  s.max_value,
                  ot.sick_duration,
                  ot.total_value,
                  ot.utilisation,
                  o.fixed_band,
                  o.salary_band,
                  COALESCE(MAX(pb.band), 1) AS projected_band,
                  spvr.name AS supervisor_name,
                  spvr.email_address AS supervisor_email_address,
                  mgr.name AS manager_name,
                  mgr.email_address AS manager_email_address
                FROM
                    operative_totals AS ot
                INNER JOIN
                    operatives AS o
                ON
                    ot.operative_id = o.id
                INNER JOIN
                    trades AS t
                ON
                    o.trade_id = t.id
                INNER JOIN
                    schemes AS s
                ON
                    o.scheme_id = s.id
                LEFT JOIN
                    people AS spvr
                ON
                    o.supervisor_id = spvr.id
                LEFT JOIN
                    people AS mgr
                ON
                    o.manager_id = mgr.id
                LEFT JOIN
                    pay_bands AS pb
                ON
                    s.id = pb.scheme_id
                    AND (pb.total_value * ot.utilisation) <= ot.total_value
                WHERE
                    o.is_archived = FALSE
                GROUP BY
                  ot.id,
                  ot.operative_id,
                  ot.bonus_period_id,
                  t.description,
                  t.id,
                  s.description,
                  s.min_value,
                  s.max_value,
                  ot.sick_duration,
                  ot.total_value,
                  ot.utilisation,
                  o.fixed_band,
                  o.salary_band,
                  spvr.name,
                  spvr.email_address,
                  mgr.name,
                  mgr.email_address
            ");
        }
    }
}
