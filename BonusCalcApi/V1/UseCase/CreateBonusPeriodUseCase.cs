using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BonusCalcApi.V1.Boundary.Request;
using BonusCalcApi.V1.Controllers.Helpers;
using BonusCalcApi.V1.Exceptions;
using BonusCalcApi.V1.Gateways.Interfaces;
using BonusCalcApi.V1.Infrastructure;
using BonusCalcApi.V1.UseCase.Interfaces;

namespace BonusCalcApi.V1.UseCase
{
    public class CreateBonusPeriodUseCase : ICreateBonusPeriodUseCase
    {
        private const int DaysPerPeriod = 91;

        private readonly IBonusPeriodGateway _bonusPeriodGateway;
        private readonly IOperativeHelpers _operativeHelpers;

        public CreateBonusPeriodUseCase(
            IBonusPeriodGateway bonusPeriodGateway,
            IOperativeHelpers operativeHelpers
        )
        {
            _bonusPeriodGateway = bonusPeriodGateway;
            _operativeHelpers = operativeHelpers;
        }

        public async Task<BonusPeriod> ExecuteAsync(BonusPeriodRequest request)
        {
            if (!IsValidDate(request.Id))
            {
                throw new BadRequestException($"Date format is invalid - it should be YYYY-MM-DD");
            }

            if (await ExistingPeriod(request.Id) != null)
            {
                throw new BadRequestException($"Bonus period '{request.Id}' already exists");
            }

            var dateTime = ParseDateTime(request.Id);

            if (dateTime == null)
            {
                throw new BadRequestException($"Date is invalid - could not parse '{request.Id}'");
            }

            if (dateTime <= FirstPeriodDate())
            {
                throw new BadRequestException($"Date is before the first bonus period");
            }

            if (dateTime <= await LastPeriodDate())
            {
                throw new BadRequestException($"Date is before the last bonus period");
            }

            if (DaysSinceFirstPeriod((DateTime) dateTime) % DaysPerPeriod > 0)
            {
                throw new BadRequestException($"Date is not a valid 13 week period");
            }

            return await _bonusPeriodGateway.CreateBonusPeriodAsync(request.Id);
        }

        private bool IsValidDate(string date)
        {
            return _operativeHelpers.IsValidDate(date);
        }

        private static DateTime? ParseDateTime(string date)
        {
            try
            {
                var dateTime = DateTime.Parse(date);
                return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, DateTimeKind.Utc);
            }
            catch (System.FormatException)
            {
                return null;
            }
        }

        private async Task<BonusPeriod> ExistingPeriod(string date)
        {
            return await _bonusPeriodGateway.GetBonusPeriodAsync(date);
        }

        private async Task<DateTime> LastPeriodDate()
        {
            var period = await _bonusPeriodGateway.GetLastBonusPeriodAsync();
            var dateTime = DateTime.Parse(period.Id);
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, DateTimeKind.Utc);
        }

        private static string FirstPeriod()
        {
            return Environment.GetEnvironmentVariable("FIRST_BONUS_PERIOD") ?? "2021-08-02";
        }

        private static DateTime FirstPeriodDate()
        {
            var dateTime = DateTime.Parse(FirstPeriod());
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, DateTimeKind.Utc);
        }

        private static int DaysSinceFirstPeriod(DateTime dateTime)
        {
            return (dateTime - FirstPeriodDate()).Days;
        }
    }
}
