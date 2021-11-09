using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalePayment.Data.Migrations
{
    public partial class InitialSalePaymentDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "sale_payment");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "customers_info",
                schema: "sale_payment",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_customers_info", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "employees_info",
                schema: "sale_payment",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_employees_info", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "products_info",
                schema: "sale_payment",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_products_info", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "orders",
                schema: "sale_payment",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    customer_info_id = table.Column<Guid>(type: "uuid", nullable: false),
                    employee_info_id = table.Column<Guid>(type: "uuid", nullable: true),
                    order_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    required_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_orders", x => x.id);
                    table.ForeignKey(
                        name: "fk_orders_customer_info_customer_info_temp_id",
                        column: x => x.customer_info_id,
                        principalSchema: "sale_payment",
                        principalTable: "customers_info",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_orders_employee_info_employee_info_temp_id",
                        column: x => x.employee_info_id,
                        principalSchema: "sale_payment",
                        principalTable: "employees_info",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "order_details",
                schema: "sale_payment",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_info_id = table.Column<Guid>(type: "uuid", nullable: false),
                    unit_price = table.Column<decimal>(type: "numeric", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    discount = table.Column<float>(type: "real", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_order_details", x => x.id);
                    table.ForeignKey(
                        name: "fk_order_details_orders_order_temp_id1",
                        column: x => x.order_id,
                        principalSchema: "sale_payment",
                        principalTable: "orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_order_details_product_info_product_info_temp_id",
                        column: x => x.product_info_id,
                        principalSchema: "sale_payment",
                        principalTable: "products_info",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_customers_info_id",
                schema: "sale_payment",
                table: "customers_info",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_employees_info_id",
                schema: "sale_payment",
                table: "employees_info",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_order_details_id",
                schema: "sale_payment",
                table: "order_details",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_order_details_order_id",
                schema: "sale_payment",
                table: "order_details",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "ix_order_details_product_info_id",
                schema: "sale_payment",
                table: "order_details",
                column: "product_info_id");

            migrationBuilder.CreateIndex(
                name: "ix_orders_customer_info_id",
                schema: "sale_payment",
                table: "orders",
                column: "customer_info_id");

            migrationBuilder.CreateIndex(
                name: "ix_orders_employee_info_id",
                schema: "sale_payment",
                table: "orders",
                column: "employee_info_id");

            migrationBuilder.CreateIndex(
                name: "ix_orders_id",
                schema: "sale_payment",
                table: "orders",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_products_info_id",
                schema: "sale_payment",
                table: "products_info",
                column: "id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "order_details",
                schema: "sale_payment");

            migrationBuilder.DropTable(
                name: "orders",
                schema: "sale_payment");

            migrationBuilder.DropTable(
                name: "products_info",
                schema: "sale_payment");

            migrationBuilder.DropTable(
                name: "customers_info",
                schema: "sale_payment");

            migrationBuilder.DropTable(
                name: "employees_info",
                schema: "sale_payment");
        }
    }
}
