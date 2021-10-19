using System;
using System.Collections.Generic;
using System.Linq;

namespace BonusCalcApi.Tests.V1.Helpers
{
    public static class EnumerableExtensions
    {
        public static T GetRandom<T>(this IEnumerable<T> enumerable)
        {
            var rand = new Random();
            var index = rand.Next(enumerable.Count());
            return enumerable.ElementAt(index);
        }
    }
}
