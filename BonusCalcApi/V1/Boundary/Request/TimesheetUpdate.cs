using System.Collections.Generic;

namespace BonusCalcApi.V1.Boundary.Request
{
    public class TimesheetUpdateRequest
    {
        public int Id { get; set; }
        public List<PayElementUpdate> PayElements { get; set; }
    }

}
