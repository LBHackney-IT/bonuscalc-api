using Microsoft.EntityFrameworkCore.Migrations;

namespace V1.Infrastructure.Migrations
{
    public partial class DropIdColumnDefaults : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE bonus_periods ALTER COLUMN id DROP DEFAULT
            ");

            migrationBuilder.Sql(@"
                ALTER TABLE timesheets ALTER COLUMN week_id DROP DEFAULT
            ");

            migrationBuilder.Sql(@"
                ALTER TABLE weeks ALTER COLUMN id DROP DEFAULT
            ");

            migrationBuilder.Sql(@"
                ALTER TABLE weeks ALTER COLUMN bonus_period_id DROP DEFAULT
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE bonus_periods ALTER COLUMN id SET DEFAULT ''::text
            ");

            migrationBuilder.Sql(@"
                ALTER TABLE timesheets ALTER COLUMN week_id SET DEFAULT ''::text
            ");

            migrationBuilder.Sql(@"
                ALTER TABLE weeks ALTER COLUMN id SET DEFAULT ''::text
            ");

            migrationBuilder.Sql(@"
                ALTER TABLE weeks ALTER COLUMN bonus_period_id SET DEFAULT ''::text
            ");
        }
    }
}
