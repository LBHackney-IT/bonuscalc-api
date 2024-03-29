using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NpgsqlTypes;

namespace BonusCalcApi.V1.Infrastructure
{
    public class Operative
    {
        [Key]
        [StringLength(6)]
        public string Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(100)]
        public string EmailAddress { get; set; }

        [StringLength(6)]
        public string SupervisorId { get; set; }
        public Person Supervisor { get; set; }

        [StringLength(6)]
        public string ManagerId { get; set; }
        public Person Manager { get; set; }

        [Required]
        [StringLength(3)]
        public string TradeId { get; set; }
        public Trade Trade { get; set; }

        public int? SchemeId { get; set; }
        public Scheme Scheme { get; set; }

        [Required]
        [StringLength(10)]
        public string Section { get; set; }

        public int SalaryBand { get; set; }

        public decimal Utilisation { get; set; }

        public bool FixedBand { get; set; }

        public bool IsArchived { get; set; }

        public NpgsqlTsVector SearchVector { get; set; }

        public List<Timesheet> Timesheets { get; set; }

        public List<WeeklySummary> WeeklySummaries { get; set; }
    }
}
