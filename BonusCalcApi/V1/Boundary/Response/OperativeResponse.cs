using System.Collections.Generic;

namespace BonusCalcApi.V1.Boundary.Response
{
    public class OperativeResponse
    {
        public int Id { get; set; }
        public string PayrollNumber { get; set; }
        public string Name { get; set; }
        public List<string> Trades { get; set; }
    }
}
