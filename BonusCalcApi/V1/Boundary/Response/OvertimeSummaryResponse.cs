using System;

namespace BonusCalcApi.V1.Boundary.Response
{
    public class OvertimeSummaryResponse
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public TradeResponse Trade { get; set; }

        public decimal TotalValue { get; set; }
    }
}
