using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shipping.Data.Migrations
{
    public partial class InitialShippingDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "shipping");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "shippers_info",
                schema: "shipping",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    shipper_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_shippers_info", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "states",
                schema: "shipping",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    name = table.Column<string>(type: "text", nullable: true),
                    abbr = table.Column<string>(type: "text", nullable: true),
                    region = table.Column<string>(type: "text", nullable: true),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_states", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "shipping_orders",
                schema: "shipping",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    shipped_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    shipper_info_id = table.Column<Guid>(type: "uuid", nullable: true),
                    freight = table.Column<float>(type: "real", nullable: true),
                    ship_name = table.Column<string>(type: "text", nullable: true),
                    ship_address = table.Column<string>(type: "text", nullable: true),
                    ship_city = table.Column<string>(type: "text", nullable: true),
                    ship_region = table.Column<string>(type: "text", nullable: true),
                    ship_postal_code = table.Column<string>(type: "text", nullable: true),
                    ship_country = table.Column<string>(type: "text", nullable: true),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_shipping_orders", x => x.id);
                    table.ForeignKey(
                        name: "fk_shipping_orders_shipper_info_shipper_info_temp_id",
                        column: x => x.shipper_info_id,
                        principalSchema: "shipping",
                        principalTable: "shippers_info",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_shippers_info_id",
                schema: "shipping",
                table: "shippers_info",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_shipping_orders_id",
                schema: "shipping",
                table: "shipping_orders",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_shipping_orders_shipper_info_id",
                schema: "shipping",
                table: "shipping_orders",
                column: "shipper_info_id");

            migrationBuilder.CreateIndex(
                name: "ix_states_id",
                schema: "shipping",
                table: "states",
                column: "id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "shipping_orders",
                schema: "shipping");

            migrationBuilder.DropTable(
                name: "states",
                schema: "shipping");

            migrationBuilder.DropTable(
                name: "shippers_info",
                schema: "shipping");
        }
    }
}
