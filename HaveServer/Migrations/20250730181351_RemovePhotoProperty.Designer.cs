﻿// <auto-generated />
using AitukServer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AitukServer.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250730181351_RemovePhotoProperty")]
    partial class RemovePhotoProperty
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("AitukServer.Models.ACategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("AitukServer.Models.AColor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Colors");
                });

            modelBuilder.Entity("AitukServer.Models.AGender", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Genders");
                });

            modelBuilder.Entity("AitukServer.Models.AProduct", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<int?>("ACategoryId")
                        .HasColumnType("integer");

                    b.Property<int?>("AColorId")
                        .HasColumnType("integer");

                    b.Property<int?>("AGenderId")
                        .HasColumnType("integer");

                    b.Property<string>("Brand")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("CategoryId")
                        .HasColumnType("integer");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("ColorId")
                        .HasColumnType("integer");

                    b.Property<decimal>("Cost")
                        .HasColumnType("numeric");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("GenderId")
                        .HasColumnType("integer");

                    b.Property<string>("KeyWords")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ACategoryId");

                    b.HasIndex("AColorId");

                    b.HasIndex("AGenderId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("AitukServer.Models.AProductPhoto", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long?>("AProductId")
                        .HasColumnType("bigint");

                    b.Property<long>("ProductId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("AProductId");

                    b.ToTable("ProductPhotos");
                });

            modelBuilder.Entity("AitukServer.Models.AProductShop", b =>
                {
                    b.Property<long>("ProductId")
                        .HasColumnType("bigint");

                    b.Property<long>("ShopId")
                        .HasColumnType("bigint");

                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<long>("ProductCount")
                        .HasColumnType("bigint");

                    b.HasKey("ProductId", "ShopId");

                    b.HasIndex("ShopId");

                    b.ToTable("ProductShops");
                });

            modelBuilder.Entity("AitukServer.Models.AProductSize", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long?>("AProductId")
                        .HasColumnType("bigint");

                    b.Property<long>("ProductId")
                        .HasColumnType("bigint");

                    b.Property<int>("SizeId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("AProductId");

                    b.ToTable("ProductSizes");
                });

            modelBuilder.Entity("AitukServer.Models.ASeller", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Sellers");
                });

            modelBuilder.Entity("AitukServer.Models.AShop", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<double?>("Latitude")
                        .HasColumnType("double precision");

                    b.Property<double?>("Longitude")
                        .HasColumnType("double precision");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("SellerId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("Shops");
                });

            modelBuilder.Entity("AitukServer.Models.AShopPhoto", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("ShopId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("ShopId");

                    b.ToTable("ShopPhotos");
                });

            modelBuilder.Entity("AitukServer.Models.ASize", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Sizes");
                });

            modelBuilder.Entity("AitukServer.Models.AProduct", b =>
                {
                    b.HasOne("AitukServer.Models.ACategory", null)
                        .WithMany("Products")
                        .HasForeignKey("ACategoryId");

                    b.HasOne("AitukServer.Models.AColor", null)
                        .WithMany("Products")
                        .HasForeignKey("AColorId");

                    b.HasOne("AitukServer.Models.AGender", null)
                        .WithMany("Products")
                        .HasForeignKey("AGenderId");
                });

            modelBuilder.Entity("AitukServer.Models.AProductPhoto", b =>
                {
                    b.HasOne("AitukServer.Models.AProduct", null)
                        .WithMany("Photos")
                        .HasForeignKey("AProductId");
                });

            modelBuilder.Entity("AitukServer.Models.AProductShop", b =>
                {
                    b.HasOne("AitukServer.Models.AProduct", "Product")
                        .WithMany("ProductShops")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AitukServer.Models.AShop", "Shop")
                        .WithMany("ProductShops")
                        .HasForeignKey("ShopId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");

                    b.Navigation("Shop");
                });

            modelBuilder.Entity("AitukServer.Models.AProductSize", b =>
                {
                    b.HasOne("AitukServer.Models.AProduct", null)
                        .WithMany("Sizes")
                        .HasForeignKey("AProductId");
                });

            modelBuilder.Entity("AitukServer.Models.AShopPhoto", b =>
                {
                    b.HasOne("AitukServer.Models.AShop", "Shop")
                        .WithMany("Photos")
                        .HasForeignKey("ShopId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Shop");
                });

            modelBuilder.Entity("AitukServer.Models.ACategory", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("AitukServer.Models.AColor", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("AitukServer.Models.AGender", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("AitukServer.Models.AProduct", b =>
                {
                    b.Navigation("Photos");

                    b.Navigation("ProductShops");

                    b.Navigation("Sizes");
                });

            modelBuilder.Entity("AitukServer.Models.AShop", b =>
                {
                    b.Navigation("Photos");

                    b.Navigation("ProductShops");
                });
#pragma warning restore 612, 618
        }
    }
}
