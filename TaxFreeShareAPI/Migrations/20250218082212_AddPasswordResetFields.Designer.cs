﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TaxFreeShareAPI.Data;

#nullable disable

namespace TaxFreeShareAPI.Migrations
{
    [DbContext(typeof(TaxFreeDbContext))]
    [Migration("20250218082212_AddPasswordResetFields")]
    partial class AddPasswordResetFields
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("TaxFreeShareAPI.Models.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Brand")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Category")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(65,30)");

                    b.HasKey("Id");

                    b.ToTable("Products");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Brand = "Lindt",
                            Category = "Chocolate",
                            CreatedAt = new DateTime(2025, 2, 18, 8, 22, 12, 2, DateTimeKind.Utc).AddTicks(4490),
                            Name = "Lindt Milk Chocolate",
                            Price = 49.99m
                        },
                        new
                        {
                            Id = 2,
                            Brand = "Dior",
                            Category = "Perfume",
                            CreatedAt = new DateTime(2025, 2, 18, 8, 22, 12, 2, DateTimeKind.Utc).AddTicks(4497),
                            Name = "Dior Sauvage",
                            Price = 1199.99m
                        });
                });

            modelBuilder.Entity("TaxFreeShareAPI.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("IsVerified")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("PasswordResetToken")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("ResetTokenExpiry")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("VerificationToken")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            CreatedAt = new DateTime(2025, 2, 18, 8, 22, 12, 2, DateTimeKind.Utc).AddTicks(4665),
                            Email = "admin@taxfreeshare.com",
                            IsVerified = false,
                            Name = "Admin",
                            PasswordHash = "hashedpassword123",
                            Role = "Admin"
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
