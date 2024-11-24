﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Piggyzen.Api.Data;

#nullable disable

namespace Piggyzen.Api.Data.Migrations
{
    [DbContext(typeof(PiggyzenContext))]
    [Migration("20241118160425_AddCategoryGroupToCategory")]
    partial class AddCategoryGroupToCategory
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.8");

            modelBuilder.Entity("Piggyzen.Api.Models.CategorizationHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CategoryId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("CategorizationHistory");
                });

            modelBuilder.Entity("Piggyzen.Api.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Group")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsActive")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsStandard")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("ParentCategoryId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ParentCategoryId");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("Piggyzen.Api.Models.Transaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<decimal?>("AdjustedAmount")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Amount")
                        .HasColumnType("TEXT");

                    b.Property<decimal?>("Balance")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("BookingDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("CategorizationStatus")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("CategoryId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsImported")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsOutlayOrReturn")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Memo")
                        .HasColumnType("TEXT");

                    b.Property<int?>("ParentTransactionId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ReferenceId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("TransactionDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("VerificationStatus")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("ParentTransactionId");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("Piggyzen.Api.Models.TransactionRelation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("SourceTransactionId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TargetTransactionId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("SourceTransactionId");

                    b.HasIndex("TargetTransactionId");

                    b.ToTable("TransactionRelations");
                });

            modelBuilder.Entity("Piggyzen.Api.Models.CategorizationHistory", b =>
                {
                    b.HasOne("Piggyzen.Api.Models.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("Piggyzen.Api.Models.Category", b =>
                {
                    b.HasOne("Piggyzen.Api.Models.Category", "ParentCategory")
                        .WithMany("Subcategories")
                        .HasForeignKey("ParentCategoryId");

                    b.Navigation("ParentCategory");
                });

            modelBuilder.Entity("Piggyzen.Api.Models.Transaction", b =>
                {
                    b.HasOne("Piggyzen.Api.Models.Category", "Category")
                        .WithMany("Transactions")
                        .HasForeignKey("CategoryId");

                    b.HasOne("Piggyzen.Api.Models.Transaction", "ParentTransaction")
                        .WithMany("ChildTransactions")
                        .HasForeignKey("ParentTransactionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Category");

                    b.Navigation("ParentTransaction");
                });

            modelBuilder.Entity("Piggyzen.Api.Models.TransactionRelation", b =>
                {
                    b.HasOne("Piggyzen.Api.Models.Transaction", "SourceTransaction")
                        .WithMany("SourceRelations")
                        .HasForeignKey("SourceTransactionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Piggyzen.Api.Models.Transaction", "TargetTransaction")
                        .WithMany("TargetRelations")
                        .HasForeignKey("TargetTransactionId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("SourceTransaction");

                    b.Navigation("TargetTransaction");
                });

            modelBuilder.Entity("Piggyzen.Api.Models.Category", b =>
                {
                    b.Navigation("Subcategories");

                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("Piggyzen.Api.Models.Transaction", b =>
                {
                    b.Navigation("ChildTransactions");

                    b.Navigation("SourceRelations");

                    b.Navigation("TargetRelations");
                });
#pragma warning restore 612, 618
        }
    }
}