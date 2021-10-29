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
        public decimal ConversionFactor { get; set; }
        public List<PayBandResponse> PayBands { get; set; }
    }

    public class PayBandResponse
    {
        public int Band { get; set; }
        public decimal Value { get; set; }
    }
}
