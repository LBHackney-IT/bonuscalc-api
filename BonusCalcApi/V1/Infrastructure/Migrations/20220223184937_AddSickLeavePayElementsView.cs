using Microsoft.EntityFrameworkCore.Migrations;

namespace V1.Infrastructure.Migrations
{
    public partial class AddSickLeavePayElementsView : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE VIEW sick_leave_pay_elements AS
                SELECT
                    p.timesheet_id,
                    SUM(p.duration)::numeric(10,4) AS duration
                FROM
                    pay_elements AS p
                INNER JOIN
                    pay_element_types AS t
                ON
                    p.pay_element_type_id = t.id
                WHERE
                    t.sick_leave = TRUE
                GROUP BY
                    p.timesheet_id
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW sick_leave_pay_elements");
        }
    }
}
