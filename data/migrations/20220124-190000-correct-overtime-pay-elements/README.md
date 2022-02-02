# Correct Overtime Pay Elements

## Description

Between 18:25 on 13th January, 2022 and 18:06 on 18th January, 2022 all work orders closed in Repairs Hub were flagged as being paid as overtime. There are 371 in-hours work orders flagged as overtime which should be paid as SMVs into the bonus calculation scheme. There are 111 out-of-hours work orders which could either be paid as SMVs or a monetary amount. A survey of affected operatives was made to determine which of those 111 work orders should be paid as SMVs.

This script compiles a list of work orders that should be paid as SMVs and then deletes the relevant overtime pay elements from Bonus Calculation database. It then inserts new productive pay element records to correct the operative bonus calculations.

## Operations

1.  Export the operative work order data by running the `export-work-order-corrections.sql` script, e.g:

    ``` sh
    $ psql <repairs-hub-db-url> -f export-work-order-corrections.sql
    ```

    This will generate `in_hours_work_order_corrections.csv` and `out_of_hours_work_order_corrections.csv` files in this directory. The output from the command should be as follows:

    ```
    CREATE VIEW
    CREATE VIEW
    COPY 404
    COPY 69
    DROP VIEW
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
    COPY 404
    COPY 69
    BEGIN
    DELETE 471
    INSERT 0 473
    COMMIT
    DROP TABLE
    ```

    Anything else suggests an error and it should be raised in the development Slack channel.

3.  Remove the file exported in the first step, e.g:

    ``` sh
    $ rm in_hours_work_order_corrections.csv out_of_hours_work_order_corrections.csv
    ```

4.  Check the [Bonus Calculation][1] application to ensure corrections have been successfully applied.

[1]: https://dlo-bonus-scheme.hackney.gov.uk/manage/weeks
