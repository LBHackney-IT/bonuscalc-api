using System;

namespace BonusCalcApi.V1.Boundary.Response
{
    public class OutOfHoursSummaryResponse
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public TradeResponse Trade { get; set; }

        public string TradeCode { get; set; }

        public decimal TotalValue { get; set; }
    }
}
