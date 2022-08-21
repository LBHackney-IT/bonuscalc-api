using System.ComponentModel.DataAnnotations;

namespace BonusCalcApi.V1.Infrastructure
{
    public class OperativeProjection
    {
        [StringLength(17)]
        public string Id { get; set; }

        [StringLength(6)]
        public string OperativeId { get; set; }
        public Operative Operative { get; set; }

        [StringLength(10)]
        public string BonusPeriodId { get; set; }
        public BonusPeriod BonusPeriod { get; set; }

        public string Trade { get; set; }

        public string Scheme { get; set; }

        public decimal BandValue { get; set; }

        public decimal MaxValue { get; set; }

        public decimal SickDuration { get; set; }

        public decimal TotalValue { get; set; }

        public decimal Utilisation { get; set; }

        public bool FixedBand { get; set; }

        public int SalaryBand { get; set; }

        public int ProjectedBand { get; set; }

        public string SupervisorName { get; set; }

        public string SupervisorEmailAddress { get; set; }

        public string ManagerName { get; set; }

        public string ManagerEmailAddress { get; set; }

        [StringLength(3)]
        public string RateCode { get; set; }
    }
}
