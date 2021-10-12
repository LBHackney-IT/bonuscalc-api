using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BonusCalcApi.V1.Infrastructure
{
    public class Operative
    {
        [Key] public string Id { get; set; }
        public string Name { get; set; }
        public string Trade { get; set; }
        public string Section { get; set; }
        public string Scheme { get; set; }
        public int SalaryBand { get; set; }
        public bool FixedBand { get; set; }
        public bool IsArchived { get; set; }
        public virtual List<NonProductiveTime> NonProductiveTimeList { get; set; }
    }
}
