using System;
using System.ComponentModel.DataAnnotations;

namespace BonusCalcApi.V1.Infrastructure
{
    public class OperativeSummary
    {
        [StringLength(6)]
        public string Id { get; set; }

        public string Name { get; set; }

        [StringLength(10)]
        public string WeekId { get; set; }

        [StringLength(3)]
        public string TradeId { get; set; }

        public string TradeDescription { get; set; }

        public int SchemeId { get; set; }

        public bool IsArchived { get; set; }

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
