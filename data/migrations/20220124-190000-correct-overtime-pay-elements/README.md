# Correct Overtime Pay Elements

## Description

Between 18:25 on 13th January, 2022 and 18:06 on 18th January, 2022 all work orders closed in Repairs Hub were flagged as being paid as overtime. This migration corrects the errors for work orders closed by operatives on their mobile devices during normal hours. This can be done automatically since they wouldn't have been presented with the overtime option when closing. There will need to be a follow on migration that corrects it for work orders outside of normal hours since there is no way to identify whether the overtime option was selected deliberately.

## Operations

1.  Export the operative work order data by running the `export-work-order-corrections.sql` script, e.g:

    ``` sh
    $ psql <repairs-hub-db-url> -f export-work-order-corrections.sql
    ```

    This will generate a `work_order_corrections.csv` file in this directory. The output from the command should be as follows:

    ```
    CREATE VIEW
    COPY 404
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
    BEGIN
    DELETE 402
    INSERT 0 404
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
