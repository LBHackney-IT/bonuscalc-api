using System;
using System.ComponentModel.DataAnnotations;

namespace BonusCalcApi.V1.Infrastructure
{
    public class WeeklySummary
    {
        [StringLength(28)]
        public string Id { get; set; }

        [StringLength(17)]
        public string SummaryId { get; set; }

        [StringLength(10)]
        public string WeekId { get; set; }

        [StringLength(6)]
        public string OperativeId { get; set; }

        public int Number { get; set; }

        public DateTime StartAt { get; set; }

        public DateTime? ClosedAt { get; set; }

        public decimal ProductiveValue { get; set; }

        public decimal NonProductiveDuration { get; set; }

        public decimal NonProductiveValue { get; set; }

        public decimal TotalValue { get; set; }

        public decimal Utilisation { get; set; }

        public decimal ProjectedValue { get; set; }

        public decimal AverageUtilisation { get; set; }

        public DateTime? ReportSentAt { get; set; }
    }
}
