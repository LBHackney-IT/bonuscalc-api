using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace V1.Infrastructure.Migrations
{
    public partial class AddRateCodeToOperativeProjections : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW operative_projections");

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
                  mgr.email_address AS manager_email_address,
                  t.rate_code
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
            migrationBuilder.Sql(@"DROP VIEW operative_projections");

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
