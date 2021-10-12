using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace V1.Infrastructure.Migrations
{
    public partial class CreatePayElementType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "pay_element_types",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    pay_at_band = table.Column<bool>(type: "boolean", nullable: false),
                    paid = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pay_element_types", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_pay_element_types_description",
                table: "pay_element_types",
                column: "description",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "pay_element_types");
        }
    }
}
