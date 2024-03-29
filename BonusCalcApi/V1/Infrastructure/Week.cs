using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BonusCalcApi.V1.Infrastructure
{
    public class Week
    {
        [Key]
        [StringLength(10)]
        public string Id { get; set; }

        [Required]
        [StringLength(10)]
        public string BonusPeriodId { get; set; }

        public BonusPeriod BonusPeriod { get; set; }

        public DateTime StartAt { get; set; }

        public int Number { get; set; }

        public DateTime? ClosedAt { get; set; }

        [StringLength(100)]
        public string ClosedBy { get; set; }

        public DateTime? ReportsSentAt { get; set; }

        public List<Timesheet> Timesheets { get; set; }

        public List<OperativeSummary> OperativeSummaries { get; set; }

        public bool IsOpen => ClosedAt == null;
    }
}
