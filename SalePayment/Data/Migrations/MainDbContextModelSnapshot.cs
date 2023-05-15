﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SalePayment.Data;

#nullable disable

namespace SalePayment.Data.Migrations
{
    [DbContext(typeof(MainDbContext))]
    partial class MainDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresExtension(modelBuilder, "uuid-ossp");
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("SalePayment.Domain.CustomerInfo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created");

                    b.Property<Guid>("CustomerId")
                        .HasColumnType("uuid")
                        .HasColumnName("customer_id");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated");

                    b.HasKey("Id")
                        .HasName("pk_customers_info");

                    b.HasIndex("Id")
                        .IsUnique()
                        .HasDatabaseName("ix_customers_info_id");

                    b.ToTable("customers_info", "sale_payment");
                });

            modelBuilder.Entity("SalePayment.Domain.EmployeeInfo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created");

                    b.Property<Guid>("EmployeeId")
                        .HasColumnType("uuid")
                        .HasColumnName("employee_id");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated");

                    b.HasKey("Id")
                        .HasName("pk_employees_info");

                    b.HasIndex("Id")
                        .IsUnique()
                        .HasDatabaseName("ix_employees_info_id");

                    b.ToTable("employees_info", "sale_payment");
                });

            modelBuilder.Entity("SalePayment.Domain.Order", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<DateTime>("Created")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created")
                        .HasDefaultValueSql("now()");

                    b.Property<Guid>("CustomerInfoId")
                        .HasColumnType("uuid")
                        .HasColumnName("customer_info_id");

                    b.Property<Guid?>("EmployeeInfoId")
                        .HasColumnType("uuid")
                        .HasColumnName("employee_info_id");

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("order_date");

                    b.Property<DateTime?>("RequiredDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("required_date");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated");

                    b.HasKey("Id")
                        .HasName("pk_orders");

                    b.HasIndex("CustomerInfoId")
                        .HasDatabaseName("ix_orders_customer_info_id");

                    b.HasIndex("EmployeeInfoId")
                        .HasDatabaseName("ix_orders_employee_info_id");

                    b.HasIndex("Id")
                        .IsUnique()
                        .HasDatabaseName("ix_orders_id");

                    b.ToTable("orders", "sale_payment");
                });

            modelBuilder.Entity("SalePayment.Domain.OrderDetails", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<DateTime>("Created")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created")
                        .HasDefaultValueSql("now()");

                    b.Property<float>("Discount")
                        .HasColumnType("real")
                        .HasColumnName("discount");

                    b.Property<Guid>("OrderId")
                        .HasColumnType("uuid")
                        .HasColumnName("order_id");

                    b.Property<Guid>("ProductInfoId")
                        .HasColumnType("uuid")
                        .HasColumnName("product_info_id");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer")
                        .HasColumnName("quantity");

                    b.Property<decimal>("UnitPrice")
                        .HasColumnType("numeric")
                        .HasColumnName("unit_price");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated");

                    b.HasKey("Id")
                        .HasName("pk_order_details");

                    b.HasIndex("Id")
                        .IsUnique()
                        .HasDatabaseName("ix_order_details_id");

                    b.HasIndex("OrderId")
                        .HasDatabaseName("ix_order_details_order_id");

                    b.HasIndex("ProductInfoId")
                        .HasDatabaseName("ix_order_details_product_info_id");

                    b.ToTable("order_details", "sale_payment");
                });

            modelBuilder.Entity("SalePayment.Domain.Outbox.OrderOutbox", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<Guid>("AggregateId")
                        .HasColumnType("uuid")
                        .HasColumnName("aggregate_id");

                    b.Property<string>("AggregateType")
                        .HasColumnType("text")
                        .HasColumnName("aggregate_type");

                    b.Property<byte[]>("Payload")
                        .HasColumnType("bytea")
                        .HasColumnName("payload");

                    b.Property<string>("Type")
                        .HasColumnType("text")
                        .HasColumnName("type");

                    b.HasKey("Id")
                        .HasName("pk_order_outboxes");

                    b.HasIndex("Id")
                        .IsUnique()
                        .HasDatabaseName("ix_order_outboxes_id");

                    b.ToTable("order_outboxes", "sale_payment");
                });

            modelBuilder.Entity("SalePayment.Domain.ProductInfo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uuid")
                        .HasColumnName("product_id");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated");

                    b.HasKey("Id")
                        .HasName("pk_products_info");

                    b.HasIndex("Id")
                        .IsUnique()
                        .HasDatabaseName("ix_products_info_id");

                    b.ToTable("products_info", "sale_payment");
                });

            modelBuilder.Entity("SalePayment.Domain.Order", b =>
                {
                    b.HasOne("SalePayment.Domain.CustomerInfo", "CustomerInfo")
                        .WithMany()
                        .HasForeignKey("CustomerInfoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_orders_customer_info_customer_info_temp_id");

                    b.HasOne("SalePayment.Domain.EmployeeInfo", "EmployeeInfo")
                        .WithMany()
                        .HasForeignKey("EmployeeInfoId")
                        .HasConstraintName("fk_orders_employee_info_employee_info_temp_id");

                    b.Navigation("CustomerInfo");

                    b.Navigation("EmployeeInfo");
                });

            modelBuilder.Entity("SalePayment.Domain.OrderDetails", b =>
                {
                    b.HasOne("SalePayment.Domain.Order", null)
                        .WithMany("OrderDetails")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_order_details_orders_order_temp_id1");

                    b.HasOne("SalePayment.Domain.ProductInfo", "ProductInfo")
                        .WithMany()
                        .HasForeignKey("ProductInfoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_order_details_product_info_product_info_temp_id");

                    b.Navigation("ProductInfo");
                });

            modelBuilder.Entity("SalePayment.Domain.Order", b =>
                {
                    b.Navigation("OrderDetails");
                });
#pragma warning restore 612, 618
        }
    }
}
