using System;
using System.Collections.Generic;

namespace BonusCalcApi.V1.Boundary.Response
{
    public class WeekResponse
    {
        public string Id { get; set; }

        public BonusPeriodResponse BonusPeriod { get; set; }

        public DateTime StartAt { get; set; }

        public int Number { get; set; }

        public DateTime? ClosedAt { get; set; }

        public List<OperativeSummaryResponse> OperativeSummaries { get; set; }

        public bool ShouldSerializeBonusPeriod() => BonusPeriod != null;
        public bool ShouldSerializeOperativeSummaries() => OperativeSummaries != null;
    }
}
