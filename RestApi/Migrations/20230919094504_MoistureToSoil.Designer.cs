﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using RestApi.Data;

#nullable disable

namespace RestApi.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20230919094504_MoistureToSoil")]
    partial class MoistureToSoil
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("RestApi.Models.Soil", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<double>("Moisture")
                        .HasColumnType("double precision")
                        .HasColumnName("soil_moisture");

                    b.Property<double>("Temperature")
                        .HasColumnType("double precision")
                        .HasColumnName("soil_temperature");

                    b.HasKey("Id");

                    b.ToTable("soil");
                });

            modelBuilder.Entity("RestApi.Models.Temperature", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<double>("Temp")
                        .HasColumnType("double precision")
                        .HasColumnName("temp");

                    b.HasKey("Id");

                    b.ToTable("temperatures");
                });
#pragma warning restore 612, 618
        }
    }
}