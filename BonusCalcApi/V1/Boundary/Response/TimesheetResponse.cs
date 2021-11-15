using System.Collections.Generic;

namespace BonusCalcApi.V1.Boundary.Response
{
    public class TimesheetResponse
    {
        public string Id { get; set; }
        public WeekResponse Week { get; set; }
        public List<PayElementResponse> PayElements { get; set; }
    }
}
