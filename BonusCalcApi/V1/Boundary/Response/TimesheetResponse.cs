using System.Collections.Generic;

namespace BonusCalcApi.V1.Boundary.Response
{
    public class TimesheetResponse
    {
        public int Id { get; set; }
        public WeekResponse Week { get; set; }
        public List<PayElementResponse> PayElements { get; set; }
    }

}
