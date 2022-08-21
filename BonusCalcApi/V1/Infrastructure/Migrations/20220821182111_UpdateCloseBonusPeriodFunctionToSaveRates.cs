using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace V1.Infrastructure.Migrations
{
    public partial class UpdateCloseBonusPeriodFunctionToSaveRates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE OR REPLACE FUNCTION close_bonus_period(text, integer, timestamp with time zone, text)
RETURNS TABLE (
  id character varying(10),
  start_at timestamp with time zone,
  year integer,
  number integer,
  closed_at timestamp with time zone,
  closed_by character varying(100)
)
LANGUAGE 'plpgsql'
AS $$
BEGIN
  UPDATE band_changes AS bc
  SET bonus_rate = br.rates[bc.final_band]
  FROM bonus_rates AS br
  WHERE bc.rate_code = br.id
  AND bc.bonus_period_id = $1;

  UPDATE operatives AS o
  SET salary_band = bc.final_band
  FROM band_changes AS bc
  WHERE o.id = bc.operative_id
  AND bc.bonus_period_id = $1
  AND o.salary_band != bc.final_band;

  DELETE FROM pay_elements AS pe
  WHERE pe.pay_element_type_id = $2
  AND pe.timesheet_id IN (
    SELECT
    CONCAT(o.id, '/', ($1::date + 91)::text)
    FROM operatives AS o
  );

  INSERT INTO pay_elements (
    timesheet_id, pay_element_type_id,
    duration, value, monday
  )
  SELECT
    CONCAT(operative_id, '/', ($1::date + 91)::text),
    $2 AS pay_element_type_id,
    balance_duration AS duration,
    balance_value AS value,
    balance_duration AS monday
  FROM band_changes
  WHERE bonus_period_id = $1
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
    SELECT pet.id FROM pay_element_types AS pet
    WHERE pet.pay_at_band = true AND pet.non_productive = true
  )
  AND w.bonus_period_id = ($1::date + 91)::text;

  UPDATE bonus_periods AS bp
  SET closed_at = $3, closed_by = $4
  WHERE bp.id = $1;

  RETURN QUERY SELECT bp.id, bp.start_at, bp.year, bp.number, bp.closed_at, bp.closed_by
  FROM bonus_periods AS bp WHERE bp.id = $1;
END;
$$;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE OR REPLACE FUNCTION close_bonus_period(text, integer, timestamp with time zone, text)
RETURNS TABLE (
  id character varying(10),
  start_at timestamp with time zone,
  year integer,
  number integer,
  closed_at timestamp with time zone,
  closed_by character varying(100)
)
LANGUAGE 'plpgsql'
AS $$
BEGIN
  UPDATE operatives AS o
  SET salary_band = bc.final_band
  FROM band_changes AS bc
  WHERE o.id = bc.operative_id
  AND bc.bonus_period_id = $1
  AND o.salary_band != bc.final_band;

  DELETE FROM pay_elements AS pe
  WHERE pe.pay_element_type_id = $2
  AND pe.timesheet_id IN (
    SELECT
    CONCAT(o.id, '/', ($1::date + 91)::text)
    FROM operatives AS o
  );

  INSERT INTO pay_elements (
    timesheet_id, pay_element_type_id,
    duration, value, monday
  )
  SELECT
    CONCAT(operative_id, '/', ($1::date + 91)::text),
    $2 AS pay_element_type_id,
    balance_duration AS duration,
    balance_value AS value,
    balance_duration AS monday
  FROM band_changes
  WHERE bonus_period_id = $1
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
    SELECT pet.id FROM pay_element_types AS pet
    WHERE pet.pay_at_band = true AND pet.non_productive = true
  )
  AND w.bonus_period_id = ($1::date + 91)::text;

  UPDATE bonus_periods AS bp
  SET closed_at = $3, closed_by = $4
  WHERE bp.id = $1;

  RETURN QUERY SELECT bp.id, bp.start_at, bp.year, bp.number, bp.closed_at, bp.closed_by
  FROM bonus_periods AS bp WHERE bp.id = $1;
END;
$$;
            ");
        }
    }
}
