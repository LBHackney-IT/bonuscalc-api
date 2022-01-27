using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

namespace V1.Infrastructure.Migrations
{
    public partial class ChangeSearchVectorConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_operatives_search_vector",
                table: "operatives");

            migrationBuilder.DropColumn(
                name: "search_vector",
                table: "operatives");

            migrationBuilder.AddColumn<NpgsqlTsVector>(
                name: "search_vector",
                table: "operatives",
                type: "tsvector",
                nullable: false)
                .Annotation("Npgsql:TsVectorConfig", "simple")
                .Annotation("Npgsql:TsVectorProperties", new[] { "id", "name", "trade_id", "section" });

            migrationBuilder.CreateIndex(
                name: "ix_operatives_search_vector",
                table: "operatives",
                column: "search_vector")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.Sql("DROP VIEW work_elements");

            migrationBuilder.DropIndex(
                name: "ix_pay_elements_search_vector",
                table: "pay_elements");

            migrationBuilder.DropColumn(
                name: "search_vector",
                table: "pay_elements");

            migrationBuilder.AddColumn<NpgsqlTsVector>(
                name: "search_vector",
                table: "pay_elements",
                type: "tsvector",
                nullable: false)
                .Annotation("Npgsql:TsVectorConfig", "simple")
                .Annotation("Npgsql:TsVectorProperties", new[] { "work_order", "address" });

            migrationBuilder.CreateIndex(
                name: "ix_pay_elements_search_vector",
                table: "pay_elements",
                column: "search_vector")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.Sql(@"
CREATE VIEW work_elements AS
SELECT
    pe.id,
    pet.description AS type,
    pe.work_order,
    pe.address,
    pe.comment AS description,
    t.operative_id,
    o.name AS operative_name,
    t.week_id,
    pe.value,
    pe.closed_at,
    pe.search_vector
FROM
    pay_elements AS pe
INNER JOIN
    pay_element_types AS pet
ON
    pe.pay_element_type_id = pet.id
INNER JOIN
    timesheets AS t
ON
    pe.timesheet_id = t.id
INNER JOIN
    operatives AS o
ON
    t.operative_id = o.id;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_operatives_search_vector",
                table: "operatives");

            migrationBuilder.DropColumn(
                name: "search_vector",
                table: "operatives");

            migrationBuilder.AddColumn<NpgsqlTsVector>(
                name: "search_vector",
                table: "operatives",
                type: "tsvector",
                nullable: false)
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "id", "name", "trade_id", "section" });

            migrationBuilder.CreateIndex(
                name: "ix_operatives_search_vector",
                table: "operatives",
                column: "search_vector")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.Sql("DROP VIEW work_elements");

            migrationBuilder.DropIndex(
                name: "ix_pay_elements_search_vector",
                table: "pay_elements");

            migrationBuilder.DropColumn(
                name: "search_vector",
                table: "pay_elements");

            migrationBuilder.AddColumn<NpgsqlTsVector>(
                name: "search_vector",
                table: "pay_elements",
                type: "tsvector",
                nullable: false)
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "work_order", "address" });

            migrationBuilder.CreateIndex(
                name: "ix_pay_elements_search_vector",
                table: "pay_elements",
                column: "search_vector")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.Sql(@"
CREATE VIEW work_elements AS
SELECT
    pe.id,
    pet.description AS type,
    pe.work_order,
    pe.address,
    pe.comment AS description,
    t.operative_id,
    o.name AS operative_name,
    t.week_id,
    pe.value,
    pe.closed_at,
    pe.search_vector
FROM
    pay_elements AS pe
INNER JOIN
    pay_element_types AS pet
ON
    pe.pay_element_type_id = pet.id
INNER JOIN
    timesheets AS t
ON
    pe.timesheet_id = t.id
INNER JOIN
    operatives AS o
ON
    t.operative_id = o.id;
            ");
        }
    }
}
