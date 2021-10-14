using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BonusCalcApi.V1.Infrastructure
{
    public class Week
    {
        [Key]
        public string WeekId { get; set; }

        public string BonusPeriodId { get; set; }

        public BonusPeriod BonusPeriod { get; set; }

        public DateTime StartAt { get; set; }

        public int Number { get; set; }

        public DateTime? ClosedAt { get; set; }

        public List<Timesheet> Timesheets { get; set; }
    }
}
