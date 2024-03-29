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
    [Migration("20211012131447_CreatePayElement")]
    partial class CreatePayElement
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
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime?>("ClosedAt")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("closed_at");

                    b.Property<int>("Period")
                        .HasColumnType("integer")
                        .HasColumnName("period");

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

                    b.HasIndex("Year", "Period")
                        .IsUnique()
                        .HasDatabaseName("ix_bonus_periods_year_period");

                    b.ToTable("bonus_periods");
                });

            modelBuilder.Entity("BonusCalcApi.V1.Infrastructure.Operative", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(6)
                        .HasColumnType("character varying(6)")
                        .HasColumnName("id");

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

                    b.Property<string>("Scheme")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("scheme");

                    b.Property<string>("Section")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("section");

                    b.Property<string>("Trade")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("trade");

                    b.HasKey("Id")
                        .HasName("pk_operatives");

                    b.ToTable("operatives");
                });

            modelBuilder.Entity("BonusCalcApi.V1.Infrastructure.PayBand", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("Band")
                        .HasColumnType("integer")
                        .HasColumnName("band");

                    b.Property<string>("TradeId")
                        .IsRequired()
                        .HasMaxLength(3)
                        .HasColumnType("character varying(3)")
                        .HasColumnName("trade_id");

                    b.Property<decimal>("Value")
                        .HasColumnType("numeric")
                        .HasColumnName("value");

                    b.HasKey("Id")
                        .HasName("pk_pay_bands");

                    b.HasIndex("TradeId", "Band")
                        .IsUnique()
                        .HasDatabaseName("ix_pay_bands_trade_id_band");

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

                    b.Property<string>("Comment")
                        .HasColumnType("text")
                        .HasColumnName("comment");

                    b.Property<decimal>("Duration")
                        .HasPrecision(10, 4)
                        .HasColumnType("numeric(10,4)")
                        .HasColumnName("duration");

                    b.Property<int>("PayElementTypeId")
                        .HasColumnType("integer")
                        .HasColumnName("pay_element_type_id");

                    b.Property<bool>("Productive")
                        .HasColumnType("boolean")
                        .HasColumnName("productive");

                    b.Property<int>("TimesheetId")
                        .HasColumnType("integer")
                        .HasColumnName("timesheet_id");

                    b.Property<decimal>("Value")
                        .HasPrecision(10, 4)
                        .HasColumnType("numeric(10,4)")
                        .HasColumnName("value");

                    b.Property<int>("WeekDay")
                        .HasColumnType("integer")
                        .HasColumnName("week_day");

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
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("description");

                    b.Property<bool>("Paid")
                        .HasColumnType("boolean")
                        .HasColumnName("paid");

                    b.Property<bool>("PayAtBand")
                        .HasColumnType("boolean")
                        .HasColumnName("pay_at_band");

                    b.HasKey("Id")
                        .HasName("pk_pay_element_types");

                    b.HasIndex("Description")
                        .IsUnique()
                        .HasDatabaseName("ix_pay_element_types_description");

                    b.ToTable("pay_element_types");
                });

            modelBuilder.Entity("BonusCalcApi.V1.Infrastructure.Timesheet", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("OperativeId")
                        .IsRequired()
                        .HasMaxLength(6)
                        .HasColumnType("character varying(6)")
                        .HasColumnName("operative_id");

                    b.Property<int>("WeekId")
                        .HasColumnType("integer")
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
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("BonusPeriodId")
                        .HasColumnType("integer")
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

            modelBuilder.Entity("BonusCalcApi.V1.Infrastructure.PayBand", b =>
                {
                    b.HasOne("BonusCalcApi.V1.Infrastructure.Trade", "Trade")
                        .WithMany("PayBands")
                        .HasForeignKey("TradeId")
                        .HasConstraintName("fk_pay_bands_trades_trade_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Trade");
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
                        .HasConstraintName("fk_timesheets_weeks_week_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Operative");

                    b.Navigation("Week");
                });

            modelBuilder.Entity("BonusCalcApi.V1.Infrastructure.Week", b =>
                {
                    b.HasOne("BonusCalcApi.V1.Infrastructure.BonusPeriod", "BonusPeriod")
                        .WithMany("Weeks")
                        .HasForeignKey("BonusPeriodId")
                        .HasConstraintName("fk_weeks_bonus_periods_bonus_period_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BonusPeriod");
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

            modelBuilder.Entity("BonusCalcApi.V1.Infrastructure.Timesheet", b =>
                {
                    b.Navigation("PayElements");
                });

            modelBuilder.Entity("BonusCalcApi.V1.Infrastructure.Trade", b =>
                {
                    b.Navigation("PayBands");
                });

            modelBuilder.Entity("BonusCalcApi.V1.Infrastructure.Week", b =>
                {
                    b.Navigation("Timesheets");
                });
#pragma warning restore 612, 618
        }
    }
}
