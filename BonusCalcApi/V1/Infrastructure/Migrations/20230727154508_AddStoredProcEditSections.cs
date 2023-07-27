using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace V1.Infrastructure.Migrations
{
    public partial class AddStoredProcEditSections : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
@"CREATE OR REPLACE PROCEDURE public.sp_edit_section(
	op_id text,
	op_section text)
LANGUAGE 'sql'
AS $BODY$
UPDATE public.operatives SET SECTION = op_section WHERE id = op_id
$BODY$;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE public.sp_edit_section(text, text)");
        }
    }
}
