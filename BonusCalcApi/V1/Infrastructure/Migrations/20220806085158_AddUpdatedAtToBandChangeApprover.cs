using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace V1.Infrastructure.Migrations
{
    public partial class AddUpdatedAtToBandChangeApprover : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "manager_updated_at",
                table: "band_changes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "supervisor_updated_at",
                table: "band_changes",
                type: "timestamp with time zone",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "manager_updated_at",
                table: "band_changes");

            migrationBuilder.DropColumn(
                name: "supervisor_updated_at",
                table: "band_changes");
        }
    }
}
