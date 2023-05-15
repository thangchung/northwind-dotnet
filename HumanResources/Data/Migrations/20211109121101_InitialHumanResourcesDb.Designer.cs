﻿// <auto-generated />
using System;
using HumanResources.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HumanResources.Data.Migrations
{
    [DbContext(typeof(MainDbContext))]
    [Migration("20211109121101_InitialHumanResourcesDb")]
    partial class InitialHumanResourcesDb
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresExtension(modelBuilder, "uuid-ossp");
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("EmployeeTerritory", b =>
                {
                    b.Property<Guid>("EmployeesId")
                        .HasColumnType("uuid")
                        .HasColumnName("employees_id");

                    b.Property<Guid>("TerritoriesId")
                        .HasColumnType("uuid")
                        .HasColumnName("territories_id");

                    b.HasKey("EmployeesId", "TerritoriesId")
                        .HasName("pk_employee_territories");

                    b.HasIndex("TerritoriesId")
                        .HasDatabaseName("ix_employee_territories_territories_id");

                    b.ToTable("employee_territories", "human_resources");
                });

            modelBuilder.Entity("HumanResources.Domain.Customer", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<string>("CompanyName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("company_name");

                    b.Property<string>("ContactName")
                        .HasColumnType("text")
                        .HasColumnName("contact_name");

                    b.Property<string>("ContactTitle")
                        .HasColumnType("text")
                        .HasColumnName("contact_title");

                    b.Property<DateTime>("Created")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created")
                        .HasDefaultValueSql("now()");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated");

                    b.HasKey("Id")
                        .HasName("pk_customers");

                    b.HasIndex("Id")
                        .IsUnique()
                        .HasDatabaseName("ix_customers_id");

                    b.ToTable("customers", "human_resources");
                });

            modelBuilder.Entity("HumanResources.Domain.Employee", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<DateTime?>("BirthDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("birth_date");

                    b.Property<DateTime>("Created")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created")
                        .HasDefaultValueSql("now()");

                    b.Property<string>("Extension")
                        .HasColumnType("text")
                        .HasColumnName("extension");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("first_name");

                    b.Property<DateTime?>("HireDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("hire_date");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("last_name");

                    b.Property<string>("Notes")
                        .HasColumnType("text")
                        .HasColumnName("notes");

                    b.Property<byte[]>("Photo")
                        .HasColumnType("bytea")
                        .HasColumnName("photo");

                    b.Property<string>("PhotoPath")
                        .HasColumnType("text")
                        .HasColumnName("photo_path");

                    b.Property<Guid?>("ReportsToId")
                        .HasColumnType("uuid")
                        .HasColumnName("reports_to_id");

                    b.Property<string>("Title")
                        .HasColumnType("text")
                        .HasColumnName("title");

                    b.Property<string>("TitleOfCourtesy")
                        .HasColumnType("text")
                        .HasColumnName("title_of_courtesy");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated");

                    b.HasKey("Id")
                        .HasName("pk_employees");

                    b.HasIndex("Id")
                        .IsUnique()
                        .HasDatabaseName("ix_employees_id");

                    b.HasIndex("ReportsToId")
                        .HasDatabaseName("ix_employees_reports_to_id");

                    b.ToTable("employees", "human_resources");
                });

            modelBuilder.Entity("HumanResources.Domain.OutBox.CustomerOutbox", b =>
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
                        .HasName("pk_customer_outboxes");

                    b.HasIndex("Id")
                        .IsUnique()
                        .HasDatabaseName("ix_customer_outboxes_id");

                    b.ToTable("customer_outboxes", "human_resources");
                });

            modelBuilder.Entity("HumanResources.Domain.OutBox.EmployeeOutbox", b =>
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
                        .HasName("pk_employee_outboxes");

                    b.HasIndex("Id")
                        .IsUnique()
                        .HasDatabaseName("ix_employee_outboxes_id");

                    b.ToTable("employee_outboxes", "human_resources");
                });

            modelBuilder.Entity("HumanResources.Domain.OutBox.ShipperOutbox", b =>
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
                        .HasName("pk_shipper_outboxes");

                    b.HasIndex("Id")
                        .IsUnique()
                        .HasDatabaseName("ix_shipper_outboxes_id");

                    b.ToTable("shipper_outboxes", "human_resources");
                });

            modelBuilder.Entity("HumanResources.Domain.OutBox.SupplierOutbox", b =>
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
                        .HasName("pk_supplier_outboxes");

                    b.HasIndex("Id")
                        .IsUnique()
                        .HasDatabaseName("ix_supplier_outboxes_id");

                    b.ToTable("supplier_outboxes", "human_resources");
                });

            modelBuilder.Entity("HumanResources.Domain.Region", b =>
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

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated");

                    b.HasKey("Id")
                        .HasName("pk_regions");

                    b.HasIndex("Id")
                        .IsUnique()
                        .HasDatabaseName("ix_regions_id");

                    b.ToTable("regions", "human_resources");
                });

            modelBuilder.Entity("HumanResources.Domain.Shipper", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<string>("CompanyName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("company_name");

                    b.Property<DateTime>("Created")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created")
                        .HasDefaultValueSql("now()");

                    b.Property<string>("Phone")
                        .HasColumnType("text")
                        .HasColumnName("phone");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated");

                    b.HasKey("Id")
                        .HasName("pk_shippers");

                    b.HasIndex("Id")
                        .IsUnique()
                        .HasDatabaseName("ix_shippers_id");

                    b.ToTable("shippers", "human_resources");
                });

            modelBuilder.Entity("HumanResources.Domain.Supplier", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<string>("CompanyName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("company_name");

                    b.Property<string>("ContactName")
                        .HasColumnType("text")
                        .HasColumnName("contact_name");

                    b.Property<string>("ContactTitle")
                        .HasColumnType("text")
                        .HasColumnName("contact_title");

                    b.Property<DateTime>("Created")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created")
                        .HasDefaultValueSql("now()");

                    b.Property<string>("HomePage")
                        .HasColumnType("text")
                        .HasColumnName("home_page");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated");

                    b.HasKey("Id")
                        .HasName("pk_suppliers");

                    b.HasIndex("Id")
                        .IsUnique()
                        .HasDatabaseName("ix_suppliers_id");

                    b.ToTable("suppliers", "human_resources");
                });

            modelBuilder.Entity("HumanResources.Domain.Territory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<Guid>("RegionId")
                        .HasColumnType("uuid")
                        .HasColumnName("region_id");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated");

                    b.HasKey("Id")
                        .HasName("pk_territories");

                    b.HasIndex("Id")
                        .IsUnique()
                        .HasDatabaseName("ix_territories_id");

                    b.HasIndex("RegionId")
                        .HasDatabaseName("ix_territories_region_id");

                    b.ToTable("territories", "human_resources");
                });

            modelBuilder.Entity("EmployeeTerritory", b =>
                {
                    b.HasOne("HumanResources.Domain.Employee", null)
                        .WithMany()
                        .HasForeignKey("EmployeesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_employee_territories_employees_employees_id");

                    b.HasOne("HumanResources.Domain.Territory", null)
                        .WithMany()
                        .HasForeignKey("TerritoriesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_employee_territories_territories_territories_id");
                });

            modelBuilder.Entity("HumanResources.Domain.Customer", b =>
                {
                    b.OwnsOne("HumanResources.Domain.AddressInfo", "AddressInfo", b1 =>
                        {
                            b1.Property<Guid>("CustomerId")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("uuid")
                                .HasColumnName("id");

                            b1.Property<string>("Address")
                                .HasColumnType("text")
                                .HasColumnName("address_info_address");

                            b1.Property<string>("City")
                                .HasColumnType("text")
                                .HasColumnName("address_info_city");

                            b1.Property<string>("Country")
                                .HasColumnType("text")
                                .HasColumnName("address_info_country");

                            b1.Property<string>("Fax")
                                .HasColumnType("text")
                                .HasColumnName("address_info_fax");

                            b1.Property<string>("Phone")
                                .HasColumnType("text")
                                .HasColumnName("address_info_phone");

                            b1.Property<string>("PostalCode")
                                .HasColumnType("text")
                                .HasColumnName("address_info_postal_code");

                            b1.Property<string>("Region")
                                .HasColumnType("text")
                                .HasColumnName("address_info_region");

                            b1.HasKey("CustomerId");

                            b1.ToTable("customers", "human_resources");

                            b1.WithOwner()
                                .HasForeignKey("CustomerId")
                                .HasConstraintName("fk_customers_customers_id");
                        });

                    b.Navigation("AddressInfo");
                });

            modelBuilder.Entity("HumanResources.Domain.Employee", b =>
                {
                    b.HasOne("HumanResources.Domain.Employee", "ReportsTo")
                        .WithMany()
                        .HasForeignKey("ReportsToId")
                        .HasConstraintName("fk_employees_employees_reports_to_temp_id1");

                    b.OwnsOne("HumanResources.Domain.AddressInfo", "AddressInfo", b1 =>
                        {
                            b1.Property<Guid>("EmployeeId")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("uuid")
                                .HasColumnName("id");

                            b1.Property<string>("Address")
                                .HasColumnType("text")
                                .HasColumnName("address_info_address");

                            b1.Property<string>("City")
                                .HasColumnType("text")
                                .HasColumnName("address_info_city");

                            b1.Property<string>("Country")
                                .HasColumnType("text")
                                .HasColumnName("address_info_country");

                            b1.Property<string>("Fax")
                                .HasColumnType("text")
                                .HasColumnName("address_info_fax");

                            b1.Property<string>("Phone")
                                .HasColumnType("text")
                                .HasColumnName("address_info_phone");

                            b1.Property<string>("PostalCode")
                                .HasColumnType("text")
                                .HasColumnName("address_info_postal_code");

                            b1.Property<string>("Region")
                                .HasColumnType("text")
                                .HasColumnName("address_info_region");

                            b1.HasKey("EmployeeId");

                            b1.ToTable("employees", "human_resources");

                            b1.WithOwner()
                                .HasForeignKey("EmployeeId")
                                .HasConstraintName("fk_employees_employees_id");
                        });

                    b.Navigation("AddressInfo");

                    b.Navigation("ReportsTo");
                });

            modelBuilder.Entity("HumanResources.Domain.Supplier", b =>
                {
                    b.OwnsOne("HumanResources.Domain.AddressInfo", "AddressInfo", b1 =>
                        {
                            b1.Property<Guid>("SupplierId")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("uuid")
                                .HasColumnName("id");

                            b1.Property<string>("Address")
                                .HasColumnType("text")
                                .HasColumnName("address_info_address");

                            b1.Property<string>("City")
                                .HasColumnType("text")
                                .HasColumnName("address_info_city");

                            b1.Property<string>("Country")
                                .HasColumnType("text")
                                .HasColumnName("address_info_country");

                            b1.Property<string>("Fax")
                                .HasColumnType("text")
                                .HasColumnName("address_info_fax");

                            b1.Property<string>("Phone")
                                .HasColumnType("text")
                                .HasColumnName("address_info_phone");

                            b1.Property<string>("PostalCode")
                                .HasColumnType("text")
                                .HasColumnName("address_info_postal_code");

                            b1.Property<string>("Region")
                                .HasColumnType("text")
                                .HasColumnName("address_info_region");

                            b1.HasKey("SupplierId");

                            b1.ToTable("suppliers", "human_resources");

                            b1.WithOwner()
                                .HasForeignKey("SupplierId")
                                .HasConstraintName("fk_suppliers_suppliers_id");
                        });

                    b.Navigation("AddressInfo");
                });

            modelBuilder.Entity("HumanResources.Domain.Territory", b =>
                {
                    b.HasOne("HumanResources.Domain.Region", "Region")
                        .WithMany("Territories")
                        .HasForeignKey("RegionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_territories_regions_region_id");

                    b.Navigation("Region");
                });

            modelBuilder.Entity("HumanResources.Domain.Region", b =>
                {
                    b.Navigation("Territories");
                });
#pragma warning restore 612, 618
        }
    }
}
