using Microsoft.EntityFrameworkCore.Migrations;

namespace V1.Infrastructure.Migrations
{
    public partial class CreateOperativeTotalsView : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW operative_totals");
        }
    }
}
