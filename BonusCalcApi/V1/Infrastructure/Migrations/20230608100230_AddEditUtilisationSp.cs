using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace V1.Infrastructure.Migrations
{
    public partial class AddEditUtilisationSp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
@"
CREATE OR REPLACE PROCEDURE public.sp_edit_utilisation(
	op_id text,
	op_util numeric,
	effective_date date)
LANGUAGE 'plpgsql'
AS $BODY$
BEGIN
		UPDATE operatives 
		SET	utilisation = op_util
		WHERE id = op_id;
	
		UPDATE timesheets 
		SET utilisation = op_util
		WHERE operative_id  = op_id
		AND CAST(week_id AS DATE)  >= effective_date;
		END
$BODY$;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE public.sp_edit_utilisation(text, numeric, date)");
        }
    }
}
