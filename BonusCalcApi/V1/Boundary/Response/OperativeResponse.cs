using System.Collections.Generic;

namespace BonusCalcApi.V1.Boundary.Response
{
    public class OperativeResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public TradeResponse Trade { get; set; }
        public SchemeResponse Scheme { get; set; }
        public string Section { get; set; }
        public int SalaryBand { get; set; }
        public bool FixedBand { get; set; }
        public bool IsArchived { get; set; }
    }

    public class TradeResponse
    {
        public string Id { get; set; }
        public string Description { get; set; }
    }

    public class SchemeResponse
    {
        public string Type { get; set; }
        public string Description { get; set; }
    }
}
