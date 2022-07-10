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

-- All timestamps are recorded as UTC
SET TimeZone='UTC';

-- Wrap in a transaction so that if it fails the database is still in a consistent state
BEGIN;

-- Remove any existing data
TRUNCATE TABLE
  band_changes,
  bonus_periods,
  operatives,
  pay_bands,
  pay_element_types,
  pay_elements,
  people,
  schemes,
  timesheets,
  trades,
  weeks
RESTART IDENTITY;

-- Generate current bonus period:
INSERT INTO bonus_periods (id, start_at, year, number)
VALUES (
  (SELECT CURRENT_DATE - (CURRENT_DATE - '2021-11-01') % 91),
  (SELECT TO_TIMESTAMP(TO_CHAR(CURRENT_DATE - (CURRENT_DATE - '2021-11-01') % 91, 'YYYY-MM-DD'), 'YYYY-MM-DD') AT TIME ZONE 'UTC'),
  (SELECT DATE_PART('year', CURRENT_DATE - (CURRENT_DATE - '2021-11-01') % 91)),
  (SELECT (4 + ((CURRENT_DATE - '2021-11-01') / 91)) % 4)
);

-- Generate weeks
INSERT INTO weeks (id, bonus_period_id, start_at, number)
VALUES (
  (SELECT CURRENT_DATE - (CURRENT_DATE - '2021-11-01') % 91),
  (SELECT CURRENT_DATE - (CURRENT_DATE - '2021-11-01') % 91),
  (SELECT TO_TIMESTAMP(TO_CHAR(CURRENT_DATE - (CURRENT_DATE - '2021-11-01') % 91, 'YYYY-MM-DD'), 'YYYY-MM-DD') AT TIME ZONE 'UTC'), 1
),
(
  (SELECT CURRENT_DATE - (CURRENT_DATE - '2021-11-01') % 91 + 7),
  (SELECT CURRENT_DATE - (CURRENT_DATE - '2021-11-01') % 91),
  (SELECT TO_TIMESTAMP(TO_CHAR(CURRENT_DATE - (CURRENT_DATE - '2021-11-01') % 91 + 7, 'YYYY-MM-DD'), 'YYYY-MM-DD') AT TIME ZONE 'UTC'), 2
),
(
  (SELECT CURRENT_DATE - (CURRENT_DATE - '2021-11-01') % 91 + 14),
  (SELECT CURRENT_DATE - (CURRENT_DATE - '2021-11-01') % 91),
  (SELECT TO_TIMESTAMP(TO_CHAR(CURRENT_DATE - (CURRENT_DATE - '2021-11-01') % 91 + 14, 'YYYY-MM-DD'), 'YYYY-MM-DD') AT TIME ZONE 'UTC'), 3
),
(
  (SELECT CURRENT_DATE - (CURRENT_DATE - '2021-11-01') % 91 + 21),
  (SELECT CURRENT_DATE - (CURRENT_DATE - '2021-11-01') % 91),
  (SELECT TO_TIMESTAMP(TO_CHAR(CURRENT_DATE - (CURRENT_DATE - '2021-11-01') % 91 + 21, 'YYYY-MM-DD'), 'YYYY-MM-DD') AT TIME ZONE 'UTC'), 4
),
(
  (SELECT CURRENT_DATE - (CURRENT_DATE - '2021-11-01') % 91 + 28),
  (SELECT CURRENT_DATE - (CURRENT_DATE - '2021-11-01') % 91),
  (SELECT TO_TIMESTAMP(TO_CHAR(CURRENT_DATE - (CURRENT_DATE - '2021-11-01') % 91 + 28, 'YYYY-MM-DD'), 'YYYY-MM-DD') AT TIME ZONE 'UTC'), 5
),
(
  (SELECT CURRENT_DATE - (CURRENT_DATE - '2021-11-01') % 91 + 35),
  (SELECT CURRENT_DATE - (CURRENT_DATE - '2021-11-01') % 91),
  (SELECT TO_TIMESTAMP(TO_CHAR(CURRENT_DATE - (CURRENT_DATE - '2021-11-01') % 91 + 35, 'YYYY-MM-DD'), 'YYYY-MM-DD') AT TIME ZONE 'UTC'), 6
),
(
  (SELECT CURRENT_DATE - (CURRENT_DATE - '2021-11-01') % 91 + 42),
  (SELECT CURRENT_DATE - (CURRENT_DATE - '2021-11-01') % 91),
  (SELECT TO_TIMESTAMP(TO_CHAR(CURRENT_DATE - (CURRENT_DATE - '2021-11-01') % 91 + 42, 'YYYY-MM-DD'), 'YYYY-MM-DD') AT TIME ZONE 'UTC'), 7
),
(
  (SELECT CURRENT_DATE - (CURRENT_DATE - '2021-11-01') % 91 + 49),
  (SELECT CURRENT_DATE - (CURRENT_DATE - '2021-11-01') % 91),
  (SELECT TO_TIMESTAMP(TO_CHAR(CURRENT_DATE - (CURRENT_DATE - '2021-11-01') % 91 + 49, 'YYYY-MM-DD'), 'YYYY-MM-DD') AT TIME ZONE 'UTC'), 8
),
(
  (SELECT CURRENT_DATE - (CURRENT_DATE - '2021-11-01') % 91 + 56),
  (SELECT CURRENT_DATE - (CURRENT_DATE - '2021-11-01') % 91),
  (SELECT TO_TIMESTAMP(TO_CHAR(CURRENT_DATE - (CURRENT_DATE - '2021-11-01') % 91 + 56, 'YYYY-MM-DD'), 'YYYY-MM-DD') AT TIME ZONE 'UTC'), 9
),
(
  (SELECT CURRENT_DATE - (CURRENT_DATE - '2021-11-01') % 91 + 63),
  (SELECT CURRENT_DATE - (CURRENT_DATE - '2021-11-01') % 91),
  (SELECT TO_TIMESTAMP(TO_CHAR(CURRENT_DATE - (CURRENT_DATE - '2021-11-01') % 91 + 63, 'YYYY-MM-DD'), 'YYYY-MM-DD') AT TIME ZONE 'UTC'), 10
),
(
  (SELECT CURRENT_DATE - (CURRENT_DATE - '2021-11-01') % 91 + 70),
  (SELECT CURRENT_DATE - (CURRENT_DATE - '2021-11-01') % 91),
  (SELECT TO_TIMESTAMP(TO_CHAR(CURRENT_DATE - (CURRENT_DATE - '2021-11-01') % 91 + 70, 'YYYY-MM-DD'), 'YYYY-MM-DD') AT TIME ZONE 'UTC'), 11
),
(
  (SELECT CURRENT_DATE - (CURRENT_DATE - '2021-11-01') % 91 + 77),
  (SELECT CURRENT_DATE - (CURRENT_DATE - '2021-11-01') % 91),
  (SELECT TO_TIMESTAMP(TO_CHAR(CURRENT_DATE - (CURRENT_DATE - '2021-11-01') % 91 + 77, 'YYYY-MM-DD'), 'YYYY-MM-DD') AT TIME ZONE 'UTC'), 12
),
(
  (SELECT CURRENT_DATE - (CURRENT_DATE - '2021-11-01') % 91 + 84),
  (SELECT CURRENT_DATE - (CURRENT_DATE - '2021-11-01') % 91),
  (SELECT TO_TIMESTAMP(TO_CHAR(CURRENT_DATE - (CURRENT_DATE - '2021-11-01') % 91 + 84, 'YYYY-MM-DD'), 'YYYY-MM-DD') AT TIME ZONE 'UTC'), 13
);

-- Import schemes
\COPY schemes(id, type, description, conversion_factor, max_value) FROM 'schemes.csv' CSV HEADER;

-- Import pay bands
\COPY pay_bands(id, scheme_id, band, value) FROM 'pay_bands.csv' CSV HEADER;

-- Import trades
\COPY trades(id, description) FROM 'trades.csv' CSV HEADER;

-- Import pay element types
\COPY pay_element_types(id, description, pay_at_band, paid, adjustment, productive, non_productive, out_of_hours, overtime, selectable, smv_per_hour, sick_leave, cost_code) FROM 'pay_element_types.csv' CSV HEADER;

-- Import people
\COPY people(id, name, email_address) FROM 'people.csv' CSV HEADER;

-- Import operatives
\COPY operatives(id, name, trade_id, section, scheme_id, salary_band, fixed_band, utilisation, is_archived, manager_id, supervisor_id) FROM 'operatives.csv' CSV HEADER;

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
