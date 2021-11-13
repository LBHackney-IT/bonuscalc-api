using Microsoft.EntityFrameworkCore.Migrations;

namespace V1.Infrastructure.Migrations
{
    public partial class UpdateReactivePayBands : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE pay_bands SET value = 2160 WHERE scheme_id = 2 AND band = 1");
            migrationBuilder.Sql("UPDATE pay_bands SET value = 2771 WHERE scheme_id = 2 AND band = 2");
            migrationBuilder.Sql("UPDATE pay_bands SET value = 3132 WHERE scheme_id = 2 AND band = 3");
            migrationBuilder.Sql("UPDATE pay_bands SET value = 3367 WHERE scheme_id = 2 AND band = 4");
            migrationBuilder.Sql("UPDATE pay_bands SET value = 3619 WHERE scheme_id = 2 AND band = 5");
            migrationBuilder.Sql("UPDATE pay_bands SET value = 3891 WHERE scheme_id = 2 AND band = 6");
            migrationBuilder.Sql("UPDATE pay_bands SET value = 4183 WHERE scheme_id = 2 AND band = 7");
            migrationBuilder.Sql("UPDATE pay_bands SET value = 4496 WHERE scheme_id = 2 AND band = 8");
            migrationBuilder.Sql("UPDATE pay_bands SET value = 4834 WHERE scheme_id = 2 AND band = 9");

            migrationBuilder.Sql(@"
                CREATE OR REPLACE VIEW smvs_per_hour AS
                SELECT
                  scheme_id,
                  ARRAY_AGG((value / 36) ORDER BY value) AS value
                FROM pay_bands
                GROUP BY scheme_id
            ");

            migrationBuilder.Sql(@"
                UPDATE
                    pay_elements AS pe
                SET
                    value = ROUND(pe.duration * sph.value[
                                CASE WHEN pet.pay_at_band
                                THEN o.salary_band
                                ELSE 3 END
                            ], 4)::numeric(10,4)
                FROM
                    pay_element_types AS pet,
                    timesheets AS t,
                    operatives AS o,
                    smvs_per_hour AS sph
                WHERE
                    pe.pay_element_type_id = pet.id
                AND
                    pe.timesheet_id = t.id
                AND
                    t.operative_id = o.id
                AND
                    o.scheme_id = sph.scheme_id
                AND
                    pet.non_productive = TRUE;
            ");

            migrationBuilder.Sql("DROP VIEW smvs_per_hour");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE pay_bands SET value = 2160 WHERE scheme_id = 2 AND band = 1");
            migrationBuilder.Sql("UPDATE pay_bands SET value = 2772 WHERE scheme_id = 2 AND band = 2");
            migrationBuilder.Sql("UPDATE pay_bands SET value = 3132 WHERE scheme_id = 2 AND band = 3");
            migrationBuilder.Sql("UPDATE pay_bands SET value = 3366 WHERE scheme_id = 2 AND band = 4");
            migrationBuilder.Sql("UPDATE pay_bands SET value = 3618 WHERE scheme_id = 2 AND band = 5");
            migrationBuilder.Sql("UPDATE pay_bands SET value = 3888 WHERE scheme_id = 2 AND band = 6");
            migrationBuilder.Sql("UPDATE pay_bands SET value = 4182 WHERE scheme_id = 2 AND band = 7");
            migrationBuilder.Sql("UPDATE pay_bands SET value = 4494 WHERE scheme_id = 2 AND band = 8");
            migrationBuilder.Sql("UPDATE pay_bands SET value = 4836 WHERE scheme_id = 2 AND band = 9");

            migrationBuilder.Sql(@"
                CREATE OR REPLACE VIEW smvs_per_hour AS
                SELECT
                  scheme_id,
                  ARRAY_AGG((value / 36) ORDER BY value) AS value
                FROM pay_bands
                GROUP BY scheme_id
            ");

            migrationBuilder.Sql(@"
                UPDATE
                    pay_elements AS pe
                SET
                    value = ROUND(pe.duration * sph.value[
                                CASE WHEN pet.pay_at_band
                                THEN o.salary_band
                                ELSE 3 END
                            ], 4)::numeric(10,4)
                FROM
                    pay_element_types AS pet,
                    timesheets AS t,
                    operatives AS o,
                    smvs_per_hour AS sph
                WHERE
                    pe.pay_element_type_id = pet.id
                AND
                    pe.timesheet_id = t.id
                AND
                    t.operative_id = o.id
                AND
                    o.scheme_id = sph.scheme_id
                AND
                    pet.non_productive = TRUE;
            ");

            migrationBuilder.Sql("DROP VIEW smvs_per_hour");
        }
    }
}
