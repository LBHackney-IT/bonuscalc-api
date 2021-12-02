using Microsoft.EntityFrameworkCore.Migrations;

namespace V1.Infrastructure.Migrations
{
    public partial class CreateWorkOrderView : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW work_elements");
        }
    }
}
