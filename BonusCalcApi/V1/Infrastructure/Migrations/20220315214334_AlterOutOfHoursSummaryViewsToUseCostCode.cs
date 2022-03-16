using Microsoft.EntityFrameworkCore.Migrations;

namespace V1.Infrastructure.Migrations
{
    public partial class AlterOutOfHoursSummaryViewsToUseCostCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW out_of_hours_summaries");
            migrationBuilder.Sql("DROP VIEW out_of_hours_pay_elements");

            migrationBuilder.Sql(@"
                CREATE VIEW out_of_hours_pay_elements AS
                SELECT
                    p.timesheet_id,
                    p.cost_code,
                    ROUND(SUM(p.value), 2)::numeric(8,2) AS total_value
                FROM
                    pay_elements p
                INNER JOIN
                    pay_element_types t
                ON
                    p.pay_element_type_id = t.id
                WHERE
                    t.out_of_hours = true
                GROUP BY
                    p.timesheet_id,
                    p.cost_code
            ");

            migrationBuilder.Sql(@"
                CREATE VIEW out_of_hours_summaries AS
                SELECT
                    o.id,
                    ts.week_id,
                    o.name,
                    o.trade_id,
                    t.description AS trade_description,
                    COALESCE(p.cost_code, o.section) AS cost_code,
                    p.total_value
                FROM
                    weeks AS w
                INNER JOIN
                    timesheets ts
                ON
                    w.id = ts.week_id
                INNER JOIN
                    out_of_hours_pay_elements p
                ON
                    ts.id = p.timesheet_id
                INNER JOIN
                    operatives AS o
                ON
                    ts.operative_id = o.id
                INNER JOIN
                    trades AS t
                ON
                    o.trade_id = t.id
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW out_of_hours_summaries");
            migrationBuilder.Sql("DROP VIEW out_of_hours_pay_elements");

            migrationBuilder.Sql(@"
                CREATE VIEW out_of_hours_pay_elements AS
                SELECT
                    p.timesheet_id,
                    p.trade_code,
                    ROUND(SUM(p.value), 2)::numeric(8,2) AS total_value
                FROM
                    pay_elements p
                INNER JOIN
                    pay_element_types t
                ON
                    p.pay_element_type_id = t.id
                WHERE
                    t.out_of_hours = true
                GROUP BY
                    p.timesheet_id,
                    p.trade_code
            ");

            migrationBuilder.Sql(@"
                CREATE VIEW out_of_hours_summaries AS
                SELECT
                    o.id,
                    ts.week_id,
                    o.name,
                    o.trade_id,
                    t.description AS trade_description,
                    p.trade_code,
                    p.total_value
                FROM
                    weeks AS w
                INNER JOIN
                    timesheets ts
                ON
                    w.id = ts.week_id
                INNER JOIN
                    out_of_hours_pay_elements p
                ON
                    ts.id = p.timesheet_id
                INNER JOIN
                    operatives AS o
                ON
                    ts.operative_id = o.id
                INNER JOIN
                    trades AS t
                ON
                    o.trade_id = t.id
            ");
        }
    }
}
