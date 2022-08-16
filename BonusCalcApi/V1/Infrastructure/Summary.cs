using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BonusCalcApi.V1.Infrastructure
{
    public class Summary
    {
        [StringLength(17)]
        public string Id { get; set; }

        [StringLength(6)]
        public string OperativeId { get; set; }

        [StringLength(10)]
        public string BonusPeriodId { get; set; }
        public BonusPeriod BonusPeriod { get; set; }

        public List<WeeklySummary> WeeklySummaries { get; set; }
    }
}
