using Microsoft.EntityFrameworkCore.Migrations;

namespace V1.Infrastructure.Migrations
{
    public partial class AddTotalPayElementsView : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE VIEW total_pay_elements AS
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
                    t.productive = TRUE OR t.non_productive = TRUE
                GROUP BY
                    p.timesheet_id
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW total_pay_elements");
        }
    }
}
