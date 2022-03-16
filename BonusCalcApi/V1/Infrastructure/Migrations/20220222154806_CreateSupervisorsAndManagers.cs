using Microsoft.EntityFrameworkCore.Migrations;

namespace V1.Infrastructure.Migrations
{
    public partial class CreateSupervisorsAndManagers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "manager_id",
                table: "operatives",
                type: "character varying(6)",
                maxLength: 6,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "supervisor_id",
                table: "operatives",
                type: "character varying(6)",
                maxLength: 6,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "people",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email_address = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_people", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_operatives_manager_id",
                table: "operatives",
                column: "manager_id");

            migrationBuilder.CreateIndex(
                name: "ix_operatives_supervisor_id",
                table: "operatives",
                column: "supervisor_id");

            migrationBuilder.AddForeignKey(
                name: "fk_operatives_people_manager_id",
                table: "operatives",
                column: "manager_id",
                principalTable: "people",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_operatives_people_supervisor_id",
                table: "operatives",
                column: "supervisor_id",
                principalTable: "people",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_operatives_people_manager_id",
                table: "operatives");

            migrationBuilder.DropForeignKey(
                name: "fk_operatives_people_supervisor_id",
                table: "operatives");

            migrationBuilder.DropTable(
                name: "people");

            migrationBuilder.DropIndex(
                name: "ix_operatives_manager_id",
                table: "operatives");

            migrationBuilder.DropIndex(
                name: "ix_operatives_supervisor_id",
                table: "operatives");

            migrationBuilder.DropColumn(
                name: "manager_id",
                table: "operatives");

            migrationBuilder.DropColumn(
                name: "supervisor_id",
                table: "operatives");
        }
    }
}
