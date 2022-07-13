using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace BonusCalcApi.V1.Infrastructure
{
    public class BonusCalcContext : DbContext
    {
        static BonusCalcContext()
            => NpgsqlConnection.GlobalTypeMapper.MapEnum<BandChangeDecision>();

        public BonusCalcContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<BandChange> BandChanges { get; set; }
        public DbSet<BonusPeriod> BonusPeriods { get; set; }
        public DbSet<Operative> Operatives { get; set; }
        public DbSet<OperativeProjection> OperativeProjections { get; set; }
        public DbSet<OperativeSummary> OperativeSummaries { get; set; }
        public DbSet<OutOfHoursSummary> OutOfHoursSummaries { get; set; }
        public DbSet<OvertimeSummary> OvertimeSummaries { get; set; }
        public DbSet<PayBand> PayBands { get; set; }
        public DbSet<PayElement> PayElements { get; set; }
        public DbSet<PayElementType> PayElementTypes { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<Scheme> Schemes { get; set; }
        public DbSet<Timesheet> Timesheets { get; set; }
        public DbSet<Trade> Trades { get; set; }
        public DbSet<Week> Weeks { get; set; }
        public DbSet<Summary> Summaries { get; set; }
        public DbSet<WeeklySummary> WeeklySummaries { get; set; }
        public DbSet<WorkElement> WorkElements { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseNpgsql(o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresEnum<BandChangeDecision>();

            modelBuilder.Entity<BandChange>()
                .Property(bc => bc.MaxValue)
                .HasPrecision(10, 4);

            modelBuilder.Entity<BandChange>()
                .Property(bc => bc.BandValue)
                .HasPrecision(10, 4);

            modelBuilder.Entity<BandChange>()
                .Property(bc => bc.SickDuration)
                .HasPrecision(10, 4);

            modelBuilder.Entity<BandChange>()
                .Property(bc => bc.TotalValue)
                .HasPrecision(10, 4);

            modelBuilder.Entity<BandChange>()
                .Property(bc => bc.Utilisation)
                .HasPrecision(10, 4);

            modelBuilder.Entity<BandChange>()
                .OwnsOne(bc => bc.Supervisor);

            modelBuilder.Entity<BandChange>()
                .OwnsOne(bc => bc.Manager);

            modelBuilder.Entity<BandChange>()
                .Property(pb => pb.BalanceDuration)
                .HasPrecision(10, 4)
                .HasComputedColumnSql(@"ROUND(GREATEST(LEAST(max_value * utilisation, total_value * (NOT fixed_band)::int) -  band_value * utilisation, 0) / 60, 4)", stored: true);

            modelBuilder.Entity<BandChange>()
                .Property(pb => pb.BalanceValue)
                .HasPrecision(10, 4)
                .HasComputedColumnSql(@"GREATEST(LEAST(max_value * utilisation, total_value * (NOT fixed_band)::int) -  band_value * utilisation, 0)", stored: true);

            modelBuilder.Entity<BonusPeriod>()
                .HasIndex(bp => bp.StartAt)
                .IsUnique();

            modelBuilder.Entity<BonusPeriod>()
                .HasIndex(bp => new { bp.Year, bp.Number })
                .IsUnique();

            modelBuilder.Entity<Operative>()
                .HasIndex(o => o.TradeId);

            modelBuilder.Entity<Operative>()
                .HasOne(o => o.Trade)
                .WithMany(t => t.Operatives)
                .HasForeignKey(o => o.TradeId);

            modelBuilder.Entity<Operative>()
                .HasIndex(o => o.SchemeId);

            modelBuilder.Entity<Operative>()
                .HasOne(o => o.Scheme)
                .WithMany(s => s.Operatives)
                .HasForeignKey(o => o.SchemeId);

            modelBuilder.Entity<Operative>()
                .HasIndex(o => o.EmailAddress)
                .IsUnique();

            modelBuilder.Entity<Operative>()
                .Property(o => o.Utilisation)
                .HasPrecision(5, 4)
                .HasDefaultValue(1.0);

            modelBuilder.Entity<Operative>()
                .HasGeneratedTsVectorColumn(
                    o => o.SearchVector, "simple",
                    o => new { o.Id, o.Name, o.TradeId, o.Section })
                .HasIndex(pe => pe.SearchVector)
                .HasMethod("GIN");

            modelBuilder.Entity<OperativeProjection>()
                .ToView("operative_projections")
                .HasKey(op => op.Id);

            modelBuilder.Entity<OperativeSummary>()
                .ToView("operative_summaries")
                .HasKey(os => new { os.Id, os.WeekId });

            modelBuilder.Entity<OutOfHoursSummary>()
                .ToView("out_of_hours_summaries")
                .HasKey(os => new { os.Id, os.WeekId, os.CostCode });

            modelBuilder.Entity<OvertimeSummary>()
                .ToView("overtime_summaries")
                .HasKey(os => new { os.Id, os.WeekId, os.CostCode });

            modelBuilder.Entity<PayBand>()
                .Property(pb => pb.Id)
                .ValueGeneratedNever();

            modelBuilder.Entity<PayBand>()
                .HasIndex(pb => pb.SchemeId);

            modelBuilder.Entity<PayBand>()
                .HasOne(pb => pb.Scheme)
                .WithMany(s => s.PayBands)
                .HasForeignKey(pb => pb.SchemeId);

            modelBuilder.Entity<PayBand>()
                .Property(pb => pb.TotalValue)
                .HasPrecision(10, 4)
                .HasComputedColumnSql(@"value * 13", stored: true);

            modelBuilder.Entity<PayBand>()
                .Property(pb => pb.SmvPerHour)
                .HasPrecision(20, 14)
                .HasComputedColumnSql(@"value / 36", stored: true);

            modelBuilder.Entity<PayElement>()
                .HasIndex(pe => pe.TimesheetId);

            modelBuilder.Entity<PayElement>()
                .HasIndex(pe => pe.WorkOrder);

            modelBuilder.Entity<PayElement>()
                .HasIndex(pe => pe.CostCode);

            modelBuilder.Entity<PayElement>()
                .HasOne(pe => pe.Timesheet)
                .WithMany(t => t.PayElements)
                .HasForeignKey(t => t.TimesheetId);

            modelBuilder.Entity<PayElement>()
                .HasIndex(pe => pe.PayElementTypeId);

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

            modelBuilder.Entity<PayElement>()
                .Property(pe => pe.Monday)
                .HasPrecision(10, 4)
                .HasDefaultValue(0.0);

            modelBuilder.Entity<PayElement>()
                .Property(pe => pe.Tuesday)
                .HasPrecision(10, 4)
                .HasDefaultValue(0.0);

            modelBuilder.Entity<PayElement>()
                .Property(pe => pe.Wednesday)
                .HasPrecision(10, 4)
                .HasDefaultValue(0.0);

            modelBuilder.Entity<PayElement>()
                .Property(pe => pe.Thursday)
                .HasPrecision(10, 4)
                .HasDefaultValue(0.0);

            modelBuilder.Entity<PayElement>()
                .Property(pe => pe.Friday)
                .HasPrecision(10, 4)
                .HasDefaultValue(0.0);

            modelBuilder.Entity<PayElement>()
                .Property(pe => pe.Saturday)
                .HasPrecision(10, 4)
                .HasDefaultValue(0.0);

            modelBuilder.Entity<PayElement>()
                .Property(pe => pe.Sunday)
                .HasPrecision(10, 4)
                .HasDefaultValue(0.0);

            modelBuilder.Entity<PayElement>()
                .HasGeneratedTsVectorColumn(
                    pe => pe.SearchVector, "simple",
                    pe => new { pe.WorkOrder, pe.Address })
                .HasIndex(pe => pe.SearchVector)
                .HasMethod("GIN");

            modelBuilder.Entity<PayElementType>()
                .Property(pet => pet.Id)
                .ValueGeneratedNever();

            modelBuilder.Entity<PayElementType>()
                .HasIndex(pet => pet.Description)
                .IsUnique();

            modelBuilder.Entity<PayElementType>()
                .Property(pet => pet.NonProductive)
                .HasDefaultValue(false);

            modelBuilder.Entity<PayElementType>()
                .Property(pet => pet.Productive)
                .HasDefaultValue(false);

            modelBuilder.Entity<PayElementType>()
                .Property(pet => pet.Adjustment)
                .HasDefaultValue(false);

            modelBuilder.Entity<PayElementType>()
                .Property(pet => pet.OutOfHours)
                .HasDefaultValue(false);

            modelBuilder.Entity<PayElementType>()
                .Property(pet => pet.Overtime)
                .HasDefaultValue(false);

            modelBuilder.Entity<PayElementType>()
                .Property(pet => pet.SickLeave)
                .HasDefaultValue(false);

            modelBuilder.Entity<PayElementType>()
                .Property(pet => pet.Selectable)
                .HasDefaultValue(false);

            modelBuilder.Entity<Scheme>()
                .Property(s => s.Id)
                .ValueGeneratedNever();

            modelBuilder.Entity<Scheme>()
                .HasIndex(s => s.Description)
                .IsUnique();

            modelBuilder.Entity<Scheme>()
                .Property(s => s.ConversionFactor)
                .HasPrecision(20, 14)
                .HasDefaultValue(1.0);

            modelBuilder.Entity<Scheme>()
                .Property(s => s.MinValue)
                .HasPrecision(10, 4)
                .HasDefaultValue(0.0);

            modelBuilder.Entity<Scheme>()
                .Property(s => s.MaxValue)
                .HasPrecision(10, 4)
                .HasDefaultValue(0.0);

            modelBuilder.Entity<Timesheet>()
                .Property(t => t.Id)
                .ValueGeneratedNever();

            modelBuilder.Entity<Timesheet>()
                .HasIndex(t => new { t.OperativeId, t.WeekId })
                .IsUnique();

            modelBuilder.Entity<Timesheet>()
                .HasIndex(t => t.WeekId);

            modelBuilder.Entity<Timesheet>()
                .HasOne(t => t.Operative)
                .WithMany(o => o.Timesheets)
                .HasForeignKey(t => t.OperativeId);

            modelBuilder.Entity<Timesheet>()
                .HasOne(t => t.Week)
                .WithMany(w => w.Timesheets)
                .HasForeignKey(t => t.WeekId);

            modelBuilder.Entity<Timesheet>()
                .Property(t => t.Utilisation)
                .HasPrecision(5, 4)
                .HasDefaultValue(1.0);

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

            modelBuilder.Entity<Summary>()
                .ToView("summaries")
                .HasKey(s => s.Id);

            modelBuilder.Entity<WeeklySummary>()
                .ToView("weekly_summaries")
                .HasKey(ws => ws.Id);

            modelBuilder.Entity<WorkElement>()
                .ToView("work_elements")
                .HasKey(wo => wo.Id);

            modelBuilder.Entity<WorkElement>()
                .Property(we => we.Id)
                .ValueGeneratedNever();
        }
    }
}
