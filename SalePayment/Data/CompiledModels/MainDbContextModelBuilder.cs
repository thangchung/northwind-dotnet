﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#pragma warning disable 219, 612, 618
#nullable enable

namespace SalePayment
{
    public partial class MainDbContextModel
    {
        partial void Initialize()
        {
            var customerInfo = CustomerInfoEntityType.Create(this);
            var employeeInfo = EmployeeInfoEntityType.Create(this);
            var order = OrderEntityType.Create(this);
            var orderDetails = OrderDetailsEntityType.Create(this);
            var orderOutbox = OrderOutboxEntityType.Create(this);
            var productInfo = ProductInfoEntityType.Create(this);

            OrderEntityType.CreateForeignKey1(order, customerInfo);
            OrderEntityType.CreateForeignKey2(order, employeeInfo);
            OrderDetailsEntityType.CreateForeignKey1(orderDetails, order);
            OrderDetailsEntityType.CreateForeignKey2(orderDetails, productInfo);

            CustomerInfoEntityType.CreateAnnotations(customerInfo);
            EmployeeInfoEntityType.CreateAnnotations(employeeInfo);
            OrderEntityType.CreateAnnotations(order);
            OrderDetailsEntityType.CreateAnnotations(orderDetails);
            OrderOutboxEntityType.CreateAnnotations(orderOutbox);
            ProductInfoEntityType.CreateAnnotations(productInfo);

            AddAnnotation("Npgsql:PostgresExtension:uuid-ossp", ",,");
            AddAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
            AddAnnotation("ProductVersion", "6.0.0");
            AddAnnotation("Relational:MaxIdentifierLength", 63);
        }
    }
}
