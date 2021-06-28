﻿// <auto-generated />
using System;
using CHAI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CHAI.Migrations
{
    [DbContext(typeof(CHAIDbContext))]
    partial class CHAIDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.7");

            modelBuilder.Entity("CHAI.Models.QueuedEvent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("TriggerId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("TriggeredAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("EventQueue");
                });

            modelBuilder.Entity("CHAI.Models.Setting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Application")
                        .HasColumnType("TEXT");

                    b.Property<int>("GlobalCooldown")
                        .HasColumnType("INTEGER");

                    b.Property<int>("GlobalCooldownUnit")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("GlobalLastTriggered")
                        .HasColumnType("TEXT");

                    b.Property<bool>("LoggingEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<string>("OAuthToken")
                        .HasMaxLength(24)
                        .HasColumnType("TEXT");

                    b.Property<string>("UserID")
                        .HasColumnType("TEXT");

                    b.Property<string>("Username")
                        .HasMaxLength(25)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Settings");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            GlobalCooldown = 0,
                            GlobalCooldownUnit = 1,
                            GlobalLastTriggered = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            LoggingEnabled = true
                        });
                });

            modelBuilder.Entity("CHAI.Models.Trigger", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("BitsCondition")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("BitsEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<string>("CharAnimTriggerKeyChar")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("CharAnimTriggerKeyValue")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Cooldown")
                        .HasColumnType("INTEGER");

                    b.Property<int>("CooldownUnit")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Keywords")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("LastTriggered")
                        .HasColumnType("TEXT");

                    b.Property<int>("MaximumBits")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MinimumBits")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("RewardName")
                        .HasMaxLength(45)
                        .HasColumnType("TEXT");

                    b.Property<bool>("UserLevelEveryone")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("UserLevelMods")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("UserLevelSubs")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("UserLevelVips")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Triggers");
                });
#pragma warning restore 612, 618
        }
    }
}
