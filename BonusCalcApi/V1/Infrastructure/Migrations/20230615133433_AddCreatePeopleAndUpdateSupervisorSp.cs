using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace V1.Infrastructure.Migrations
{
    public partial class AddCreatePeopleAndUpdateSupervisorSp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
@"
CREATE OR REPLACE PROCEDURE public.sp_create_people_record(
	person_name text,
	email_address text)
LANGUAGE 'plpgsql'
AS $BODY$
BEGIN
		INSERT INTO people
		(id,
		name,
		email_address)
		VALUES
		(
		(SELECT MAX(CAST (id AS numeric ))+1 FROM people p), 
		person_name,
		email_address) ;
	END;
$BODY$;");

            migrationBuilder.Sql(
@"
CREATE OR REPLACE PROCEDURE public.sp_update_supervisor(
	op_id text,
	op_supervisor_people_id text)
LANGUAGE 'plpgsql'
AS $BODY$
BEGIN
		UPDATE operatives 
		SET	supervisor_id  = op_supervisor_people_id
		WHERE id = op_id;
	END;
$BODY$;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE public.sp_create_people_record(text, text)");

            migrationBuilder.Sql("DROP PROCEDURE public.sp_update_supervisor(text, text)");
        }
    }
}
