using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BonusCalcApi.V1.Infrastructure
{
    public class Summary
    {
        public string Id { get; set; }

        public string OperativeId { get; set; }

        public string BonusPeriodId { get; set; }
        public BonusPeriod BonusPeriod { get; set; }

        public List<WeeklySummary> WeeklySummaries { get; set; }
    }
}
