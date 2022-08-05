using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BonusCalcApi.V1.Infrastructure
{
    [JsonConverter(typeof(StringEnumConverter))]
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

        public BandChangeDecision? Decision { get; set; }

        public string Reason { get; set; }

        public int? SalaryBand { get; set; }
    }

    public class BandChange
    {
        public BandChange()
        {
        }

        public BandChange(OperativeProjection projection)
        {
            Id = projection.Id;
            BonusPeriodId = projection.BonusPeriodId;
            OperativeId = projection.OperativeId;
            Trade = projection.Trade;
            Scheme = projection.Scheme;
            BandValue = projection.BandValue;
            MaxValue = projection.MaxValue;
            SickDuration = projection.SickDuration;
            TotalValue = projection.TotalValue;
            Utilisation = projection.Utilisation;
            FixedBand = projection.FixedBand;
            SalaryBand = projection.SalaryBand;
            ProjectedBand = projection.ProjectedBand;
            Supervisor = new BandChangeApprover
            {
                Name = projection.SupervisorName,
                EmailAddress = projection.SupervisorEmailAddress,
                Decision = null,
                SalaryBand = null
            };
            Manager = new BandChangeApprover
            {
                Name = projection.ManagerName,
                EmailAddress = projection.ManagerEmailAddress,
                Decision = null,
                SalaryBand = null
            };
        }

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

        [Required]
        public BandChangeApprover Supervisor { get; set; }

        [Required]
        public BandChangeApprover Manager { get; set; }

        public int? FinalBand { get; set; }

        public decimal BalanceDuration { get; set; }

        public decimal BalanceValue { get; set; }

        public DateTime? ReportSentAt { get; set; }

        public List<WeeklySummary> WeeklySummaries { get; set; }
    }
}
