using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace V1.Infrastructure.Migrations
{
    public partial class AddArchiveOperativeSp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE OR REPLACE PROCEDURE public.sp_archive_operative(op_id text)
LANGUAGE SQL
AS $BODY$
UPDATE public.operatives SET is_archived = true WHERE id = op_id
$BODY$;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DROP PROCEDURE IF EXISTS public.sp_archive_operative(op_id text);
            ");
        }
    }
}
