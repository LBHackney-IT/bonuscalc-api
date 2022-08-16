using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace V1.Infrastructure.Migrations
{
    public partial class UpdateCreateOperativeStoredProcedure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE OR REPLACE PROCEDURE
sp_create_operative(
  op_id text,
  op_name text,
  op_trade text,
  op_section text,
  op_email_address text DEFAULT NULL,
  op_scheme text DEFAULT 'Reactive',
  op_utilisation numeric DEFAULT 1.0,
  op_salary_band integer DEFAULT 3,
  op_fixed_band boolean DEFAULT false,
  op_manager text DEFAULT NULL,
  op_supervisor text DEFAULT NULL,
  op_is_archived boolean DEFAULT false
)
LANGUAGE 'plpgsql'
AS $$
BEGIN
  INSERT INTO operatives (
    id, name, trade_id, section, salary_band, fixed_band,
    scheme_id, email_address, utilisation, is_archived,
    manager_id, supervisor_id
  ) VALUES (
    op_id,
    op_name,
    op_trade,
    op_section,
    op_salary_band,
    op_fixed_band,
    (
      SELECT id FROM schemes
      WHERE description = op_scheme
      LIMIT 1
    ),
    op_email_address,
    op_utilisation,
    op_is_archived,
    op_manager,
    op_supervisor
  );

  INSERT INTO timesheets (id, operative_id, week_id, utilisation)
  SELECT
    CONCAT(o.id, '/', w.id) AS id,
    o.id AS operative_id,
    w.id AS week_id,
    o.utilisation
  FROM weeks AS w
  CROSS JOIN operatives AS o
  WHERE o.id = op_id
  AND w.bonus_period_id >= (CURRENT_DATE - (CURRENT_DATE - '2021-02-01') % 91)::text;
END;
$$;
            ");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE OR REPLACE PROCEDURE
sp_create_operative(
  op_id text,
  op_name text,
  op_trade text,
  op_section text,
  op_email_address text DEFAULT NULL,
  op_scheme text DEFAULT 'Reactive',
  op_utilisation numeric DEFAULT 1.0,
  op_salary_band integer DEFAULT 3,
  op_fixed_band boolean DEFAULT false,
  op_manager text DEFAULT NULL,
  op_supervisor text DEFAULT NULL,
  op_is_archived boolean DEFAULT false
)
LANGUAGE 'plpgsql'
AS $$
BEGIN
  INSERT INTO operatives (
    id, name, trade_id, section, salary_band, fixed_band,
    scheme_id, email_address, utilisation, is_archived,
    manager_id, supervisor_id
  ) VALUES (
    op_id,
    op_name,
    op_trade,
    op_section,
    op_salary_band,
    op_fixed_band,
    (
      SELECT id FROM schemes
      WHERE description = op_scheme
      LIMIT 1
    ),
    op_email_address,
    op_utilisation,
    op_is_archived,
    op_manager,
    op_supervisor
  );

  INSERT INTO timesheets (id, operative_id, week_id, utilisation)
  SELECT
    CONCAT(o.id, '/', w.id) AS id,
    o.id AS operative_id,
    w.id AS week_id,
    o.utilisation
  FROM weeks AS w
  CROSS JOIN operatives AS o
  WHERE o.id = op_id
  AND w.bonus_period_id = (CURRENT_DATE - (CURRENT_DATE - '2021-02-01') % 91)::text;
END;
$$;
            ");
        }
    }
}
