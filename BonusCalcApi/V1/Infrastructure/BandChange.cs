using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BonusCalcApi.V1.Infrastructure
{
    public enum BandChangeDecision
    {
        Approved,
        Rejected
    }

    [Owned]
    public class BandChangeApprover
    {
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(100)]
        public string EmailAddress { get; set; }

        public BandChangeDecision Decision { get; set; }

        public string Reason { get; set; }

        public int SalaryBand { get; set; }
    }

    public class BandChange
    {
        [Key]
        [StringLength(17)]
        public string Id { get; set; }

        [Required]
        [StringLength(6)]
        public string OperativeId { get; set; }
        public Operative Operative { get; set; }

        [Required]
        [StringLength(10)]
        public string BonusPeriodId { get; set; }
        public BonusPeriod BonusPeriod { get; set; }

        [Required]
        [StringLength(100)]
        public string Trade { get; set; }

        [Required]
        [StringLength(100)]
        public string Scheme { get; set; }

        public decimal BandValue { get; set; }

        public decimal MaxValue { get; set; }

        public decimal SickDuration { get; set; }

        public decimal TotalValue { get; set; }

        public decimal Utilisation { get; set; }

        public bool FixedBand { get; set; }

        public int SalaryBand { get; set; }

        public int ProjectedBand { get; set; }

        public BandChangeApprover Supervisor { get; set; }

        public BandChangeApprover Manager { get; set; }

        public int? FinalBand { get; set; }

        public decimal BalanceDuration { get; set; }

        public decimal BalanceValue { get; set; }
    }
}
