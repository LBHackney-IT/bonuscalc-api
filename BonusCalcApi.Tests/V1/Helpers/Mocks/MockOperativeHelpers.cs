using BonusCalcApi.V1.Controllers.Helpers;
using Moq;

namespace BonusCalcApi.Tests.V1.Helpers.Mocks
{
    public class MockOperativeHelpers : Mock<IOperativeHelpers>
    {
        public void ValidPrn(bool valid)
        {
            Setup(x => x.IsValidPrn(It.IsAny<string>()))
                .Returns(valid);
        }
    }
}
