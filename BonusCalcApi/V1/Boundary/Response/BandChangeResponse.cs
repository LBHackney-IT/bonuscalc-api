using System;
using System.Collections.Generic;

namespace BonusCalcApi.V1.Boundary.Response
{
    public class BandChangeResponse
    {
        public string Id { get; set; }

        public string OperativeId { get; set; }

        public string OperativeName { get; set; }

        public string EmailAddress { get; set; }

        public BonusPeriodResponse BonusPeriod { get; set; }

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

        public BandChangeApproverResponse Supervisor { get; set; }

        public BandChangeApproverResponse Manager { get; set; }

        public int? FinalBand { get; set; }

        public string RateCode { get; set; }

        public decimal BonusRate { get; set; }

        public DateTime? ReportSentAt { get; set; }

        public List<WeeklySummaryResponse> WeeklySummaries { get; set; }

        public bool ShouldSerializeBonusPeriod() => BonusPeriod != null;

        public bool ShouldSerializeWeeklySummaries() => WeeklySummaries != null;
    }
}
