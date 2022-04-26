-- Create temporary table to hold records dumped from Repairs Hub database
CREATE TEMPORARY TABLE work_order_corrections (
  work_order varchar(10) NOT NULL,
  status_code integer NOT NULL,
  contractor_reference varchar(3),
  address_line text,
  description_of_work text,
  closed_date timestamp without time zone NOT NULL,
  payroll_number varchar(6) NOT NULL,
  job_percentage numeric NOT NULL,
  total_smv numeric NOT NULL,
  operative_cost numeric NOT NULL,
  is_overtime boolean NOT NULL,
  is_outofhours boolean NOT NULL
);

-- Import work order corrections
\COPY work_order_corrections(work_order, status_code, contractor_reference, address_line, description_of_work, closed_date, payroll_number, job_percentage, total_smv, operative_cost, is_overtime, is_outofhours) FROM 'work_order_corrections.csv' CSV HEADER;

-- Wrap the changes in a transaction to ensure they're applied as a whole
BEGIN;

-- Remove the existing overtime pay elements as not all were created
DELETE FROM pay_elements
WHERE work_order IN (
  SELECT DISTINCT(work_order) FROM work_order_corrections
);

-- Insert new productive pay elements that were completed
INSERT INTO pay_elements (
  timesheet_id, pay_element_type_id, work_order, cost_code, address,
  comment, closed_at, value, duration, monday, tuesday, wednesday,
  thursday, friday, saturday, sunday, read_only
)
SELECT
  CONCAT(payroll_number, '/', TO_CHAR(DATE_TRUNC('week', closed_date), 'YYYY-MM-DD')) AS timesheet_id,
  301 AS pay_element_type_id,
  work_order,
  CASE
  WHEN contractor_reference = 'H01' THEN 'H3009'
  WHEN contractor_reference = 'H02' THEN 'H3007'
  WHEN contractor_reference = 'H03' THEN 'H3015'
  WHEN contractor_reference = 'H04' THEN 'H3002'
  WHEN contractor_reference = 'H05' THEN 'H3002'
  WHEN contractor_reference = 'H06' THEN 'H3003'
  WHEN contractor_reference = 'H07' THEN 'H3010'
  WHEN contractor_reference = 'H08' THEN 'H3002'
  WHEN contractor_reference = 'H09' THEN 'H3005'
  WHEN contractor_reference = 'H10' THEN 'H3010'
  WHEN contractor_reference = 'H11' THEN 'H3016'
  WHEN contractor_reference = 'H12' THEN 'H3004'
  WHEN contractor_reference = 'H13' THEN 'H3008'
  WHEN contractor_reference = 'H14' THEN 'H1040'
  WHEN contractor_reference = 'H15' THEN 'H1039'
  END AS cost_code,
  address_line AS address,
  description_of_work AS comment,
  closed_date AS closed_at,
  ROUND(total_smv * job_percentage / 100, 4) AS value,
  ROUND((total_smv * job_percentage / 100) / 60, 4) AS duration,
  CASE EXTRACT(isodow FROM closed_date) WHEN 1 THEN ROUND((total_smv * job_percentage / 100) / 60, 4) ELSE 0 END AS monday,
  CASE EXTRACT(isodow FROM closed_date) WHEN 2 THEN ROUND((total_smv * job_percentage / 100) / 60, 4) ELSE 0 END AS tuesday,
  CASE EXTRACT(isodow FROM closed_date) WHEN 3 THEN ROUND((total_smv * job_percentage / 100) / 60, 4) ELSE 0 END AS wednesday,
  CASE EXTRACT(isodow FROM closed_date) WHEN 4 THEN ROUND((total_smv * job_percentage / 100) / 60, 4) ELSE 0 END AS thursday,
  CASE EXTRACT(isodow FROM closed_date) WHEN 5 THEN ROUND((total_smv * job_percentage / 100) / 60, 4) ELSE 0 END AS friday,
  CASE EXTRACT(isodow FROM closed_date) WHEN 6 THEN ROUND((total_smv * job_percentage / 100) / 60, 4) ELSE 0 END AS saturday,
  CASE EXTRACT(isodow FROM closed_date) WHEN 7 THEN ROUND((total_smv * job_percentage / 100) / 60, 4) ELSE 0 END AS sunday,
  true AS read_only
FROM work_order_corrections
WHERE status_code = 50 AND is_outofhours = false AND is_overtime = false;

