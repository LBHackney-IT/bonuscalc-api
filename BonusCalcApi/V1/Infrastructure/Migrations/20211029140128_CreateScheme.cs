using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace V1.Infrastructure.Migrations
{
    public partial class CreateScheme : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "scheme_id",
                table: "pay_bands",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "scheme_id",
                table: "operatives",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "schemes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    type = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    description = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_schemes", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_pay_bands_scheme_id",
                table: "pay_bands",
                column: "scheme_id");

            migrationBuilder.CreateIndex(
                name: "ix_operatives_scheme_id",
                table: "operatives",
                column: "scheme_id");

            migrationBuilder.CreateIndex(
                name: "ix_schemes_description",
                table: "schemes",
                column: "description",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_operatives_schemes_scheme_id",
                table: "operatives",
                column: "scheme_id",
                principalTable: "schemes",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_pay_bands_schemes_scheme_id",
                table: "pay_bands",
                column: "scheme_id",
                principalTable: "schemes",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_operatives_schemes_scheme_id",
                table: "operatives");

            migrationBuilder.DropForeignKey(
                name: "fk_pay_bands_schemes_scheme_id",
                table: "pay_bands");

            migrationBuilder.DropTable(
                name: "schemes");

            migrationBuilder.DropIndex(
                name: "ix_pay_bands_scheme_id",
                table: "pay_bands");

            migrationBuilder.DropIndex(
                name: "ix_operatives_scheme_id",
                table: "operatives");

            migrationBuilder.DropColumn(
                name: "scheme_id",
                table: "pay_bands");

            migrationBuilder.DropColumn(
                name: "scheme_id",
                table: "operatives");
        }
    }
}
