using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

namespace V1.Infrastructure.Migrations
{
    public partial class AddSearchVectorToOperatives : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<NpgsqlTsVector>(
                name: "search_vector",
                table: "operatives",
                type: "tsvector",
                nullable: true)
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "id", "name", "trade_id", "section" });

            migrationBuilder.CreateIndex(
                name: "ix_operatives_search_vector",
                table: "operatives",
                column: "search_vector")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_operatives_search_vector",
                table: "operatives");

            migrationBuilder.DropColumn(
                name: "search_vector",
                table: "operatives");
        }
    }
}
