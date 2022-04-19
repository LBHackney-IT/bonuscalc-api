using Microsoft.EntityFrameworkCore.Migrations;

namespace V1.Infrastructure.Migrations
{
    public partial class CreateBonusPeriodStoredProcedure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE OR REPLACE PROCEDURE
sp_create_bonus_period(bp_id text)
LANGUAGE 'plpgsql'
AS $$
BEGIN
  INSERT INTO bonus_periods (id, start_at, year, number)
  VALUES (
    bp_id,
    bp_id::timestamp AT TIME ZONE 'Europe/London' AT TIME ZONE 'UTC',
    (bp_id::date - '2021-02-01'::date) / 364 + 2021,
    ((bp_id::date - '2021-02-01'::date) % 364) / 91 + 1
  );

  FOR i IN 1..13 LOOP
  	INSERT INTO weeks (id, bonus_period_id, start_at, number)
  	VALUES (
  	  (bp_id::date + (i - 1) * 7)::text,
  	  bp_id,
  	  (bp_id::date + (i - 1) * 7)::timestamp AT TIME ZONE 'Europe/London' AT TIME ZONE 'UTC',
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
  AND w.bonus_period_id = bp_id;
END;
$$;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP PROCEDURE IF EXISTS sp_create_bonus_period(bp_id text);");
        }
    }
}
