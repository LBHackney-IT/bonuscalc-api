using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace V1.Infrastructure.Migrations
{
    public partial class CreatePayElement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "pay_elements",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    timesheet_id = table.Column<int>(type: "integer", nullable: false),
                    pay_element_type_id = table.Column<int>(type: "integer", nullable: false),
                    week_day = table.Column<int>(type: "integer", nullable: false),
                    work_order = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    address = table.Column<string>(type: "text", nullable: true),
                    comment = table.Column<string>(type: "text", nullable: true),
                    productive = table.Column<bool>(type: "boolean", nullable: false),
                    duration = table.Column<decimal>(type: "numeric(10,4)", precision: 10, scale: 4, nullable: false),
                    value = table.Column<decimal>(type: "numeric(10,4)", precision: 10, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pay_elements", x => x.id);
                    table.ForeignKey(
                        name: "fk_pay_elements_pay_element_types_pay_element_type_id",
                        column: x => x.pay_element_type_id,
                        principalTable: "pay_element_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_pay_elements_timesheets_timesheet_id",
                        column: x => x.timesheet_id,
                        principalTable: "timesheets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_pay_elements_pay_element_type_id",
                table: "pay_elements",
                column: "pay_element_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_pay_elements_timesheet_id",
                table: "pay_elements",
                column: "timesheet_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "pay_elements");
        }
    }
}
