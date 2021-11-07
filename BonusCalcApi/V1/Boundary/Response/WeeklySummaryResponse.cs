using System;

namespace BonusCalcApi.V1.Boundary.Response
{
    public class WeeklySummaryResponse
    {
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
