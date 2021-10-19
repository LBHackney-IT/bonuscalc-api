using System;
using BonusCalcApi.V1.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;

namespace BonusCalcApi.Tests
{
    public static class InMemoryDb
    {
        private static BonusCalcContext _context;
        private static MockDbSaver _dbSaver;

        public static BonusCalcContext Instance
        {
            get
            {
                if (_context == null)
                {
                    DbContextOptionsBuilder<BonusCalcContext> builder = new DbContextOptionsBuilder<BonusCalcContext>();
                    builder.EnableSensitiveDataLogging();
                    builder.ConfigureWarnings(options =>
                    {
                        options.Ignore(InMemoryEventId.TransactionIgnoredWarning);
                    });
                    builder.UseInMemoryDatabase(Guid.NewGuid().ToString());

                    _context = new BonusCalcContext(builder.Options);
                    _context.Database.EnsureCreated();
                }

                return _context;
            }
        }

        public static MockDbSaver DbSaver => _dbSaver ??= new MockDbSaver();

        public static void Teardown()
        {
            _context = null;
            _dbSaver = null;
        }
    }

}
