using Microsoft.EntityFrameworkCore.Migrations;

namespace V1.Infrastructure.Migrations
{
    public partial class AddMissingIndexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_pay_elements_timesheet_id",
                table: "pay_elements",
                column: "timesheet_id");

            migrationBuilder.AlterColumn<string>(
                name: "timesheet_id",
                table: "pay_elements",
                defaultValue: null,
                oldDefaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "id",
                table: "timesheets",
                defaultValue: null,
                oldDefaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "id",
                table: "timesheets",
                defaultValue: "",
                oldDefaultValue: null);

            migrationBuilder.AlterColumn<string>(
                name: "timesheet_id",
                table: "pay_elements",
                defaultValue: "",
                oldDefaultValue: null);

            migrationBuilder.DropIndex(
                name: "ix_pay_elements_timesheet_id");
        }
    }
}
