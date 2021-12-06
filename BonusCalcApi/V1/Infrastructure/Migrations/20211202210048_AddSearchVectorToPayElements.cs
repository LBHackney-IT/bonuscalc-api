using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

namespace V1.Infrastructure.Migrations
{
    public partial class AddSearchVectorToPayElements : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<NpgsqlTsVector>(
                name: "search_vector",
                table: "pay_elements",
                type: "tsvector",
                nullable: true)
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "work_order", "address" });

            migrationBuilder.CreateIndex(
                name: "ix_pay_elements_search_vector",
                table: "pay_elements",
                column: "search_vector")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_pay_elements_search_vector",
                table: "pay_elements");

            migrationBuilder.DropColumn(
                name: "search_vector",
                table: "pay_elements");
        }
    }
}
