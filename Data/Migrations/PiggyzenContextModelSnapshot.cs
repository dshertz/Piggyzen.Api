﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using piggyzen.api.Data;

#nullable disable

namespace piggyzen.api.Data.Migrations
{
    [DbContext(typeof(PiggyzenContext))]
    partial class PiggyzenContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.8");

            modelBuilder.Entity("piggyzen.api.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("ParentCategoryId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("piggyzen.api.Models.Transaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("Amount")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Balance")
                        .HasColumnType("TEXT");

                    b.Property<string>("BookTransDescAmountBalanceID")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateOnly>("BookingDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("CategoryId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateOnly>("TransactionDate")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Transactions");
                });
#pragma warning restore 612, 618
        }
    }
}
