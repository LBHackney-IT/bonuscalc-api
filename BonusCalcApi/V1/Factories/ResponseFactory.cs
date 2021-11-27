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
                EmailAddress = operative.EmailAddress,
                Trade = operative.Trade.ToResponse(),
                Scheme = operative.Scheme?.ToResponse(),
                Section = operative.Section,
                SalaryBand = operative.SalaryBand,
                Utilisation = operative.Utilisation,
                FixedBand = operative.FixedBand,
                IsArchived = operative.IsArchived
            };
        }

        public static TimesheetResponse ToResponse(this Timesheet timesheet)
        {
            return new TimesheetResponse
            {
                Id = timesheet.Id,
                Utilisation = timesheet.Utilisation,
                Week = timesheet.Week.ToResponse(),
                PayElements = timesheet.PayElements.Select(pe => pe.ToResponse()).ToList()
            };
        }

        public static WeekResponse ToResponse(this Week week, bool includeBonusPeriod = true)
        {
            return new WeekResponse
            {
                Id = week.Id,
                Number = week.Number,
                BonusPeriod = includeBonusPeriod ? week.BonusPeriod.ToResponse(false) : null,
                ClosedAt = week.ClosedAt,
                StartAt = week.StartAt
            };
        }

        public static BonusPeriodResponse ToResponse(this BonusPeriod bonusPeriod, bool includeWeeks = true)
        {
            return new BonusPeriodResponse
            {
                Id = bonusPeriod.Id,
                Number = bonusPeriod.Number,
                Year = bonusPeriod.Year,
                ClosedAt = bonusPeriod.ClosedAt,
                StartAt = bonusPeriod.StartAt,
                Weeks = includeWeeks ? bonusPeriod.Weeks.Select(w => w.ToResponse(false)).ToList() : null
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
                ClosedAt = payElement.ClosedAt,
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
                Adjustment = payElementType.Adjustment,
                OutOfHours = payElementType.OutOfHours,
                Overtime = payElementType.Overtime,
                Selectable = payElementType.Selectable,
                SmvPerHour = payElementType.SmvPerHour
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
                PayBands = scheme.PayBands.Select(pb => pb.ToResponse()).ToList()
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

        public static SummaryResponse ToResponse(this Summary summary)
        {
            return new SummaryResponse
            {
                Id = summary.Id,
                BonusPeriod = summary.BonusPeriod.ToResponse(false),
                WeeklySummaries = summary.WeeklySummaries.Select(ws => ws.ToResponse()).ToList()
            };
        }

        public static WeeklySummaryResponse ToResponse(this WeeklySummary weeklySummary)
        {
            return new WeeklySummaryResponse
            {
                Number = weeklySummary.Number,
                StartAt = weeklySummary.StartAt,
                ClosedAt = weeklySummary.ClosedAt,
                ProductiveValue = weeklySummary.ProductiveValue,
                NonProductiveDuration = weeklySummary.NonProductiveDuration,
                NonProductiveValue = weeklySummary.NonProductiveValue,
                TotalValue = weeklySummary.TotalValue,
                Utilisation = weeklySummary.Utilisation,
                ProjectedValue = weeklySummary.ProjectedValue,
                AverageUtilisation = weeklySummary.AverageUtilisation
            };
        }

        public static WorkElementResponse ToResponse(this WorkElement workElement)
        {
            return new WorkElementResponse
            {
                Id = workElement.Id,
                Type = workElement.Type,
                WorkOrder = workElement.WorkOrder,
                Address = workElement.Address,
                Description = workElement.Description,
                OperativeId = workElement.OperativeId,
                OperativeName = workElement.OperativeName,
                WeekId = workElement.WeekId,
                Value = workElement.Value,
                ClosedAt = workElement.ClosedAt
            };
        }
    }
}
