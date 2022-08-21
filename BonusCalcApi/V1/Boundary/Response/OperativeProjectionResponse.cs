using System;
using System.Collections.Generic;

namespace BonusCalcApi.V1.Boundary.Response
{
    public class OperativeProjectionResponse
    {
        public string Id { get; set; }

        public string OperativeId { get; set; }

        public string OperativeName { get; set; }

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

        public string RateCode { get; set; }
    }
}
