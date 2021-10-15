using System;

namespace BonusCalcApi.V1.Boundary.Response
{
    public class WeekResponse
    {
        public string Id { get; set; }

        public BonusPeriodResponse BonusPeriod { get; set; }

        public DateTime StartAt { get; set; }

        public int Number { get; set; }

        public DateTime? ClosedAt { get; set; }
    }
}
