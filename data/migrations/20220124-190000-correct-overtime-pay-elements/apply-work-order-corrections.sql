-- Create temporary table to hold records dumped from Repairs Hub database
CREATE TEMPORARY TABLE work_order_corrections (
  work_order varchar(10) NOT NULL,
  status_code integer NOT NULL,
  address_line text,
  description_of_work text,
  closed_date timestamp without time zone NOT NULL,
  payroll_number varchar(6) NOT NULL,
  job_percentage numeric NOT NULL,
  total_smv numeric NOT NULL
);

-- Import work order corrections
\COPY work_order_corrections(work_order, status_code, address_line, description_of_work, closed_date, payroll_number, job_percentage, total_smv) FROM 'in_hours_work_order_corrections.csv' CSV HEADER;
\COPY work_order_corrections(work_order, status_code, address_line, description_of_work, closed_date, payroll_number, job_percentage, total_smv) FROM 'out_of_hours_work_order_corrections.csv' CSV HEADER;

-- Wrap the changes in a transaction to ensure they're applied as a whole
BEGIN;

-- Remove the existing overtime pay elements as not all were created
DELETE FROM pay_elements
WHERE work_order IN (
  SELECT DISTINCT(work_order) FROM work_order_corrections
);

-- Insert new productive pay elements
INSERT INTO pay_elements (
  timesheet_id, pay_element_type_id, work_order, address, comment,
  closed_at, value, duration, monday, tuesday, wednesday, thursday,
  friday, saturday, sunday, read_only
)
SELECT
  CONCAT(payroll_number, '/', TO_CHAR(DATE_TRUNC('week', closed_date), 'YYYY-MM-DD')) AS timesheet_id,
  301 AS pay_element_type_id,
  work_order,
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
FROM work_order_corrections;

COMMIT;

-- We created the view as temporary so PostgreSQL will clean
-- it up anyway but we'll drop it here for the sake of completeness
DROP TABLE work_order_corrections;
