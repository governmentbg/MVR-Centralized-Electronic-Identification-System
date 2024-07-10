﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using eID.RO.Application.StateMachines;

#nullable disable

namespace eID.RO.Application.Migrations
{
    [DbContext(typeof(SagasDbContext))]
    [Migration("20230911063023_Add-DenialReason-field-to-state")]
    partial class AddDenialReasonfieldtostate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("eID.RO.Application.StateMachines.EmpowermentActivationState", b =>
                {
                    b.Property<Guid>("CorrelationId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("ActivationDeadline")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("AuthorizerUids")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("CurrentState")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<int>("DenialReason")
                        .HasColumnType("integer");

                    b.Property<string>("EmpoweredUids")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid?>("EmpowermentActivationTimeoutTokenId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("EmpowermentId")
                        .HasColumnType("uuid");

                    b.Property<string>("IssuerName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("IssuerPosition")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("OnBehalfOf")
                        .HasColumnType("integer");

                    b.Property<Guid>("OriginCorrelationId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("ReceivedDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("SuccessfulCompletion")
                        .HasColumnType("boolean");

                    b.Property<string>("Uid")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("CorrelationId");

                    b.ToTable("Sagas.EmpowermentActivations", (string)null);
                });

            modelBuilder.Entity("eID.RO.Application.StateMachines.SignaturesCollectionState", b =>
                {
                    b.Property<Guid>("CorrelationId")
                        .HasColumnType("uuid");

                    b.Property<string>("AuthorizerUids")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("CurrentState")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<Guid>("EmpowermentId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("OriginCorrelationId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("ReceivedDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("SignatureCollectionTimeoutTokenId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("SignaturesCollectionDeadline")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("SignedUids")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("CorrelationId");

                    b.ToTable("Sagas.SignaturesCollections", (string)null);
                });

            modelBuilder.Entity("eID.RO.Application.StateMachines.WithdrawalsCollectionState", b =>
                {
                    b.Property<Guid>("CorrelationId")
                        .HasColumnType("uuid");

                    b.Property<string>("AuthorizerUids")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ConfirmedUids")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("CurrentState")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<string>("EmpoweredUids")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("EmpowermentId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("EmpowermentWithdrawalId")
                        .HasColumnType("uuid");

                    b.Property<string>("IssuerName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("IssuerPosition")
                        .HasColumnType("text");

                    b.Property<string>("IssuerUid")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("LegalEntityName")
                        .HasColumnType("text");

                    b.Property<string>("LegalEntityUid")
                        .HasColumnType("text");

                    b.Property<int>("OnBehalfOf")
                        .HasColumnType("integer");

                    b.Property<Guid>("OriginCorrelationId")
                        .HasColumnType("uuid");

                    b.Property<string>("Reason")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<DateTime>("ReceivedDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("WithdrawalsCollectionTimeoutTokenId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("WithdrawalsCollectionsDeadline")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("CorrelationId");

                    b.ToTable("Sagas.WithdrawalsCollections", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
