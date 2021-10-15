using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BonusCalcApi.V1.Infrastructure
{
    public class BonusPeriod
    {
        [Key]
        public string BonusPeriodId { get; set; }

        public DateTime StartAt { get; set; }

        public int Year { get; set; }

        public int Period { get; set; }

        public DateTime? ClosedAt { get; set; }

        public List<Week> Weeks { get; set; }
    }
}
