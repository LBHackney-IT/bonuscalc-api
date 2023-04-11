using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace V1.Infrastructure.Migrations
{
    public partial class AddSpArchiveOperative : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
@"
CREATE OR REPLACE PROCEDURE public.sp_archive_operative(
	op_payroll_number text,
	op_leave_year text)
LANGUAGE 'sql'
AS $BODY$
UPDATE public.operatives SET is_archived = TRUE,
""name"" = name ||' LEFT '||op_leave_year
WHERE id  = op_payroll_number
$BODY$;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE public.sp_archive_operative(text, text)");
        }
    }
}
