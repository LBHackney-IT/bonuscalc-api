using AutoFixture;
using AutoFixture.Dsl;
using BonusCalcApi.V1.Infrastructure;

namespace BonusCalcApi.Tests.V1.Helpers
{
    public static class FixtureHelpers
    {
        public static Fixture Fixture => CreateFixture();
        private static Fixture CreateFixture()
        {
            var fixture = new Fixture();
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            return fixture;
        }

        private static string CreateOperativeId()
        {
            return $"{Fixture.Create<int>():D6}";
        }

        public static IPostprocessComposer<Operative> BuildOperative()
        {
            return Fixture.Build<Operative>()
                .With(o => o.Id, CreateOperativeId())
                .Without(o => o.Timesheets);
        }

        public static Operative CreateOperative()
        {
            return BuildOperative().Create();
        }
    }
}
