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
                Trade = operative.Trade.ToResponse(),
                Scheme = operative.Scheme?.ToResponse(),
                Section = operative.Section,
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
                PayElements = timesheet.PayElements.Select(pe => pe.ToResponse()).OrderBy(pe => pe.Id).ToList()
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
                Number = bonusPeriod.Number,
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
                Monday = payElement.Monday,
                Tuesday = payElement.Tuesday,
                Wednesday = payElement.Wednesday,
                Thursday = payElement.Thursday,
                Friday = payElement.Friday,
                Saturday = payElement.Saturday,
                Sunday = payElement.Sunday,
                Duration = payElement.Duration,
                Value = payElement.Value,
                WorkOrder = payElement.WorkOrder,
                PayElementType = payElement.PayElementType.ToResponse()
            };
        }

        public static PayElementTypeResponse ToResponse(this PayElementType payElementType)
        {
            return new PayElementTypeResponse
            {
                Id = payElementType.Id,
                Description = payElementType.Description,
                PayAtBand = payElementType.PayAtBand,
                Paid = payElementType.Paid,
                NonProductive = payElementType.NonProductive,
                Productive = payElementType.Productive,
                Adjustment = payElementType.Adjustment
            };
        }

        public static TradeResponse ToResponse(this Trade trade)
        {
            return new TradeResponse
            {
                Id = trade.Id,
                Description = trade.Description
            };
        }

        public static SchemeResponse ToResponse(this Scheme scheme)
        {
            return new SchemeResponse
            {
                Type = scheme.Type,
                Description = scheme.Description,
                ConversionFactor = scheme.ConversionFactor,
                PayBands = scheme.PayBands.Select(pb => pb.ToResponse()).OrderBy(pb => pb.Band).ToList()
            };
        }

        public static PayBandResponse ToResponse(this PayBand payBand)
        {
            return new PayBandResponse
            {
                Band = payBand.Band,
                Value = payBand.Value
            };
        }
    }
}
