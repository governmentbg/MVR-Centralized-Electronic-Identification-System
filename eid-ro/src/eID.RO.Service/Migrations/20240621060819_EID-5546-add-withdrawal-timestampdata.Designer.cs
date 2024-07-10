﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using eID.RO.Service.Database;
using eID.RO.Service.Entities;

#nullable disable

namespace eID.RO.Service.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240621060819_EID-5546-add-withdrawal-timestampdata")]
    partial class EID5546addwithdrawaltimestampdata
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("eID.RO.Service.Entities.AuthorizerUid", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("EmpowermentStatementId")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("Uid")
                        .IsRequired()
                        .HasMaxLength(13)
                        .HasColumnType("character varying(13)");

                    b.Property<int>("UidType")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("EmpowermentStatementId");

                    b.HasIndex("Uid");

                    b.ToTable("EmpowermentStatements.AuthorizerUids", (string)null);
                });

            modelBuilder.Entity("eID.RO.Service.Entities.EmpoweredUid", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("EmpowermentStatementId")
                        .HasColumnType("uuid");

                    b.Property<string>("Uid")
                        .IsRequired()
                        .HasMaxLength(13)
                        .HasColumnType("character varying(13)");

                    b.Property<int>("UidType")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("EmpowermentStatementId");

                    b.HasIndex("Uid");

                    b.ToTable("EmpowermentStatements.EmpoweredUids", (string)null);
                });

            modelBuilder.Entity("eID.RO.Service.Entities.EmpowermentDisagreement", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("ActiveDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("EmpowermentStatementId")
                        .HasColumnType("uuid");

                    b.Property<string>("IssuerUid")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<int>("IssuerUidType")
                        .HasColumnType("integer");

                    b.Property<string>("Reason")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("EmpowermentStatementId");

                    b.ToTable("EmpowermentStatements.Disagreements", (string)null);
                });

            modelBuilder.Entity("eID.RO.Service.Entities.EmpowermentDisagreementReason", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<ICollection<EmpowermentDisagreementReasonTranslation>>("Translations")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.HasKey("Id");

                    b.ToTable("EmpowermentStatements.DisagreementReasons", (string)null);
                });

            modelBuilder.Entity("eID.RO.Service.Entities.EmpowermentSignature", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("EmpowermentStatementId")
                        .HasColumnType("uuid");

                    b.Property<string>("Signature")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("SignerUid")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<int>("SignerUidType")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("EmpowermentStatementId", "SignerUid")
                        .IsUnique();

                    b.ToTable("EmpowermentStatements.Signatures", (string)null);
                });

            modelBuilder.Entity("eID.RO.Service.Entities.EmpowermentStatement", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<DateTime?>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("DenialReason")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("ExpiryDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("IssuerPosition")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("OnBehalfOf")
                        .HasColumnType("integer");

                    b.Property<int>("ServiceId")
                        .HasColumnType("integer");

                    b.Property<string>("ServiceName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<string>("SupplierId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("SupplierName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Uid")
                        .IsRequired()
                        .HasMaxLength(13)
                        .HasColumnType("character varying(13)");

                    b.Property<int>("UidType")
                        .HasColumnType("integer");

                    b.Property<ICollection<VolumeOfRepresentation>>("VolumeOfRepresentation")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<string>("XMLRepresentation")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Uid");

                    b.ToTable("EmpowermentStatements", (string)null);
                });

            modelBuilder.Entity("eID.RO.Service.Entities.EmpowermentTimestamp", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("EmpowermentStatementId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("EmpowermentStatementId")
                        .IsUnique();

                    b.ToTable("EmpowermentStatements.Timestamps", (string)null);
                });

            modelBuilder.Entity("eID.RO.Service.Entities.EmpowermentWithdrawal", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("ActiveDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("EmpowermentStatementId")
                        .HasColumnType("uuid");

                    b.Property<string>("IssuerUid")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<int>("IssuerUidType")
                        .HasColumnType("integer");

                    b.Property<string>("Reason")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<DateTime>("StartDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<string>("TimestampData")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("EmpowermentStatementId");

                    b.ToTable("EmpowermentStatements.Withdrawals", (string)null);
                });

            modelBuilder.Entity("eID.RO.Service.Entities.EmpowermentWithdrawalReason", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<ICollection<EmpowermentWithdrawalReasonTranslation>>("Translations")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.HasKey("Id");

                    b.ToTable("EmpowermentStatements.WithdrawalReasons", (string)null);
                });

            modelBuilder.Entity("eID.RO.Service.Entities.ScheduledJobSetting", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("JobName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("JobSettings")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.HasKey("Id");

                    b.HasIndex("JobName");

                    b.ToTable("ScheduledJobSettings", (string)null);
                });

            modelBuilder.Entity("eID.RO.Service.Entities.StatusHistoryRecord", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("EmpowermentStatementId")
                        .HasColumnType("uuid");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("EmpowermentStatementId");

                    b.HasIndex("Status", "DateTime");

                    b.ToTable("EmpowermentStatements.StatusHistory", (string)null);
                });

            modelBuilder.Entity("eID.RO.Service.Entities.AuthorizerUid", b =>
                {
                    b.HasOne("eID.RO.Service.Entities.EmpowermentStatement", "EmpowermentStatement")
                        .WithMany("AuthorizerUids")
                        .HasForeignKey("EmpowermentStatementId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EmpowermentStatement");
                });

            modelBuilder.Entity("eID.RO.Service.Entities.EmpoweredUid", b =>
                {
                    b.HasOne("eID.RO.Service.Entities.EmpowermentStatement", "EmpowermentStatement")
                        .WithMany("EmpoweredUids")
                        .HasForeignKey("EmpowermentStatementId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EmpowermentStatement");
                });

            modelBuilder.Entity("eID.RO.Service.Entities.EmpowermentDisagreement", b =>
                {
                    b.HasOne("eID.RO.Service.Entities.EmpowermentStatement", "EmpowermentStatement")
                        .WithMany("EmpowermentDisagreements")
                        .HasForeignKey("EmpowermentStatementId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EmpowermentStatement");
                });

            modelBuilder.Entity("eID.RO.Service.Entities.EmpowermentSignature", b =>
                {
                    b.HasOne("eID.RO.Service.Entities.EmpowermentStatement", "EmpowermentStatement")
                        .WithMany("EmpowermentSignatures")
                        .HasForeignKey("EmpowermentStatementId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EmpowermentStatement");
                });

            modelBuilder.Entity("eID.RO.Service.Entities.EmpowermentTimestamp", b =>
                {
                    b.HasOne("eID.RO.Service.Entities.EmpowermentStatement", "EmpowermentStatement")
                        .WithOne("Timestamp")
                        .HasForeignKey("eID.RO.Service.Entities.EmpowermentTimestamp", "EmpowermentStatementId");

                    b.Navigation("EmpowermentStatement");
                });

            modelBuilder.Entity("eID.RO.Service.Entities.EmpowermentWithdrawal", b =>
                {
                    b.HasOne("eID.RO.Service.Entities.EmpowermentStatement", "EmpowermentStatement")
                        .WithMany("EmpowermentWithdrawals")
                        .HasForeignKey("EmpowermentStatementId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EmpowermentStatement");
                });

            modelBuilder.Entity("eID.RO.Service.Entities.StatusHistoryRecord", b =>
                {
                    b.HasOne("eID.RO.Service.Entities.EmpowermentStatement", "EmpowermentStatement")
                        .WithMany("StatusHistory")
                        .HasForeignKey("EmpowermentStatementId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EmpowermentStatement");
                });

            modelBuilder.Entity("eID.RO.Service.Entities.EmpowermentStatement", b =>
                {
                    b.Navigation("AuthorizerUids");

                    b.Navigation("EmpoweredUids");

                    b.Navigation("EmpowermentDisagreements");

                    b.Navigation("EmpowermentSignatures");

                    b.Navigation("EmpowermentWithdrawals");

                    b.Navigation("StatusHistory");

                    b.Navigation("Timestamp");
                });
#pragma warning restore 612, 618
        }
    }
}
