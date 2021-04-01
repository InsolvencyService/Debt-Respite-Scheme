﻿// <auto-generated />

using System;
using Insolvency.Data.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Insolvency.Data.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20200812110851_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Insolvency.Notifications.Models.NotificationMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("PayLoad")
                        .HasColumnType("text");

                    b.Property<Guid?>("SenderSystemId")
                        .HasColumnType("uuid");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("SenderSystemId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("Insolvency.Notifications.Models.Sender", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Senders");
                });

            modelBuilder.Entity("Insolvency.Notifications.Models.NotificationMessage", b =>
                {
                    b.HasOne("Insolvency.Notifications.Models.Sender", "SenderSystem")
                        .WithMany()
                        .HasForeignKey("SenderSystemId");
                });
#pragma warning restore 612, 618
        }
    }
}
