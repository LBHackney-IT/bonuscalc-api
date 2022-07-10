using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace V1.Infrastructure.Migrations
{
    public partial class AddIsArchivedToOperativeSummaries : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW operative_summaries");

            migrationBuilder.Sql(@"
                CREATE VIEW operative_summaries AS
                SELECT
                    ws.operative_id AS id,
                    ws.week_id,
                    o.name,
                    o.trade_id,
                    t.description AS trade_description,
                    o.scheme_id,
                    o.is_archived,
                    ws.productive_value,
                    ws.non_productive_duration,
                    ws.non_productive_value,
                    ws.total_value,
                    ws.utilisation,
                    ws.projected_value,
                    ws.average_utilisation,
                    ws.report_sent_at
                FROM
                    weekly_summaries AS ws
                INNER JOIN
                    operatives AS o
                ON
                    ws.operative_id = o.id
                INNER JOIN
                    trades AS t
                ON
                    o.trade_id = t.id
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW operative_summaries");

            migrationBuilder.Sql(@"
                CREATE VIEW operative_summaries AS
                SELECT
                    ws.operative_id AS id,
                    ws.week_id,
                    o.name,
                    o.trade_id,
                    t.description AS trade_description,
                    o.scheme_id,
                    ws.productive_value,
                    ws.non_productive_duration,
                    ws.non_productive_value,
                    ws.total_value,
                    ws.utilisation,
                    ws.projected_value,
                    ws.average_utilisation,
                    ws.report_sent_at
                FROM
                    weekly_summaries AS ws
                INNER JOIN
                    operatives AS o
                ON
                    ws.operative_id = o.id
                INNER JOIN
                    trades AS t
                ON
                    o.trade_id = t.id
            ");
        }
    }
}