-- Insert new productive pay elements that were closed as 'No Access'
INSERT INTO pay_elements (
  timesheet_id, pay_element_type_id, work_order, cost_code, address,
  comment, closed_at, value, duration, monday, tuesday, wednesday,
  thursday, friday, saturday, sunday, read_only
)
SELECT
  CONCAT(payroll_number, '/', TO_CHAR(DATE_TRUNC('week', closed_date), 'YYYY-MM-DD')) AS timesheet_id,
  301 AS pay_element_type_id,
  work_order,
  CASE
  WHEN contractor_reference = 'H01' THEN 'H3009'
  WHEN contractor_reference = 'H02' THEN 'H3007'
  WHEN contractor_reference = 'H03' THEN 'H3015'
  WHEN contractor_reference = 'H04' THEN 'H3002'
  WHEN contractor_reference = 'H05' THEN 'H3002'
  WHEN contractor_reference = 'H06' THEN 'H3003'
  WHEN contractor_reference = 'H07' THEN 'H3010'
  WHEN contractor_reference = 'H08' THEN 'H3002'
  WHEN contractor_reference = 'H09' THEN 'H3005'
  WHEN contractor_reference = 'H10' THEN 'H3010'
  WHEN contractor_reference = 'H11' THEN 'H3016'
  WHEN contractor_reference = 'H12' THEN 'H3004'
  WHEN contractor_reference = 'H13' THEN 'H3008'
  WHEN contractor_reference = 'H14' THEN 'H1040'
  WHEN contractor_reference = 'H15' THEN 'H1039'
  END AS cost_code,
  address_line AS address,
  description_of_work AS comment,
  closed_date AS closed_at,
  0 AS value,
  0 AS duration,
  0 AS monday,
  0 AS tuesday,
  0 AS wednesday,
  0 AS thursday,
  0 AS friday,
  0 AS saturday,
  0 AS sunday,
  true AS read_only
FROM work_order_corrections
WHERE status_code = 1000 AND is_outofhours = false AND is_overtime = false;

-- Insert new overtime pay elements that were completed
INSERT INTO pay_elements (
  timesheet_id, pay_element_type_id, work_order, cost_code, address,
  comment, closed_at, value, duration, monday, tuesday, wednesday,
  thursday, friday, saturday, sunday, read_only
)
SELECT
  CONCAT(payroll_number, '/', TO_CHAR(DATE_TRUNC('week', closed_date), 'YYYY-MM-DD')) AS timesheet_id,
  401 AS pay_element_type_id,
  work_order,
  CASE
  WHEN contractor_reference = 'H01' THEN 'H3009'
  WHEN contractor_reference = 'H02' THEN 'H3007'
  WHEN contractor_reference = 'H03' THEN 'H3015'
  WHEN contractor_reference = 'H04' THEN 'H3002'
  WHEN contractor_reference = 'H05' THEN 'H3002'
  WHEN contractor_reference = 'H06' THEN 'H3003'
  WHEN contractor_reference = 'H07' THEN 'H3010'
  WHEN contractor_reference = 'H08' THEN 'H3002'
  WHEN contractor_reference = 'H09' THEN 'H3005'
  WHEN contractor_reference = 'H10' THEN 'H3010'
  WHEN contractor_reference = 'H11' THEN 'H3016'
  WHEN contractor_reference = 'H12' THEN 'H3004'
  WHEN contractor_reference = 'H13' THEN 'H3008'
  WHEN contractor_reference = 'H14' THEN 'H1040'
  WHEN contractor_reference = 'H15' THEN 'H1039'
  END AS cost_code,
  address_line AS address,
  description_of_work AS comment,
  closed_date AS closed_at,
  21.6 AS value,
  0 AS duration,
  0 AS monday,
  0 AS tuesday,
  0 AS wednesday,
  0 AS thursday,
  0 AS friday,
  0 AS saturday,
  0 AS sunday,
  true AS read_only
FROM work_order_corrections
WHERE status_code = 50 AND is_overtime = true;

-- Insert new overtime pay elements that were closed as 'No Access'
INSERT INTO pay_elements (
  timesheet_id, pay_element_type_id, work_order, cost_code, address,
  comment, closed_at, value, duration, monday, tuesday, wednesday,
  thursday, friday, saturday, sunday, read_only
)
SELECT
  CONCAT(payroll_number, '/', TO_CHAR(DATE_TRUNC('week', closed_date), 'YYYY-MM-DD')) AS timesheet_id,
  401 AS pay_element_type_id,
  work_order,
  CASE
  WHEN contractor_reference = 'H01' THEN 'H3009'
  WHEN contractor_reference = 'H02' THEN 'H3007'
  WHEN contractor_reference = 'H03' THEN 'H3015'
  WHEN contractor_reference = 'H04' THEN 'H3002'
  WHEN contractor_reference = 'H05' THEN 'H3002'
  WHEN contractor_reference = 'H06' THEN 'H3003'
  WHEN contractor_reference = 'H07' THEN 'H3010'
  WHEN contractor_reference = 'H08' THEN 'H3002'
  WHEN contractor_reference = 'H09' THEN 'H3005'
  WHEN contractor_reference = 'H10' THEN 'H3010'
  WHEN contractor_reference = 'H11' THEN 'H3016'
  WHEN contractor_reference = 'H12' THEN 'H3004'
  WHEN contractor_reference = 'H13' THEN 'H3008'
  WHEN contractor_reference = 'H14' THEN 'H1040'
  WHEN contractor_reference = 'H15' THEN 'H1039'
  END AS cost_code,
  address_line AS address,
  description_of_work AS comment,
  closed_date AS closed_at,
  0 AS value,
  0 AS duration,
  0 AS monday,
  0 AS tuesday,
  0 AS wednesday,
  0 AS thursday,
  0 AS friday,
  0 AS saturday,
  0 AS sunday,
  true AS read_only
