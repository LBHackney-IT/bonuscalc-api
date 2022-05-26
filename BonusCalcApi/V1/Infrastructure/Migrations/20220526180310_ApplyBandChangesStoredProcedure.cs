using Microsoft.EntityFrameworkCore.Migrations;

namespace V1.Infrastructure.Migrations
{
    public partial class ApplyBandChangesStoredProcedure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE OR REPLACE PROCEDURE
sp_apply_band_changes(bp_id text, pe_type_id integer DEFAULT 202)
LANGUAGE 'plpgsql'
AS $$
BEGIN
  UPDATE operatives AS o
  SET salary_band = bc.final_band
  FROM band_changes AS bc
  WHERE o.id = bc.operative_id
  AND bc.bonus_period_id = bp_id
  AND o.salary_band != bc.final_band;

  INSERT INTO pay_elements (
    timesheet_id, pay_element_type_id,
    duration, value, monday
  )
  SELECT
    CONCAT(operative_id, '/', (bp_id::date + 91)::text),
    pe_type_id AS pay_element_type_id,
    balance_duration AS duration,
    balance_value AS value,
    balance_duration AS monday
  FROM band_changes
  WHERE bonus_period_id = bp_id
  AND balance_value > 0;

  UPDATE pay_elements AS pe
  SET value = ROUND((pb.value / 36) * duration, 4)::numeric(10,4)
  FROM timesheets AS t
  JOIN weeks AS w ON t.week_id = w.id
  JOIN operatives AS o ON t.operative_id = o.id
  JOIN schemes AS s ON o.scheme_id = s.id
  JOIN pay_bands AS pb ON s.id = pb.scheme_id AND o.salary_band = pb.band
  WHERE pe.timesheet_id = t.id
  AND pe.pay_element_type_id IN (
    SELECT id FROM pay_element_types WHERE pay_at_band = true AND non_productive = true
  )
  AND w.bonus_period_id = (bp_id::date + 91)::text;
END;
$$;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP PROCEDURE IF EXISTS sp_apply_band_changes(bp_id text, pe_type_id integer);");
        }
    }
}
