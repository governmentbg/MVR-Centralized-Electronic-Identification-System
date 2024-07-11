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
    [Migration("20230731072835_EID-1347 Supplier and volume of representation property type changes")]
    partial class EID1347Supplierandvolumeofrepresentationpropertytypechanges
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

                    b.Property<string>("Uid")
                        .IsRequired()
                        .HasMaxLength(13)
                        .HasColumnType("character varying(13)");

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

                    b.HasKey("Id");

                    b.HasIndex("EmpowermentStatementId");

                    b.HasIndex("Uid");

                    b.ToTable("EmpowermentStatements.EmpoweredUids", (string)null);
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

            modelBuilder.Entity("eID.RO.Service.Entities.EmpowermentWithdrawReason", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<ICollection<EmpowermentWithdrawalReasonTranslation>>("Translations")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.HasKey("Id");

                    b.ToTable("EmpowermentStatements.Reasons", (string)null);
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

            modelBuilder.Entity("eID.RO.Service.Entities.EmpowermentStatement", b =>
                {
                    b.Navigation("AuthorizerUids");

                    b.Navigation("EmpoweredUids");
                });
#pragma warning restore 612, 618
        }
    }
}
