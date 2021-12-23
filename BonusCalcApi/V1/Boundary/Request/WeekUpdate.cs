using System;

namespace BonusCalcApi.V1.Boundary.Request
{
    public class WeekUpdate
    {
        public DateTime? ClosedAt { get; set; }
        public string ClosedBy { get; set; }
    }
}
