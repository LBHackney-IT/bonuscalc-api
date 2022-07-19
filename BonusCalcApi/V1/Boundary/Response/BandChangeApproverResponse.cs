using BonusCalcApi.V1.Infrastructure;

namespace BonusCalcApi.V1.Boundary.Response
{
    public class BandChangeApproverResponse
    {
        public string Name { get; set; }

        public string EmailAddress { get; set; }

        public BandChangeDecision? Decision { get; set; }

        public string Reason { get; set; }

        public int? SalaryBand { get; set; }
    }
}
