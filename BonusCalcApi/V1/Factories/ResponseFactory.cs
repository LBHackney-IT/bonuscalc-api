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
                Manager = operative.Manager?.ToResponse(),
                Supervisor = operative.Supervisor?.ToResponse(),
                Trade = operative.Trade.ToResponse(),
                Scheme = operative.Scheme.ToResponse(),
                Section = operative.Section,
                SalaryBand = operative.SalaryBand,
                Utilisation = operative.Utilisation,
                FixedBand = operative.FixedBand,
                IsArchived = operative.IsArchived
            };
        }

        public static PersonResponse ToResponse(this Person person)
        {
            return new PersonResponse
            {
                Id = person.Id,
                Name = person.Name,
                EmailAddress = person.EmailAddress
            };
        }

        public static OperativeSummaryResponse ToResponse(this OperativeSummary operativeSummary)
        {
            return new OperativeSummaryResponse
            {
                Id = operativeSummary.Id,
                Name = operativeSummary.Name,
                Trade = new TradeResponse
                {
                    Id = operativeSummary.TradeId,
                    Description = operativeSummary.TradeDescription
                },
                SchemeId = operativeSummary.SchemeId,
                ProductiveValue = operativeSummary.ProductiveValue,
                NonProductiveDuration = operativeSummary.NonProductiveDuration,
                NonProductiveValue = operativeSummary.NonProductiveValue,
                TotalValue = operativeSummary.TotalValue,
                Utilisation = operativeSummary.Utilisation,
                ProjectedValue = operativeSummary.ProjectedValue,
                AverageUtilisation = operativeSummary.AverageUtilisation,
                ReportSentAt = operativeSummary.ReportSentAt
            };
        }

        public static OutOfHoursSummaryResponse ToResponse(this OutOfHoursSummary outOfHoursSummary)
        {
            return new OutOfHoursSummaryResponse
            {
                Id = outOfHoursSummary.Id,
                Name = outOfHoursSummary.Name,
                Trade = new TradeResponse
                {
                    Id = outOfHoursSummary.TradeId,
                    Description = outOfHoursSummary.TradeDescription
                },
                TradeCode = outOfHoursSummary.TradeCode,
                TotalValue = outOfHoursSummary.TotalValue
            };
        }

        public static OvertimeSummaryResponse ToResponse(this OvertimeSummary overtimeSummary)
        {
            return new OvertimeSummaryResponse
            {
                Id = overtimeSummary.Id,
                Name = overtimeSummary.Name,
                Trade = new TradeResponse
                {
                    Id = overtimeSummary.TradeId,
                    Description = overtimeSummary.TradeDescription
                },
                TotalValue = overtimeSummary.TotalValue
            };
        }

        public static TimesheetResponse ToResponse(this Timesheet timesheet)
        {
            return new TimesheetResponse
            {
                Id = timesheet.Id,
                Utilisation = timesheet.Utilisation,
                ReportSentAt = timesheet.ReportSentAt,
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
                BonusPeriod = new BonusPeriodResponse()
                {
                    Id = week.BonusPeriod.Id,
                    Number = week.BonusPeriod.Number,
                    Year = week.BonusPeriod.Year,
                    ClosedAt = week.BonusPeriod.ClosedAt,
                    StartAt = week.BonusPeriod.StartAt,
                },
                ClosedAt = week.ClosedAt,
                ClosedBy = week.ClosedBy,
                ReportsSentAt = week.ReportsSentAt,
                StartAt = week.StartAt,
                OperativeSummaries = week.OperativeSummaries?.Select(os => os.ToResponse()).ToList()
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
                StartAt = bonusPeriod.StartAt,
                Weeks = bonusPeriod.Weeks?.Select(w => w.ToResponse()).ToList()
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
                TradeCode = payElement.TradeCode,
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
                Id = scheme.Id,
                Type = scheme.Type,
                Description = scheme.Description,
                ConversionFactor = scheme.ConversionFactor,
                MaxValue = scheme.MaxValue,
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
                BonusPeriod = summary.BonusPeriod.ToResponse(),
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
                AverageUtilisation = weeklySummary.AverageUtilisation,
                ReportSentAt = weeklySummary.ReportSentAt
            };
        }

        public static WorkElementResponse ToResponse(this WorkElement workElement)
        {
            return new WorkElementResponse
            {
                Id = workElement.Id,
                PayElementType = workElement.PayElementType.ToResponse(),
                WorkOrder = workElement.WorkOrder,
                Address = workElement.Address,
                Description = workElement.Description,
                OperativeId = workElement.OperativeId,
                OperativeName = workElement.OperativeName,
                Week = workElement.Week.ToResponse(),
                Value = workElement.Value,
                ClosedAt = workElement.ClosedAt
            };
        }
    }
}
