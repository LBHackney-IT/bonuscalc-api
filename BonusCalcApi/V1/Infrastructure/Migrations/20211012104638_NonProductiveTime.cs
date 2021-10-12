using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace V1.Infrastructure.Migrations
{
    public partial class NonProductiveTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "non_productive_times",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    standard_minute_value = table.Column<double>(type: "double precision", nullable: false),
                    hours = table.Column<double>(type: "double precision", nullable: false),
                    date_of_work = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    date_recorded = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_non_productive_times", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "non_productive_times");
        }
    }
}
