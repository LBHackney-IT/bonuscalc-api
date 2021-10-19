using System;

namespace BonusCalcApi.V1.Boundary.Response
{
    public class BonusPeriodResponse
    {
        public string Id { get; set; }

        public DateTime StartAt { get; set; }

        public int Year { get; set; }

        public int Number { get; set; }

        public DateTime? ClosedAt { get; set; }
    }
}
