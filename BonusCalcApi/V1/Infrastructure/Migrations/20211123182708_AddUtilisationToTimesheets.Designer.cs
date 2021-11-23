﻿// <auto-generated />
using System;
using BonusCalcApi.V1.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace V1.Infrastructure.Migrations
{
    [DbContext(typeof(BonusCalcContext))]
    [Migration("20211123182708_AddUtilisationToTimesheets")]
    partial class AddUtilisationToTimesheets
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.10")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("BonusCalcApi.V1.Infrastructure.BonusPeriod", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<DateTime?>("ClosedAt")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("closed_at");

                    b.Property<int>("Number")
                        .HasColumnType("integer")
                        .HasColumnName("number");

                    b.Property<DateTime>("StartAt")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("start_at");

                    b.Property<int>("Year")
                        .HasColumnType("integer")
                        .HasColumnName("year");

                    b.HasKey("Id")
                        .HasName("pk_bonus_periods");

                    b.HasIndex("StartAt")
                        .IsUnique()
                        .HasDatabaseName("ix_bonus_periods_start_at");

                    b.HasIndex("Year", "Number")
                        .IsUnique()
                        .HasDatabaseName("ix_bonus_periods_year_number");

                    b.ToTable("bonus_periods");
                });

            modelBuilder.Entity("BonusCalcApi.V1.Infrastructure.Operative", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(6)
                        .HasColumnType("character varying(6)")
                        .HasColumnName("id");

                    b.Property<string>("EmailAddress")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("email_address");

                    b.Property<bool>("FixedBand")
                        .HasColumnType("boolean")
                        .HasColumnName("fixed_band");

                    b.Property<bool>("IsArchived")
                        .HasColumnType("boolean")
                        .HasColumnName("is_archived");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("name");

                    b.Property<int>("SalaryBand")
                        .HasColumnType("integer")
                        .HasColumnName("salary_band");

                    b.Property<int?>("SchemeId")
                        .HasColumnType("integer")
                        .HasColumnName("scheme_id");

                    b.Property<string>("Section")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("section");

                    b.Property<string>("TradeId")
                        .IsRequired()
                        .HasMaxLength(3)
                        .HasColumnType("character varying(3)")
                        .HasColumnName("trade_id");

                    b.Property<decimal>("Utilisation")
                        .ValueGeneratedOnAdd()
                        .HasPrecision(5, 4)
                        .HasColumnType("numeric(5,4)")
                        .HasDefaultValue(1m)
                        .HasColumnName("utilisation");

                    b.HasKey("Id")
                        .HasName("pk_operatives");

                    b.HasIndex("EmailAddress")
                        .IsUnique()
                        .HasDatabaseName("ix_operatives_email_address");

                    b.HasIndex("SchemeId")
                        .HasDatabaseName("ix_operatives_scheme_id");

                    b.HasIndex("TradeId")
                        .HasDatabaseName("ix_operatives_trade_id");

                    b.ToTable("operatives");
                });

            modelBuilder.Entity("BonusCalcApi.V1.Infrastructure.PayBand", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    b.Property<int>("Band")
                        .HasColumnType("integer")
                        .HasColumnName("band");

                    b.Property<int?>("SchemeId")
                        .HasColumnType("integer")
                        .HasColumnName("scheme_id");

                    b.Property<decimal>("Value")
                        .HasColumnType("numeric")
                        .HasColumnName("value");

                    b.HasKey("Id")
                        .HasName("pk_pay_bands");

                    b.HasIndex("SchemeId")
                        .HasDatabaseName("ix_pay_bands_scheme_id");

                    b.ToTable("pay_bands");
                });

            modelBuilder.Entity("BonusCalcApi.V1.Infrastructure.PayElement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Address")
                        .HasColumnType("text")
                        .HasColumnName("address");

                    b.Property<DateTime?>("ClosedAt")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("closed_at");

                    b.Property<string>("Comment")
                        .HasColumnType("text")
                        .HasColumnName("comment");

                    b.Property<decimal>("Duration")
                        .HasPrecision(10, 4)
                        .HasColumnType("numeric(10,4)")
                        .HasColumnName("duration");

                    b.Property<decimal>("Friday")
                        .ValueGeneratedOnAdd()
                        .HasPrecision(10, 4)
                        .HasColumnType("numeric(10,4)")
                        .HasDefaultValue(0m)
                        .HasColumnName("friday");

                    b.Property<decimal>("Monday")
                        .ValueGeneratedOnAdd()
                        .HasPrecision(10, 4)
                        .HasColumnType("numeric(10,4)")
                        .HasDefaultValue(0m)
                        .HasColumnName("monday");

                    b.Property<int>("PayElementTypeId")
                        .HasColumnType("integer")
                        .HasColumnName("pay_element_type_id");

                    b.Property<bool>("ReadOnly")
                        .HasColumnType("boolean")
                        .HasColumnName("read_only");

                    b.Property<decimal>("Saturday")
                        .ValueGeneratedOnAdd()
                        .HasPrecision(10, 4)
                        .HasColumnType("numeric(10,4)")
                        .HasDefaultValue(0m)
                        .HasColumnName("saturday");

                    b.Property<decimal>("Sunday")
                        .ValueGeneratedOnAdd()
                        .HasPrecision(10, 4)
                        .HasColumnType("numeric(10,4)")
                        .HasDefaultValue(0m)
                        .HasColumnName("sunday");

                    b.Property<decimal>("Thursday")
                        .ValueGeneratedOnAdd()
                        .HasPrecision(10, 4)
                        .HasColumnType("numeric(10,4)")
                        .HasDefaultValue(0m)
                        .HasColumnName("thursday");

                    b.Property<string>("TimesheetId")
                        .IsRequired()
                        .HasMaxLength(17)
                        .HasColumnType("character varying(17)")
                        .HasColumnName("timesheet_id");

                    b.Property<decimal>("Tuesday")
                        .ValueGeneratedOnAdd()
                        .HasPrecision(10, 4)
                        .HasColumnType("numeric(10,4)")
                        .HasDefaultValue(0m)
                        .HasColumnName("tuesday");

                    b.Property<decimal>("Value")
                        .HasPrecision(10, 4)
                        .HasColumnType("numeric(10,4)")
                        .HasColumnName("value");

                    b.Property<decimal>("Wednesday")
                        .ValueGeneratedOnAdd()
                        .HasPrecision(10, 4)
                        .HasColumnType("numeric(10,4)")
                        .HasDefaultValue(0m)
                        .HasColumnName("wednesday");

                    b.Property<string>("WorkOrder")
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("work_order");

                    b.HasKey("Id")
                        .HasName("pk_pay_elements");

                    b.HasIndex("PayElementTypeId")
                        .HasDatabaseName("ix_pay_elements_pay_element_type_id");

                    b.HasIndex("TimesheetId")
                        .HasDatabaseName("ix_pay_elements_timesheet_id");

                    b.ToTable("pay_elements");
                });

            modelBuilder.Entity("BonusCalcApi.V1.Infrastructure.PayElementType", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    b.Property<bool>("Adjustment")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false)
                        .HasColumnName("adjustment");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("description");

                    b.Property<bool>("NonProductive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false)
                        .HasColumnName("non_productive");

                    b.Property<bool>("OutOfHours")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false)
                        .HasColumnName("out_of_hours");

                    b.Property<bool>("Overtime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false)
                        .HasColumnName("overtime");

                    b.Property<bool>("Paid")
                        .HasColumnType("boolean")
                        .HasColumnName("paid");

                    b.Property<bool>("PayAtBand")
                        .HasColumnType("boolean")
                        .HasColumnName("pay_at_band");

                    b.Property<bool>("Productive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false)
                        .HasColumnName("productive");

                    b.Property<bool>("Selectable")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false)
                        .HasColumnName("selectable");

                    b.HasKey("Id")
                        .HasName("pk_pay_element_types");

                    b.HasIndex("Description")
                        .IsUnique()
                        .HasDatabaseName("ix_pay_element_types_description");

                    b.ToTable("pay_element_types");
                });

            modelBuilder.Entity("BonusCalcApi.V1.Infrastructure.Scheme", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    b.Property<decimal>("ConversionFactor")
                        .ValueGeneratedOnAdd()
                        .HasPrecision(20, 14)
                        .HasColumnType("numeric(20,14)")
                        .HasDefaultValue(1m)
                        .HasColumnName("conversion_factor");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("description");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("type");

                    b.HasKey("Id")
                        .HasName("pk_schemes");

                    b.HasIndex("Description")
                        .IsUnique()
                        .HasDatabaseName("ix_schemes_description");

                    b.ToTable("schemes");
                });

            modelBuilder.Entity("BonusCalcApi.V1.Infrastructure.Summary", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<string>("BonusPeriodId")
                        .HasColumnType("text")
                        .HasColumnName("bonus_period_id");

                    b.Property<string>("OperativeId")
                        .HasColumnType("text")
                        .HasColumnName("operative_id");

                    b.HasKey("Id")
                        .HasName("pk_summaries");

                    b.HasIndex("BonusPeriodId")
                        .HasDatabaseName("ix_summaries_bonus_period_id");

                    b.ToView("summaries");
                });

            modelBuilder.Entity("BonusCalcApi.V1.Infrastructure.Timesheet", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(17)
                        .HasColumnType("character varying(17)")
                        .HasColumnName("id");

                    b.Property<string>("OperativeId")
                        .IsRequired()
                        .HasMaxLength(6)
                        .HasColumnType("character varying(6)")
                        .HasColumnName("operative_id");

                    b.Property<decimal>("Utilisation")
                        .ValueGeneratedOnAdd()
                        .HasPrecision(5, 4)
                        .HasColumnType("numeric(5,4)")
                        .HasDefaultValue(1m)
                        .HasColumnName("utilisation");

                    b.Property<string>("WeekId")
                        .HasColumnType("text")
                        .HasColumnName("week_id");

                    b.HasKey("Id")
                        .HasName("pk_timesheets");

                    b.HasIndex("WeekId")
                        .HasDatabaseName("ix_timesheets_week_id");

                    b.HasIndex("OperativeId", "WeekId")
                        .IsUnique()
                        .HasDatabaseName("ix_timesheets_operative_id_week_id");

                    b.ToTable("timesheets");
                });

            modelBuilder.Entity("BonusCalcApi.V1.Infrastructure.Trade", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(3)
                        .HasColumnType("character varying(3)")
                        .HasColumnName("id");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("description");

                    b.HasKey("Id")
                        .HasName("pk_trades");

                    b.HasIndex("Description")
                        .IsUnique()
                        .HasDatabaseName("ix_trades_description");

                    b.ToTable("trades");
                });

            modelBuilder.Entity("BonusCalcApi.V1.Infrastructure.Week", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<string>("BonusPeriodId")
                        .HasColumnType("text")
                        .HasColumnName("bonus_period_id");

                    b.Property<DateTime?>("ClosedAt")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("closed_at");

                    b.Property<int>("Number")
                        .HasColumnType("integer")
                        .HasColumnName("number");

                    b.Property<DateTime>("StartAt")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("start_at");

                    b.HasKey("Id")
                        .HasName("pk_weeks");

                    b.HasIndex("BonusPeriodId", "Number")
                        .IsUnique()
                        .HasDatabaseName("ix_weeks_bonus_period_id_number");

                    b.ToTable("weeks");
                });

            modelBuilder.Entity("BonusCalcApi.V1.Infrastructure.WeeklySummary", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<DateTime?>("ClosedAt")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("closed_at");

                    b.Property<decimal>("NonProductiveDuration")
                        .HasColumnType("numeric")
                        .HasColumnName("non_productive_duration");

                    b.Property<decimal>("NonProductiveValue")
                        .HasColumnType("numeric")
                        .HasColumnName("non_productive_value");

                    b.Property<int>("Number")
                        .HasColumnType("integer")
                        .HasColumnName("number");

                    b.Property<decimal>("ProductiveValue")
                        .HasColumnType("numeric")
                        .HasColumnName("productive_value");

                    b.Property<decimal>("ProjectedValue")
                        .HasColumnType("numeric")
                        .HasColumnName("projected_value");

                    b.Property<DateTime>("StartAt")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("start_at");

                    b.Property<string>("SummaryId")
                        .HasColumnType("text")
                        .HasColumnName("summary_id");

                    b.Property<decimal>("TotalValue")
                        .HasColumnType("numeric")
                        .HasColumnName("total_value");

                    b.HasKey("Id")
                        .HasName("pk_weekly_summaries");

                    b.HasIndex("SummaryId")
                        .HasDatabaseName("ix_weekly_summaries_summary_id");

                    b.ToView("weekly_summaries");
                });

            modelBuilder.Entity("BonusCalcApi.V1.Infrastructure.Operative", b =>
                {
                    b.HasOne("BonusCalcApi.V1.Infrastructure.Scheme", "Scheme")
                        .WithMany("Operatives")
                        .HasForeignKey("SchemeId")
                        .HasConstraintName("fk_operatives_schemes_scheme_id");

                    b.HasOne("BonusCalcApi.V1.Infrastructure.Trade", "Trade")
                        .WithMany("Operatives")
                        .HasForeignKey("TradeId")
                        .HasConstraintName("fk_operatives_trades_trade_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Scheme");

                    b.Navigation("Trade");
                });

            modelBuilder.Entity("BonusCalcApi.V1.Infrastructure.PayBand", b =>
                {
                    b.HasOne("BonusCalcApi.V1.Infrastructure.Scheme", "Scheme")
                        .WithMany("PayBands")
                        .HasForeignKey("SchemeId")
                        .HasConstraintName("fk_pay_bands_schemes_scheme_id");

                    b.Navigation("Scheme");
                });

            modelBuilder.Entity("BonusCalcApi.V1.Infrastructure.PayElement", b =>
                {
                    b.HasOne("BonusCalcApi.V1.Infrastructure.PayElementType", "PayElementType")
                        .WithMany("PayElements")
                        .HasForeignKey("PayElementTypeId")
                        .HasConstraintName("fk_pay_elements_pay_element_types_pay_element_type_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BonusCalcApi.V1.Infrastructure.Timesheet", "Timesheet")
                        .WithMany("PayElements")
                        .HasForeignKey("TimesheetId")
                        .HasConstraintName("fk_pay_elements_timesheets_timesheet_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PayElementType");

                    b.Navigation("Timesheet");
                });

            modelBuilder.Entity("BonusCalcApi.V1.Infrastructure.Summary", b =>
                {
                    b.HasOne("BonusCalcApi.V1.Infrastructure.BonusPeriod", "BonusPeriod")
                        .WithMany()
                        .HasForeignKey("BonusPeriodId")
                        .HasConstraintName("fk_summaries_bonus_periods_bonus_period_id");

                    b.Navigation("BonusPeriod");
                });

            modelBuilder.Entity("BonusCalcApi.V1.Infrastructure.Timesheet", b =>
                {
                    b.HasOne("BonusCalcApi.V1.Infrastructure.Operative", "Operative")
                        .WithMany("Timesheets")
                        .HasForeignKey("OperativeId")
                        .HasConstraintName("fk_timesheets_operatives_operative_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BonusCalcApi.V1.Infrastructure.Week", "Week")
                        .WithMany("Timesheets")
                        .HasForeignKey("WeekId")
                        .HasConstraintName("fk_timesheets_weeks_week_id");

                    b.Navigation("Operative");

                    b.Navigation("Week");
                });

            modelBuilder.Entity("BonusCalcApi.V1.Infrastructure.Week", b =>
                {
                    b.HasOne("BonusCalcApi.V1.Infrastructure.BonusPeriod", "BonusPeriod")
                        .WithMany("Weeks")
                        .HasForeignKey("BonusPeriodId")
                        .HasConstraintName("fk_weeks_bonus_periods_bonus_period_id");

                    b.Navigation("BonusPeriod");
                });

            modelBuilder.Entity("BonusCalcApi.V1.Infrastructure.WeeklySummary", b =>
                {
                    b.HasOne("BonusCalcApi.V1.Infrastructure.Summary", null)
                        .WithMany("WeeklySummaries")
                        .HasForeignKey("SummaryId")
                        .HasConstraintName("fk_weekly_summaries_summaries_summary_id");
                });

            modelBuilder.Entity("BonusCalcApi.V1.Infrastructure.BonusPeriod", b =>
                {
                    b.Navigation("Weeks");
                });

            modelBuilder.Entity("BonusCalcApi.V1.Infrastructure.Operative", b =>
                {
                    b.Navigation("Timesheets");
                });

            modelBuilder.Entity("BonusCalcApi.V1.Infrastructure.PayElementType", b =>
                {
                    b.Navigation("PayElements");
                });

            modelBuilder.Entity("BonusCalcApi.V1.Infrastructure.Scheme", b =>
                {
                    b.Navigation("Operatives");

                    b.Navigation("PayBands");
                });

            modelBuilder.Entity("BonusCalcApi.V1.Infrastructure.Summary", b =>
                {
                    b.Navigation("WeeklySummaries");
                });

            modelBuilder.Entity("BonusCalcApi.V1.Infrastructure.Timesheet", b =>
                {
                    b.Navigation("PayElements");
                });

            modelBuilder.Entity("BonusCalcApi.V1.Infrastructure.Trade", b =>
                {
                    b.Navigation("Operatives");
                });

            modelBuilder.Entity("BonusCalcApi.V1.Infrastructure.Week", b =>
                {
                    b.Navigation("Timesheets");
                });
#pragma warning restore 612, 618
        }
    }
}
