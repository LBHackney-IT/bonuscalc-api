using System.Collections.Generic;

namespace BonusCalcApi.V1.Boundary.Request
{
    public class TimesheetUpdate
    {
        public string Id { get; set; }
        public List<PayElementUpdate> PayElements { get; set; }
    }

}
