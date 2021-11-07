using System.Collections.Generic;

namespace BonusCalcApi.V1.Boundary.Response
{
    public class SummaryResponse
    {
        public string Id { get; set; }
        public BonusPeriodResponse BonusPeriod { get; set; }
        public List<WeeklySummaryResponse> WeeklySummaries { get; set; }
    }
}
