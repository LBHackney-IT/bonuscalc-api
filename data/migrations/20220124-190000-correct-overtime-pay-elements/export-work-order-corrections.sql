-- The \COPY command needs to be on one line so we create
-- a temporary views as our queries are complex
CREATE TEMPORARY VIEW in_hours_work_order_corrections AS

-- Dump in-hours operative SMVs based on query in:
-- https://github.com/LBHackney-IT/repairs-api-dotnet/pull/566/files#diff-68d1d2a4a5b3f28f7c80054a2b59fe0030811f60730d4f7d23662f655afc31a5
--
-- Additionally filters out 'No Access' work orders as they were not
-- affected in the Bonus Calculation database
SELECT
  wo.id AS work_order,
  wo.status_code,
  pa.address_line,
  wo.description_of_work,
  wo.closed_date,
  o.payroll_number,
  woo.job_percentage,
  SUM(rsi.quantity_amount * sc.standard_minute_value) AS total_smv
FROM work_orders AS wo
INNER JOIN work_order_operatives AS woo ON wo.id = woo.work_order_id
INNER JOIN property_class AS pc ON wo.site_id = pc.site_id
INNER JOIN property_address AS pa ON pc.address_id = pa.id
INNER JOIN work_elements AS we ON wo.id = we.work_order_id
INNER JOIN rate_schedule_item AS rsi ON we.id = rsi.work_element_id
INNER JOIN sor_codes AS sc ON rsi.custom_code = sc.code
INNER JOIN operatives AS o ON woo.operative_id = o.id
WHERE wo.id IN (
  SELECT DISTINCT(wo.id)
  FROM job_status_updates as jsu
  INNER JOIN work_orders as wo
  ON jsu.related_work_order_id = wo.id
  WHERE wo.status_code = 50 AND wo.is_overtime
  AND jsu.event_time >= '2022-01-13 18:25:47'
  AND jsu.event_time <= '2022-01-18 18:06:01'
  AND jsu.other_type = 'complete'
  AND jsu.author_email IN (SELECT DISTINCT(email) FROM operatives WHERE email IS NOT NULL AND name NOT LIKE '%(SVY)%')
  AND (
    (jsu.event_time >= '2022-01-14 08:00:00' AND jsu.event_time < '2022-01-14 16:00:00') OR
    (jsu.event_time >= '2022-01-17 08:00:00' AND jsu.event_time < '2022-01-17 16:00:00') OR
    (jsu.event_time >= '2022-01-18 08:00:00' AND jsu.event_time < '2022-01-18 16:00:00')
  )
)
GROUP BY wo.id, wo.status_code, pa.address_line, wo.description_of_work, wo.closed_date, o.payroll_number, woo.job_percentage
ORDER BY wo.id;

-- Dump out-of-hours operative SMVs based on query in:
-- https://github.com/LBHackney-IT/repairs-api-dotnet/pull/566/files#diff-fdfd01aeb18d203fd4cf7dbf47ebeb64645ecc672095095858472a47715738d9
--
-- Additionally filters out 'No Access' work orders as they were not
-- affected in the Bonus Calculation database
CREATE TEMPORARY VIEW out_of_hours_work_order_corrections AS
SELECT
  wo.id AS work_order,
  wo.status_code,
  pa.address_line,
  wo.description_of_work,
  wo.closed_date,
  o.payroll_number,
  woo.job_percentage,
  SUM(rsi.quantity_amount * sc.standard_minute_value) AS total_smv
FROM work_orders AS wo
INNER JOIN work_order_operatives AS woo ON wo.id = woo.work_order_id
INNER JOIN property_class AS pc ON wo.site_id = pc.site_id
INNER JOIN property_address AS pa ON pc.address_id = pa.id
INNER JOIN work_elements AS we ON wo.id = we.work_order_id
INNER JOIN rate_schedule_item AS rsi ON we.id = rsi.work_element_id
INNER JOIN sor_codes AS sc ON rsi.custom_code = sc.code
INNER JOIN operatives AS o ON woo.operative_id = o.id
WHERE wo.id IN (
  SELECT DISTINCT(wo.id)
  FROM temp_overtime_operative_payment_types_13_jan_18_jan AS tmp
  INNER JOIN work_order_operatives AS woo ON woo.operative_id = tmp.operative_id
  INNER JOIN work_orders AS wo ON wo.id = woo.work_order_id
  INNER JOIN job_status_updates AS jsu ON jsu.related_work_order_id = wo.id
  WHERE wo.status_code = 50 AND wo.is_overtime
  AND jsu.author_email IN (SELECT DISTINCT(email) FROM operatives WHERE email IS NOT NULL AND name NOT LIKE '%(SVY)%')
  AND jsu.other_type = 'complete'
  AND (
    (tmp.thursday_13_jan_pm_payment_type = 'bonus' AND wo.closed_date BETWEEN '2022-01-13 18:25:47' AND '2022-01-13 23:59:59') OR
    (tmp.friday_14_jan_am_payment_type = 'bonus' AND wo.closed_date BETWEEN '2022-01-14 00:00:00' AND '2022-01-14 08:00:00') OR
    (tmp.friday_14_jan_pm_payment_type = 'bonus' AND wo.closed_date BETWEEN '2022-01-14 16:00:00' AND '2022-01-14 23:59:59') OR
    (tmp.weekend_15_16_jan_payment_type = 'bonus' AND wo.closed_date BETWEEN '2022-01-15 00:00:00' AND '2022-01-16 23:59:59') OR
    (tmp.monday_17_jan_am_payment_type = 'bonus' AND wo.closed_date BETWEEN '2022-01-17 00:00:00' AND '2022-01-17 08:00:00') OR
    (tmp.monday_17_jan_pm_payment_type = 'bonus' AND wo.closed_date BETWEEN '2022-01-17 16:00:00' AND '2022-01-17 23:59:59') OR
    (tmp.tuesday_18_jan_am_payment_type = 'bonus' AND wo.closed_date BETWEEN '2022-01-18 00:00:00' AND '2022-01-18 08:00:00') OR
    (tmp.tuesday_18_jan_pm_payment_type = 'bonus' AND wo.closed_date BETWEEN '2022-01-18 16:00:00' AND '2022-01-18 19:06:01')
  )
  AND wo.id NOT IN (
    10051370, 10051518, 10052203, 10056280, 10056478,
    10056289, 10056327, 10055031, 10055064, 10055069
  )
)
GROUP BY wo.id, wo.status_code, pa.address_line, wo.description_of_work, wo.closed_date, o.payroll_number, woo.job_percentage
ORDER BY wo.id;

-- Export query results to a CSV file
\COPY (SELECT * FROM in_hours_work_order_corrections) TO 'in_hours_work_order_corrections.csv' CSV HEADER;
\COPY (SELECT * FROM out_of_hours_work_order_corrections) TO 'out_of_hours_work_order_corrections.csv' CSV HEADER;

-- We created the views as temporary so PostgreSQL will clean
-- it up anyway but we'll drop them here for the sake of completeness
DROP VIEW in_hours_work_order_corrections;
DROP VIEW out_of_hours_work_order_corrections;
