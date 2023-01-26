using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace V1.Infrastructure.Migrations
{
    public partial class UpdateBonusRatesFor2023 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE public.bonus_rates SET rates = ARRAY [-481.99,-372.58,0.00,189.69,393.61,612.83,848.48,1101.81,1374.14] WHERE id = '1'");
            migrationBuilder.Sql(@"UPDATE public.bonus_rates SET rates = ARRAY [-443.96,-362.31,0.00,184.46,382.76,595.93,825.09,1071.43,1336.25] WHERE id = '2'");
            migrationBuilder.Sql(@"UPDATE public.bonus_rates SET rates = ARRAY [-352.87,-341.25,0.00,173.74,360.50,561.28,777.11,1009.13,1258.55] WHERE id = '3'");
            migrationBuilder.Sql(@"UPDATE public.bonus_rates SET rates = ARRAY [-336.67,-336.67,0.00,173.74,360.50,561.28,777.11,1009.13,1258.55] WHERE id = '3A'");
            migrationBuilder.Sql(@"UPDATE public.bonus_rates SET rates = ARRAY [-225.60,-225.60,0.00,160.74,333.54,519.30,719.00,933.66,1164.43] WHERE id = '4'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE public.bonus_rates SET rates = ARRAY [-503.44,-372.58,0.00,189.69,393.61,612.83,848.48,1101.81,1374.14] WHERE id = '1'");
            migrationBuilder.Sql(@"UPDATE public.bonus_rates SET rates = ARRAY [-465.49,-362.31,0.00,184.46,382.76,595.93,825.09,1071.43,1336.25] WHERE id = '2'");
            migrationBuilder.Sql(@"UPDATE public.bonus_rates SET rates = ARRAY [-373.58,-341.25,0.00,173.74,360.50,561.28,777.11,1009.13,1258.55] WHERE id = '3'");
            migrationBuilder.Sql(@"UPDATE public.bonus_rates SET rates = ARRAY [-357.98,-341.25,0.00,173.74,360.50,561.28,777.11,1009.13,1258.55] WHERE id = '3A'");
            migrationBuilder.Sql(@"UPDATE public.bonus_rates SET rates = ARRAY [-245.66,-245.66,0.00,160.74,333.54,519.30,719.00,933.66,1164.43] WHERE id = '4'");
        }


    }
}
