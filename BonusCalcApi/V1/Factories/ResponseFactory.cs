using System.Collections.Generic;
using System.Linq;
using BonusCalcApi.V1.Boundary.Response;
using BonusCalcApi.V1.Infrastructure;

namespace BonusCalcApi.V1.Factories
{
    public static class ResponseFactory
    {
        public static OperativeResponse ToResponse(this Operative operative)
        {
            return new OperativeResponse
            {
                Id = operative.Id,
                Name = operative.Name,
                Trade = operative.Trade,
                Section = operative.Section,
                Scheme = operative.Scheme,
                SalaryBand = operative.SalaryBand,
                FixedBand = operative.FixedBand,
                IsArchived = operative.IsArchived
            };
        }

        public static TimesheetResponse ToResponse(this Timesheet timesheet)
        {
            return new TimesheetResponse
            {
                Id = timesheet.Id,
                Week = timesheet.Week.ToResponse(),
                PayElements = timesheet.PayElements.Select(pe => pe.ToResponse()).ToList()
            };
        }
        public static WeekResponse ToResponse(this Week week)
        {
            return new WeekResponse
            {
                Id = week.Id,
                Number = week.Number,
                BonusPeriod = week.BonusPeriod.ToResponse(),
                ClosedAt = week.ClosedAt,
                StartAt = week.StartAt
            };
        }
        public static BonusPeriodResponse ToResponse(this BonusPeriod bonusPeriod)
        {
            return new BonusPeriodResponse
            {
                Id = bonusPeriod.Id,
                Period = bonusPeriod.Period,
                Year = bonusPeriod.Year,
                ClosedAt = bonusPeriod.ClosedAt,
                StartAt = bonusPeriod.StartAt
            };
        }
        public static PayElementResponse ToResponse(this PayElement payElement)
        {
            return new PayElementResponse
            {
                Id = payElement.Id,
                Address = payElement.Address,
                Comment = payElement.Comment,
                Duration = payElement.Duration,
                Productive = payElement.Productive,
                Value = payElement.Value,
                WeekDay = payElement.WeekDay,
                WorkOrder = payElement.WorkOrder,
                PayElementTypeId = payElement.PayElementTypeId
            };
        }
    }
}
