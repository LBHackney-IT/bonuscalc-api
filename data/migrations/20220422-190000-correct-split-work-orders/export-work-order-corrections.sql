-- The \COPY command needs to be on one line so we create
-- a temporary views as our queries are complex
CREATE TEMPORARY VIEW work_order_corrections AS

-- Dump affected work orders based on those having more than one work_order_operatives record:
SELECT
  wo.id AS work_order,
  wo.status_code,
  p.contractor_reference,
  pa.address_line,
  wo.description_of_work,
  wo.closed_date,
  o.payroll_number,
  woo.job_percentage,
  SUM(ROUND((rsi.quantity_amount * sc.standard_minute_value)::numeric, 2)) AS total_smv,
  SUM(ROUND((rsi.quantity_amount * sc.operative_cost)::numeric, 2)) AS operative_cost,
  bool_or(wo.is_overtime OR (wo.payment_type IS NOT NULL AND wo.payment_type = 1)) AS is_overtime,
  bool_or(sc.is_outofhours) AS is_outofhours
FROM work_orders AS wo
INNER JOIN party AS p ON wo.assigned_to_primary_id = p.id
INNER JOIN work_order_operatives AS woo ON wo.id = woo.work_order_id
INNER JOIN property_class AS pc ON wo.site_id = pc.site_id
INNER JOIN property_address AS pa ON pc.address_id = pa.id
INNER JOIN work_elements AS we ON wo.id = we.work_order_id
INNER JOIN rate_schedule_item AS rsi ON we.id = rsi.work_element_id
INNER JOIN sor_codes AS sc ON rsi.custom_code = sc.code
INNER JOIN operatives AS o ON woo.operative_id = o.id
WHERE wo.id IN (
  SELECT t1.work_order_id
  FROM work_order_operatives AS t1
  INNER JOIN work_orders AS t2 ON t1.work_order_id = t2.id
  WHERE t2.closed_date >= '2022-01-31'
  AND t2.closed_date < '2022-04-13'
  AND t2.status_code IN (50, 1000)
  GROUP BY t1.work_order_id
  HAVING COUNT(*) > 1
)
AND o.payroll_number ~ '^\d{6}$' AND o.name !~ '\(SVY\)'
GROUP BY wo.id, wo.status_code, p.contractor_reference, pa.address_line, wo.description_of_work, wo.closed_date, o.payroll_number, woo.job_percentage
ORDER BY wo.id;

-- Export query results to a CSV file
\COPY (SELECT * FROM work_order_corrections) TO 'work_order_corrections.csv' CSV HEADER;

-- We created the view as temporary so PostgreSQL will clean
-- it up anyway but we'll drop it here for the sake of completeness
DROP VIEW work_order_corrections;
