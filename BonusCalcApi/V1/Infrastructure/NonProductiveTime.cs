using System;
using System.ComponentModel.DataAnnotations;

namespace BonusCalcApi.V1.Infrastructure
{
    public class NonProductiveTime
    {
        [Key] public Guid Id { get; set; }
        public NonProductiveTimeType Type { get; set; }
        public double StandardMinuteValue { get; set; }
        public double Hours { get; set; }
        public DateTime DateOfWork { get; set; }
        public DateTime DateRecorded { get; set; }
    }

    public enum NonProductiveTimeType
    {
        AnnualLeave,
        Dayworks
    }
}