FROM work_order_corrections
WHERE status_code = 1000 AND is_overtime = true;

-- Insert new out-of-hours pay elements that were completed
INSERT INTO pay_elements (
  timesheet_id, pay_element_type_id, work_order, cost_code, address,
  comment, closed_at, value, duration, monday, tuesday, wednesday,
  thursday, friday, saturday, sunday, read_only
)
SELECT
  CONCAT(payroll_number, '/', TO_CHAR(DATE_TRUNC('week', closed_date), 'YYYY-MM-DD')) AS timesheet_id,
  501 AS pay_element_type_id,
  work_order,
  CASE
  WHEN contractor_reference = 'H01' THEN 'H3009'
  WHEN contractor_reference = 'H02' THEN 'H3007'
  WHEN contractor_reference = 'H03' THEN 'H3015'
  WHEN contractor_reference = 'H04' THEN 'H3002'
  WHEN contractor_reference = 'H05' THEN 'H3002'
  WHEN contractor_reference = 'H06' THEN 'H3003'
  WHEN contractor_reference = 'H07' THEN 'H3010'
  WHEN contractor_reference = 'H08' THEN 'H3002'
  WHEN contractor_reference = 'H09' THEN 'H3005'
  WHEN contractor_reference = 'H10' THEN 'H3010'
  WHEN contractor_reference = 'H11' THEN 'H3016'
  WHEN contractor_reference = 'H12' THEN 'H3004'
  WHEN contractor_reference = 'H13' THEN 'H3008'
  WHEN contractor_reference = 'H14' THEN 'H1040'
  WHEN contractor_reference = 'H15' THEN 'H1039'
  END AS cost_code,
  address_line AS address,
  description_of_work AS comment,
  closed_date AS closed_at,
  ROUND(operative_cost * job_percentage / 100, 4) AS value,
  0 AS duration,
  0 AS monday,
  0 AS tuesday,
  0 AS wednesday,
  0 AS thursday,
  0 AS friday,
  0 AS saturday,
  0 AS sunday,
  true AS read_only
FROM work_order_corrections
WHERE status_code = 50 AND is_outofhours = true;

-- Insert new out-of-hours pay elements that were closed as 'No Access'
INSERT INTO pay_elements (
  timesheet_id, pay_element_type_id, work_order, cost_code, address,
  comment, closed_at, value, duration, monday, tuesday, wednesday,
  thursday, friday, saturday, sunday, read_only
)
SELECT
  CONCAT(payroll_number, '/', TO_CHAR(DATE_TRUNC('week', closed_date), 'YYYY-MM-DD')) AS timesheet_id,
  501 AS pay_element_type_id,
  work_order,
  CASE
  WHEN contractor_reference = 'H01' THEN 'H3009'
  WHEN contractor_reference = 'H02' THEN 'H3007'
  WHEN contractor_reference = 'H03' THEN 'H3015'
  WHEN contractor_reference = 'H04' THEN 'H3002'
  WHEN contractor_reference = 'H05' THEN 'H3002'
  WHEN contractor_reference = 'H06' THEN 'H3003'
  WHEN contractor_reference = 'H07' THEN 'H3010'
  WHEN contractor_reference = 'H08' THEN 'H3002'
  WHEN contractor_reference = 'H09' THEN 'H3005'
  WHEN contractor_reference = 'H10' THEN 'H3010'
  WHEN contractor_reference = 'H11' THEN 'H3016'
  WHEN contractor_reference = 'H12' THEN 'H3004'
  WHEN contractor_reference = 'H13' THEN 'H3008'
  WHEN contractor_reference = 'H14' THEN 'H1040'
  WHEN contractor_reference = 'H15' THEN 'H1039'
  END AS cost_code,
  address_line AS address,
  description_of_work AS comment,
  closed_date AS closed_at,
  0 AS value,
  0 AS duration,
  0 AS monday,
  0 AS tuesday,
  0 AS wednesday,
  0 AS thursday,
  0 AS friday,
  0 AS saturday,
  0 AS sunday,
  true AS read_only
FROM work_order_corrections
WHERE status_code = 1000 AND is_outofhours = true;

-- Ensure overtime pay elements are at the correct rate
UPDATE pay_elements SET value = 21.98
WHERE closed_at >= '2022-03-28' AND pay_element_type_id = 401 AND value = 21.60;

COMMIT;

-- We created the view as temporary so PostgreSQL will clean
-- it up anyway but we'll drop it here for the sake of completeness
DROP TABLE work_order_corrections;
