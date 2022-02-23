using System.Collections.Generic;

namespace BonusCalcApi.V1.Boundary.Response
{
    public class SchemeResponse
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public decimal ConversionFactor { get; set; }
        public decimal MaxValue { get; set; }
        public List<PayBandResponse> PayBands { get; set; }
    }

    public class PayBandResponse
    {
        public int Band { get; set; }
        public decimal Value { get; set; }
    }
}
