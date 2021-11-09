using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BonusCalcApi.V1.Infrastructure
{
    public class WeeklySummary
    {
        public string Id { get; set; }

        public string SummaryId { get; set; }

        public int Number { get; set; }

        public DateTime StartAt { get; set; }

        public DateTime? ClosedAt { get; set; }

        public decimal ProductiveValue { get; set; }

        public decimal NonProductiveDuration { get; set; }

        public decimal NonProductiveValue { get; set; }

        public decimal TotalValue { get; set; }

        public decimal ProjectedValue { get; set; }
    }
}
