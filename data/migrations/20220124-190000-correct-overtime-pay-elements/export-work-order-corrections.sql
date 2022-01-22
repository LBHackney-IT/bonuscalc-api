-- The \COPY command needs to be on one line so we create
-- a temporary view as our query is complex
CREATE TEMPORARY VIEW work_order_corrections AS

-- Dump operative SMVs based on query in:
-- https://github.com/LBHackney-IT/repairs-api-dotnet/pull/566
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
  WHERE wo.status_code = 50
  AND jsu.event_time >= '2022-01-13 18:25:47'
  AND jsu.event_time <= '2022-01-18 18:06:01'
  AND jsu.other_type = 'complete'
  AND jsu.author_email IN (SELECT DISTINCT(email) FROM operatives WHERE email IS NOT NULL)
  AND (
    (jsu.event_time >= '2022-01-14 08:00:00' AND jsu.event_time < '2022-01-14 16:00:00') OR
    (jsu.event_time >= '2022-01-17 08:00:00' AND jsu.event_time < '2022-01-17 16:00:00') OR
    (jsu.event_time >= '2022-01-18 08:00:00' AND jsu.event_time < '2022-01-18 16:00:00')
  )
)
GROUP BY wo.id, wo.status_code, pa.address_line, wo.description_of_work, wo.closed_date, o.payroll_number, woo.job_percentage
ORDER BY wo.id;

-- Export query results to a CSV file
\COPY (SELECT * FROM work_order_corrections) TO 'work_order_corrections.csv' CSV HEADER;

-- We created the view as temporary so PostgreSQL will clean
-- it up anyway but we'll drop it here for the sake of completeness
DROP VIEW work_order_corrections;
