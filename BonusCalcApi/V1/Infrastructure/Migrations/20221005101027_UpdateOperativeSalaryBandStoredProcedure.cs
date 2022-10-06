using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace V1.Infrastructure.Migrations
{
    public partial class UpdateOperativeSalaryBandStoredProcedure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE OR REPLACE PROCEDURE
sp_update_operative_salary_band(
  op_id text,
  op_salary_band integer,
  bp_id text
)
LANGUAGE 'plpgsql'
AS $$
BEGIN
  UPDATE operatives SET salary_band = op_salary_band WHERE id = op_id;

  UPDATE pay_elements AS pe
  SET value = ROUND((pb.value / 36) * duration, 4)::numeric(10,4)
  FROM timesheets AS t
  JOIN weeks AS w ON t.week_id = w.id
  JOIN operatives AS o ON t.operative_id = o.id
  JOIN schemes AS s ON o.scheme_id = s.id
  JOIN pay_bands AS pb ON s.id = pb.scheme_id AND o.salary_band = pb.band
  WHERE pe.timesheet_id = t.id
  AND pe.pay_element_type_id IN (
    SELECT pet.id FROM pay_element_types AS pet
    WHERE pet.pay_at_band = true AND pet.non_productive = true
  )
  AND w.bonus_period_id = bp_id
  AND o.id = op_id;
END;
$$;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DROP PROCEDURE IF EXISTS
sp_update_operative_salary_band(
  op_id text,
  op_salary_band integer,
  bp_id text
);
            ");
        }
    }
}
