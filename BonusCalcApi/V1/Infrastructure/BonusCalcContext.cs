using Microsoft.EntityFrameworkCore;

namespace BonusCalcApi.V1.Infrastructure
{

    public class BonusCalcContext : DbContext
    {
        public BonusCalcContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<BonusPeriod> BonusPeriods { get; set; }
        public DbSet<Operative> Operatives { get; set; }
        public DbSet<PayBand> PayBands { get; set; }
        public DbSet<PayElement> PayElements { get; set; }
        public DbSet<PayElementType> PayElementTypes { get; set; }
        public DbSet<Timesheet> Timesheets { get; set; }
        public DbSet<Trade> Trades { get; set; }
        public DbSet<Week> Weeks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BonusPeriod>()
                .HasIndex(bp => bp.StartAt)
                .IsUnique();

            modelBuilder.Entity<BonusPeriod>()
                .HasIndex(bp => new { bp.Year, bp.Period })
                .IsUnique();

            modelBuilder.Entity<Operative>()
                .HasOne(o => o.Trade)
                .WithMany(t => t.Operatives)
                .HasForeignKey(o => o.TradeId);

            modelBuilder.Entity<PayBand>()
                .HasIndex(pb => new { pb.TradeId, pb.Band })
                .IsUnique();

            modelBuilder.Entity<PayBand>()
                .HasOne(pb => pb.Trade)
                .WithMany(t => t.PayBands)
                .HasForeignKey(pb => pb.TradeId);

            modelBuilder.Entity<PayElement>()
                .HasOne(pe => pe.Timesheet)
                .WithMany(t => t.PayElements)
                .HasForeignKey(t => t.TimesheetId);

            modelBuilder.Entity<PayElement>()
                .HasOne(pe => pe.PayElementType)
                .WithMany(pet => pet.PayElements)
                .HasForeignKey(pe => pe.PayElementTypeId);

            modelBuilder.Entity<PayElement>()
                .Property(pe => pe.Duration)
                .HasPrecision(10, 4);

            modelBuilder.Entity<PayElement>()
                .Property(pe => pe.Value)
                .HasPrecision(10, 4);

            modelBuilder.Entity<PayElementType>()
                .HasIndex(pet => pet.Description)
                .IsUnique();

            modelBuilder.Entity<PayElementType>()
                .Property(pet => pet.Productive)
                .HasDefaultValue(false);

            modelBuilder.Entity<PayElementType>()
                .Property(pet => pet.Adjustment)
                .HasDefaultValue(false);

            modelBuilder.Entity<Timesheet>()
                .HasIndex(t => new { t.OperativeId, t.WeekId })
                .IsUnique();

            modelBuilder.Entity<Timesheet>()
                .HasOne(t => t.Operative)
                .WithMany(o => o.Timesheets)
                .HasForeignKey(t => t.OperativeId);

            modelBuilder.Entity<Timesheet>()
                .HasOne(t => t.Week)
                .WithMany(w => w.Timesheets)
                .HasForeignKey(t => t.WeekId);

            modelBuilder.Entity<Trade>()
                .HasIndex(t => t.Description)
                .IsUnique();

            modelBuilder.Entity<Week>()
                .HasIndex(w => new { w.BonusPeriodId, w.Number })
                .IsUnique();

            modelBuilder.Entity<Week>()
                .HasOne(w => w.BonusPeriod)
                .WithMany(bp => bp.Weeks)
                .HasForeignKey(w => w.BonusPeriodId);
        }
    }
}
