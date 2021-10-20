using System.Threading.Tasks;
using BonusCalcApi.V1.Infrastructure;
using FluentAssertions;

namespace BonusCalcApi.Tests
{
    public class MockDbSaver : IDbSaver
    {
        private bool Saved { get; set; }
        public MockDbSaver()
        {
            Saved = false;
        }

        public void VerifySaveCalled() { Saved.Should().BeTrue(); }
        public void VerifySaveNotCalled() { Saved.Should().BeFalse(); }

        public Task SaveChangesAsync()
        {
            Saved = true;
            return Task.CompletedTask;
        }
    }
}
