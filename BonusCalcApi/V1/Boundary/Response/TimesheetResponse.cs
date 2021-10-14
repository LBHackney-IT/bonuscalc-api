using System;
using System.Collections.Generic;

namespace BonusCalcApi.V1.Boundary.Response
{
    public class TimesheetResponse
    {
        public int TimesheetId { get; set; }
        public WeekResponse Week { get; set; }
        public List<PayElementResponse> PayElements { get; set; }
    }

    public class PayElementResponse
    {
        public int PayElementId { get; set; }

        public int PayElementTypeId { get; set; }

        public int WeekDay { get; set; }

        public string WorkOrder { get; set; }

        public string Address { get; set; }

        public string Comment { get; set; }

        public bool Productive { get; set; }

        public decimal Duration { get; set; }

        public decimal Value { get; set; }
    }

    public class WeekResponse
    {
        public string WeekId { get; set; }

        public BonusPeriodResponse BonusPeriod { get; set; }

        public DateTime StartAt { get; set; }

        public int Number { get; set; }

        public DateTime? ClosedAt { get; set; }
    }

    public class BonusPeriodResponse
    {
        public string BonusPeriodId { get; set; }

        public DateTime StartAt { get; set; }

        public int Year { get; set; }

        public int Period { get; set; }

        public DateTime? ClosedAt { get; set; }
    }
}
