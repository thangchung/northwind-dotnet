using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HumanResources.Data.Migrations
{
    public partial class InitialHumanResourcesDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "human_resources");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "customers",
                schema: "human_resources",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    company_name = table.Column<string>(type: "text", nullable: false),
                    contact_name = table.Column<string>(type: "text", nullable: true),
                    contact_title = table.Column<string>(type: "text", nullable: true),
                    address_info_address = table.Column<string>(type: "text", nullable: true),
                    address_info_city = table.Column<string>(type: "text", nullable: true),
                    address_info_region = table.Column<string>(type: "text", nullable: true),
                    address_info_postal_code = table.Column<string>(type: "text", nullable: true),
                    address_info_country = table.Column<string>(type: "text", nullable: true),
                    address_info_phone = table.Column<string>(type: "text", nullable: true),
                    address_info_fax = table.Column<string>(type: "text", nullable: true),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_customers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "employees",
                schema: "human_resources",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    last_name = table.Column<string>(type: "text", nullable: false),
                    first_name = table.Column<string>(type: "text", nullable: false),
                    title = table.Column<string>(type: "text", nullable: true),
                    title_of_courtesy = table.Column<string>(type: "text", nullable: true),
                    birth_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    hire_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    address_info_address = table.Column<string>(type: "text", nullable: true),
                    address_info_city = table.Column<string>(type: "text", nullable: true),
                    address_info_region = table.Column<string>(type: "text", nullable: true),
                    address_info_postal_code = table.Column<string>(type: "text", nullable: true),
                    address_info_country = table.Column<string>(type: "text", nullable: true),
                    address_info_phone = table.Column<string>(type: "text", nullable: true),
                    address_info_fax = table.Column<string>(type: "text", nullable: true),
                    extension = table.Column<string>(type: "text", nullable: true),
                    photo = table.Column<byte[]>(type: "bytea", nullable: true),
                    notes = table.Column<string>(type: "text", nullable: true),
                    reports_to_id = table.Column<Guid>(type: "uuid", nullable: true),
                    photo_path = table.Column<string>(type: "text", nullable: true),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_employees", x => x.id);
                    table.ForeignKey(
                        name: "fk_employees_employees_reports_to_temp_id1",
                        column: x => x.reports_to_id,
                        principalSchema: "human_resources",
                        principalTable: "employees",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "regions",
                schema: "human_resources",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    description = table.Column<string>(type: "text", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_regions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "shippers",
                schema: "human_resources",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    company_name = table.Column<string>(type: "text", nullable: false),
                    phone = table.Column<string>(type: "text", nullable: true),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_shippers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "suppliers",
                schema: "human_resources",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    company_name = table.Column<string>(type: "text", nullable: false),
                    contact_name = table.Column<string>(type: "text", nullable: true),
                    contact_title = table.Column<string>(type: "text", nullable: true),
                    address_info_address = table.Column<string>(type: "text", nullable: true),
                    address_info_city = table.Column<string>(type: "text", nullable: true),
                    address_info_region = table.Column<string>(type: "text", nullable: true),
                    address_info_postal_code = table.Column<string>(type: "text", nullable: true),
                    address_info_country = table.Column<string>(type: "text", nullable: true),
                    address_info_phone = table.Column<string>(type: "text", nullable: true),
                    address_info_fax = table.Column<string>(type: "text", nullable: true),
                    home_page = table.Column<string>(type: "text", nullable: true),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_suppliers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "territories",
                schema: "human_resources",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    description = table.Column<string>(type: "text", nullable: false),
                    region_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_territories", x => x.id);
                    table.ForeignKey(
                        name: "fk_territories_regions_region_id",
                        column: x => x.region_id,
                        principalSchema: "human_resources",
                        principalTable: "regions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "employee_territories",
                schema: "human_resources",
                columns: table => new
                {
                    employees_id = table.Column<Guid>(type: "uuid", nullable: false),
                    territories_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_employee_territories", x => new { x.employees_id, x.territories_id });
                    table.ForeignKey(
                        name: "fk_employee_territories_employees_employees_id",
                        column: x => x.employees_id,
                        principalSchema: "human_resources",
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_employee_territories_territories_territories_id",
                        column: x => x.territories_id,
                        principalSchema: "human_resources",
                        principalTable: "territories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_customers_id",
                schema: "human_resources",
                table: "customers",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_employee_territories_territories_id",
                schema: "human_resources",
                table: "employee_territories",
                column: "territories_id");

            migrationBuilder.CreateIndex(
                name: "ix_employees_id",
                schema: "human_resources",
                table: "employees",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_employees_reports_to_id",
                schema: "human_resources",
                table: "employees",
                column: "reports_to_id");

            migrationBuilder.CreateIndex(
                name: "ix_regions_id",
                schema: "human_resources",
                table: "regions",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_shippers_id",
                schema: "human_resources",
                table: "shippers",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_suppliers_id",
                schema: "human_resources",
                table: "suppliers",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_territories_id",
                schema: "human_resources",
                table: "territories",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_territories_region_id",
                schema: "human_resources",
                table: "territories",
                column: "region_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "customers",
                schema: "human_resources");

            migrationBuilder.DropTable(
                name: "employee_territories",
                schema: "human_resources");

            migrationBuilder.DropTable(
                name: "shippers",
                schema: "human_resources");

            migrationBuilder.DropTable(
                name: "suppliers",
                schema: "human_resources");

            migrationBuilder.DropTable(
                name: "employees",
                schema: "human_resources");

            migrationBuilder.DropTable(
                name: "territories",
                schema: "human_resources");

            migrationBuilder.DropTable(
                name: "regions",
                schema: "human_resources");
        }
    }
}
