using System;

namespace BonusCalcApi.V1.Boundary.Response
{
    public class OperativeSummaryResponse
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public TradeResponse Trade { get; set; }

        public int SchemeId { get; set; }

        public decimal ProductiveValue { get; set; }

        public decimal NonProductiveDuration { get; set; }

        public decimal NonProductiveValue { get; set; }

        public decimal TotalValue { get; set; }

        public decimal Utilisation { get; set; }

        public decimal ProjectedValue { get; set; }

        public decimal AverageUtilisation { get; set; }
    }
}
