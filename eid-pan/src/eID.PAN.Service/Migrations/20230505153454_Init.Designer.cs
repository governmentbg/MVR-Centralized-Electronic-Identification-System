﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using eID.PAN.Service.Database;
using eID.PAN.Service.Entities;

#nullable disable

namespace eID.PAN.Service.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20230505153454_Init")]
    partial class Init
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("eID.PAN.Service.Entities.Configuration", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.Property<string>("ModifiedBy")
                        .HasColumnType("text");

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("Key")
                        .IsUnique();

                    b.ToTable("Configurations", (string)null);

                    b.HasData(
                        new
                        {
                            Id = new Guid("3273b4a3-59c8-499a-87db-72221a1129ee"),
                            Data = "{\"servers\":[{\"host\":\"testHost1\",\"port\":1234,\"useSSL\":false,\"useTLS\":false,\"username\":\"testUsername1\",\"password\":\"testPassword1\"}]}",
                            Key = "SMTP"
                        });
                });

            modelBuilder.Entity("eID.PAN.Service.Entities.DeactivatedUserEvent", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("SystemEventId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("SystemEventId");

                    b.HasIndex("UserId", "SystemEventId")
                        .IsUnique();

                    b.ToTable("DeactivatedUserEvents", (string)null);
                });

            modelBuilder.Entity("eID.PAN.Service.Entities.NotificationChannel", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("CallbackUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("InfoUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric");

                    b.Property<Guid>("SystemId")
                        .HasColumnType("uuid");

                    b.Property<ICollection<NotificationChannelTranslation>>("Translations")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.HasKey("Id");

                    b.ToTable((string)null);

                    b.UseTpcMappingStrategy();
                });

            modelBuilder.Entity("eID.PAN.Service.Entities.RegisteredSystem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("IsApproved")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("Name", "IsApproved", "IsDeleted");

                    b.ToTable("RegisteredSystems", (string)null);
                });

            modelBuilder.Entity("eID.PAN.Service.Entities.SystemEvent", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsMandatory")
                        .HasColumnType("boolean");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("RegisteredSystemId")
                        .HasColumnType("uuid");

                    b.Property<ICollection<Translation>>("Translations")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.HasKey("Id");

                    b.HasIndex("Code", "IsDeleted");

                    b.HasIndex("RegisteredSystemId", "Code")
                        .IsUnique();

                    b.ToTable("SystemEvents", (string)null);
                });

            modelBuilder.Entity("eID.PAN.Service.Entities.NotificationChannelApproved", b =>
                {
                    b.HasBaseType("eID.PAN.Service.Entities.NotificationChannel");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("NotificationChannels", (string)null);
                });

            modelBuilder.Entity("eID.PAN.Service.Entities.NotificationChannelArchive", b =>
                {
                    b.HasBaseType("eID.PAN.Service.Entities.NotificationChannel");

                    b.ToTable("NotificationChannels.Archive", (string)null);
                });

            modelBuilder.Entity("eID.PAN.Service.Entities.NotificationChannelPending", b =>
                {
                    b.HasBaseType("eID.PAN.Service.Entities.NotificationChannel");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("NotificationChannels.Pending", (string)null);
                });

            modelBuilder.Entity("eID.PAN.Service.Entities.NotificationChannelRejected", b =>
                {
                    b.HasBaseType("eID.PAN.Service.Entities.NotificationChannel");

                    b.ToTable("NotificationChannels.Rejected", (string)null);
                });

            modelBuilder.Entity("eID.PAN.Service.Entities.DeactivatedUserEvent", b =>
                {
                    b.HasOne("eID.PAN.Service.Entities.SystemEvent", "Event")
                        .WithMany("DeactivatedUserEvent")
                        .HasForeignKey("SystemEventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Event");
                });

            modelBuilder.Entity("eID.PAN.Service.Entities.SystemEvent", b =>
                {
                    b.HasOne("eID.PAN.Service.Entities.RegisteredSystem", "RegisteredSystem")
                        .WithMany("Events")
                        .HasForeignKey("RegisteredSystemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("RegisteredSystem");
                });

            modelBuilder.Entity("eID.PAN.Service.Entities.RegisteredSystem", b =>
                {
                    b.Navigation("Events");
                });

            modelBuilder.Entity("eID.PAN.Service.Entities.SystemEvent", b =>
                {
                    b.Navigation("DeactivatedUserEvent");
                });
#pragma warning restore 612, 618
        }
    }
}
