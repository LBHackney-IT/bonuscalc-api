using System;
using System.Collections.Generic;
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
                .With(o => o.Utilisation, 1.0M)
                .Without(o => o.Timesheets)
                .Without(o => o.WeeklySummaries);
        }

        public static Operative CreateOperative()
        {
            return BuildOperative().Create();
        }

        private static string CreateIsoDateId()
        {
            return $"{Fixture.Create<DateTime>().ToString("yyyy-MM-dd")}";
        }

        public static IPostprocessComposer<Week> BuildWeek(BonusPeriod bonusPeriod = null)
        {
            if (bonusPeriod == null)
                return Fixture.Build<Week>()
                    .With(w => w.Id, CreateIsoDateId())
                    .Without(w => w.Timesheets);
            else
                return Fixture.Build<Week>()
                    .With(w => w.BonusPeriodId, bonusPeriod.Id)
                    .Without(w => w.BonusPeriod)
                    .Without(w => w.Timesheets);
        }

        public static Week CreateWeek(BonusPeriod bonusPeriod = null)
        {
            return BuildWeek(bonusPeriod).Create();
        }

        public static IPostprocessComposer<BonusPeriod> BuildBonusPeriod()
        {
            return Fixture.Build<BonusPeriod>()
                .With(w => w.Id, CreateIsoDateId())
                .Without(bp => bp.Weeks);
        }

        public static BonusPeriod CreateBonusPeriod()
        {
            return BuildBonusPeriod().Create();
        }

        public static List<BonusPeriod> CreateBonusPeriods()
        {
            return new List<BonusPeriod>()
            {
                CreateBonusPeriod(),
                CreateBonusPeriod()
            };
        }

        public static IPostprocessComposer<Summary> BuildSummary()
        {
            return Fixture.Build<Summary>()
                .With(s => s.BonusPeriod, CreateBonusPeriod());
        }

        public static Summary CreateSummary()
        {
            return BuildSummary().Create();
        }
    }
}
