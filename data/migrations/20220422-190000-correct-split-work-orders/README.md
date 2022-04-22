# Correct Split Work Orders

## Description

Up until 13th April, 2022 there existed a race condition in the DRS sync operation that would convert a split work order back to a single operative work order where a sync operation occurred during the interval between the split occurring and the work order being closed.

This script compiles a list of affected work orders and then deletes the relevant pay elements from Bonus Calculation database. It then inserts new pay element records to correct the operative bonus calculations as well as overtime and out-of-hours pay elements.

## Operations

1.  Export the operative work order data by running the `export-work-order-corrections.sql` script, e.g:

    ``` sh
    $ psql <repairs-hub-db-url> -f export-work-order-corrections.sql
    ```

    This will generate a `work_order_corrections.csv` file in this directory. The output from the command should be as follows:

    ```
    CREATE VIEW
    COPY 1761
    DROP VIEW
    ```

    Anything else suggests an error and it should be raised in the development Slack channel.

2.  Run the corrections script against the Bonus Calculation database, e.g:

    ``` sh
    $ psql <bonus-calculation-db-url> -f apply-work-order-corrections.sql
    ```

    The output from the command should be as follows:

    ```
    CREATE TABLE
    COPY 1761
    BEGIN
    DELETE 1657
    INSERT 0 1677
    INSERT 0 80
    INSERT 0 2
    INSERT 0 0
    INSERT 0 2
    INSERT 0 0
    UPDATE 0
    COMMIT
    DROP TABLE
    ```

    Anything else suggests an error and it should be raised in the development Slack channel.

3.  Remove the file exported in the first step, e.g:

    ``` sh
    $ rm work_order_corrections.csv
    ```

4.  Check the [Bonus Calculation][1] application to ensure corrections have been successfully applied.

[1]: https://dlo-bonus-scheme.hackney.gov.uk/manage/weeks
