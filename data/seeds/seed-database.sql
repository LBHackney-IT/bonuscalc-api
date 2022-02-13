-- Connect to the database using the psql client:
--
--   $ psql postgresql://localhost/bonuscalc
--
-- Use the correct url for your database and then run this file:
--
--   bonuscalc=# \i seed-database.sql
--
-- The API can then be started using `dotnet run`

-- Disable output for the current database check
\o /dev/null

-- Get the current database name and store it locally in the client
SELECT current_database() = 'bonuscalc-api-db-production' AS is_production;
\gset

-- Re-enable output for the rest of the script
\o

-- Bail if the current database matches the production database name
\if :is_production
  \warn 'You can''t run this script on the production database'
  \quit
\endif

-- Wrap in a transaction so that if it fails the database is still in a consistent state
BEGIN;

-- Remove any existing data
TRUNCATE TABLE
  bonus_periods,
  operatives,
  pay_bands,
  pay_element_types,
  pay_elements,
  schemes,
  timesheets,
  trades,
  weeks
RESTART IDENTITY;

-- Import bonus periods
\COPY bonus_periods(id, start_at, year, number) FROM 'bonus_periods.csv' CSV HEADER;

-- Import weeks
\COPY weeks(id, bonus_period_id, start_at, number) FROM 'weeks.csv' CSV HEADER;

-- Import schemes
\COPY schemes(id, type, description, conversion_factor) FROM 'schemes.csv' CSV HEADER;

-- Import pay bands
\COPY pay_bands(id, scheme_id, band, value) FROM 'pay_bands.csv' CSV HEADER;

-- Import trades
\COPY trades(id, description) FROM 'trades.csv' CSV HEADER;

-- Import pay element types
\COPY pay_element_types(id, description, pay_at_band, paid, adjustment, productive, non_productive, out_of_hours, overtime, selectable, smv_per_hour, sick_leave) FROM 'pay_element_types.csv' CSV HEADER;

-- Import operatives
\COPY operatives(id, name, trade_id, section, scheme_id, salary_band, fixed_band, utilisation, is_archived) FROM 'operatives.csv' CSV HEADER;

-- Generate timesheets for the period
INSERT INTO timesheets (id, operative_id, week_id, utilisation)
SELECT
  CONCAT(o.id, '/', w.id) AS id,
  o.id AS operative_id,
  w.id AS week_id,
  o.utilisation
FROM weeks AS w
CROSS JOIN operatives AS o;

COMMIT;
