using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerService.Data.Migrations
{
    public partial class InitialCustomerServiceDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "customer_service");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "customer_demographics",
                schema: "customer_service",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    description = table.Column<string>(type: "text", nullable: true),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_customer_demographics", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "customers_info",
                schema: "customer_service",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_customers_info", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "customer_customer_demo",
                schema: "customer_service",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_demographic_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_customer_customer_demo", x => x.id);
                    table.ForeignKey(
                        name: "fk_customer_customer_demo_customer_demographics_customer_demog",
                        column: x => x.customer_demographic_id,
                        principalSchema: "customer_service",
                        principalTable: "customer_demographics",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_customer_customer_demo_customer_demographic_id",
                schema: "customer_service",
                table: "customer_customer_demo",
                column: "customer_demographic_id");

            migrationBuilder.CreateIndex(
                name: "ix_customer_customer_demo_id",
                schema: "customer_service",
                table: "customer_customer_demo",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_customer_demographics_id",
                schema: "customer_service",
                table: "customer_demographics",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_customers_info_id",
                schema: "customer_service",
                table: "customers_info",
                column: "id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "customer_customer_demo",
                schema: "customer_service");

            migrationBuilder.DropTable(
                name: "customers_info",
                schema: "customer_service");

            migrationBuilder.DropTable(
                name: "customer_demographics",
                schema: "customer_service");
        }
    }
}
