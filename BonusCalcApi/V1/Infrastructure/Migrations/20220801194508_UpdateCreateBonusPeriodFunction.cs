using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace V1.Infrastructure.Migrations
{
    public partial class UpdateCreateBonusPeriodFunction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP FUNCTION IF EXISTS create_bonus_period(text);");

            migrationBuilder.Sql(@"
CREATE OR REPLACE FUNCTION create_bonus_period(text)
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
  INSERT INTO bonus_periods (id, start_at, year, number)
  VALUES (
    $1,
    $1::timestamp AT TIME ZONE 'Europe/London' AT TIME ZONE 'UTC',
    ($1::date - '2021-02-01'::date) / 364 + 2021,
    (($1::date - '2021-02-01'::date) % 364) / 91 + 1
  );

  FOR i IN 1..13 LOOP
  	INSERT INTO weeks (id, bonus_period_id, start_at, number)
  	VALUES (
  	  ($1::date + (i - 1) * 7)::text,
  	  $1,
  	  ($1::date + (i - 1) * 7)::timestamp AT TIME ZONE 'Europe/London' AT TIME ZONE 'UTC',
  	  i
  	);
  END LOOP;

  INSERT INTO timesheets (id, operative_id, week_id, utilisation)
  SELECT
    CONCAT(o.id, '/', w.id) AS id,
    o.id AS operative_id,
    w.id AS week_id,
    o.utilisation
  FROM weeks AS w
  CROSS JOIN operatives AS o
  WHERE o.is_archived = false
  AND w.bonus_period_id = $1;

  RETURN QUERY SELECT bp.id, bp.start_at, bp.year, bp.number, bp.closed_at, bp.closed_by
  FROM bonus_periods AS bp WHERE bp.id = $1;
END;
$$;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP FUNCTION IF EXISTS create_bonus_period(text);");

            migrationBuilder.Sql(@"
CREATE OR REPLACE FUNCTION create_bonus_period(text)
RETURNS TABLE (
  id character varying(10),
  start_at timestamp with time zone,
  year integer,
  number integer,
  closed_at timestamp with time zone
)
LANGUAGE 'plpgsql'
AS $$
BEGIN
  INSERT INTO bonus_periods (id, start_at, year, number)
  VALUES (
    $1,
    $1::timestamp AT TIME ZONE 'Europe/London' AT TIME ZONE 'UTC',
    ($1::date - '2021-02-01'::date) / 364 + 2021,
    (($1::date - '2021-02-01'::date) % 364) / 91 + 1
  );

  FOR i IN 1..13 LOOP
  	INSERT INTO weeks (id, bonus_period_id, start_at, number)
  	VALUES (
  	  ($1::date + (i - 1) * 7)::text,
  	  $1,
  	  ($1::date + (i - 1) * 7)::timestamp AT TIME ZONE 'Europe/London' AT TIME ZONE 'UTC',
  	  i
  	);
  END LOOP;

  INSERT INTO timesheets (id, operative_id, week_id, utilisation)
  SELECT
    CONCAT(o.id, '/', w.id) AS id,
    o.id AS operative_id,
    w.id AS week_id,
    o.utilisation
  FROM weeks AS w
  CROSS JOIN operatives AS o
  WHERE o.is_archived = false
  AND w.bonus_period_id = $1;

  RETURN QUERY SELECT bp.id, bp.start_at, bp.year, bp.number, bp.closed_at
  FROM bonus_periods AS bp WHERE bp.id = $1;
END;
$$;
            ");
        }
    }
}
