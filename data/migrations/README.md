# Data Migrations

This directory contains a record of data migrations applied to correct errors in the Bonus Calculation database arising from bugs in both the Repairs Hub application and the Bonus Calculation application.

## Guidelines

1. Ensure that no PII (Personally Identifiable Information) is in any script.

2. Wrap commands in transactions where appropriate so if an unexpected error occurs the database is left in a consistent state.

3. If the application is running ensure that any locks obtained don't block read access at any time and write access for the minimum time possible.

4. Create a directory for each migration using a name that follows the pattern `<timestamp>-<description>`, e.g. `20220122-080000-create-new-trades`, where the timestamp is the date/time at which it is intended the migration should be applied.

5. Inside the directory include a README.md file that describes the issue and what operations that are to be carried out.
