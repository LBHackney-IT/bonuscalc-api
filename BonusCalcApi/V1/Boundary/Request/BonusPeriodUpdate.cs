using System;

namespace BonusCalcApi.V1.Boundary.Request
{
    public class BonusPeriodUpdate
    {
        public DateTime ClosedAt { get; set; }
        public string ClosedBy { get; set; }
    }
}
