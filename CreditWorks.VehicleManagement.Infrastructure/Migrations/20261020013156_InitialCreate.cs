using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CreditWorks.VehicleManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Manufacturers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Manufacturers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VehicleCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MinWeightKg = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MaxWeightKg = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    IconKey = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ManufacturerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Vin = table.Column<string>(type: "nchar(17)", fixedLength: true, maxLength: 17, nullable: false),
                    RegistrationNumber = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    WeightKg = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vehicles_Manufacturers_ManufacturerId",
                        column: x => x.ManufacturerId,
                        principalTable: "Manufacturers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Manufacturers",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-5000-9000-000000000001"), "Mazda" },
                    { new Guid("10000000-0000-5000-9000-000000000002"), "Mercedes" },
                    { new Guid("10000000-0000-5000-9000-000000000003"), "Honda" },
                    { new Guid("10000000-0000-5000-9000-000000000004"), "Ferrari" },
                    { new Guid("10000000-0000-5000-9000-000000000005"), "Toyota" }
                });

            migrationBuilder.InsertData(
                table: "VehicleCategories",
                columns: new[] { "Id", "Description", "IconKey", "MaxWeightKg", "MinWeightKg", "Name" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-3000-7000-000000000001"), "Light - up to 500 kg", "car", 500.00m, 0.00m, "Light" },
                    { new Guid("10000000-0000-3000-7000-000000000002"), "Medium - 500 kg to 2500 kg", "van", 2500.00m, 500.00m, "Medium" },
                    { new Guid("10000000-0000-3000-7000-000000000003"), "Heavy - 2500 kg and above", "truck", null, 2500.00m, "Heavy" }
                });

            migrationBuilder.InsertData(
                table: "Vehicles",
                columns: new[] { "Id", "ManufacturerId", "OwnerName", "RegistrationNumber", "Vin", "WeightKg", "Year" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-4000-8000-000000000001"), new Guid("10000000-0000-5000-9000-000000000001"), "Amelia Wilson", "CW0001", "CWMZD000000000001", 420.50m, 2018 },
                    { new Guid("10000000-0000-4000-8000-000000000002"), new Guid("10000000-0000-5000-9000-000000000001"), "Noah Thompson", "CW0002", "CWMZD000000000002", 499.99m, 2020 },
                    { new Guid("10000000-0000-4000-8000-000000000003"), new Guid("10000000-0000-5000-9000-000000000001"), "Isla Martin", "CW0003", "CWMZD000000000003", 500.00m, 2022 },
                    { new Guid("10000000-0000-4000-8000-000000000004"), new Guid("10000000-0000-5000-9000-000000000001"), "Oliver Chen", "CW0004", "CWMZD000000000004", 725.25m, 2017 },
                    { new Guid("10000000-0000-4000-8000-000000000005"), new Guid("10000000-0000-5000-9000-000000000002"), "Sophie Patel", "CW0005", "CWMRC000000000005", 1850.75m, 2019 },
                    { new Guid("10000000-0000-4000-8000-000000000006"), new Guid("10000000-0000-5000-9000-000000000002"), "Liam Roberts", "CW0006", "CWMRC000000000006", 2200.00m, 2021 },
                    { new Guid("10000000-0000-4000-8000-000000000007"), new Guid("10000000-0000-5000-9000-000000000002"), "Mia Anderson", "CW0007", "CWMRC000000000007", 2499.99m, 2016 },
                    { new Guid("10000000-0000-4000-8000-000000000008"), new Guid("10000000-0000-5000-9000-000000000002"), "Ethan Walker", "CW0008", "CWMRC000000000008", 2500.00m, 2023 },
                    { new Guid("10000000-0000-4000-8000-000000000009"), new Guid("10000000-0000-5000-9000-000000000003"), "Ava Brown", "CW0009", "CWHND000000000009", 1500.00m, 2015 },
                    { new Guid("10000000-0000-4000-8000-000000000010"), new Guid("10000000-0000-5000-9000-000000000003"), "Lucas Taylor", "CW0010", "CWHND000000000010", 500.00m, 2024 },
                    { new Guid("10000000-0000-4000-8000-000000000011"), new Guid("10000000-0000-5000-9000-000000000003"), "Grace Williams", "CW0011", "CWHND000000000011", 499.50m, 2014 },
                    { new Guid("10000000-0000-4000-8000-000000000012"), new Guid("10000000-0000-5000-9000-000000000003"), "Jack Harris", "CW0012", "CWHND000000000012", 980.10m, 2020 },
                    { new Guid("10000000-0000-4000-8000-000000000013"), new Guid("10000000-0000-5000-9000-000000000004"), "Charlotte King", "CW0013", "CWFRR000000000013", 1350.00m, 2018 },
                    { new Guid("10000000-0000-4000-8000-000000000014"), new Guid("10000000-0000-5000-9000-000000000004"), "James Cooper", "CW0014", "CWFRR000000000014", 1600.25m, 2022 },
                    { new Guid("10000000-0000-4000-8000-000000000015"), new Guid("10000000-0000-5000-9000-000000000004"), "Ella Mitchell", "CW0015", "CWFRR000000000015", 2050.00m, 2017 },
                    { new Guid("10000000-0000-4000-8000-000000000016"), new Guid("10000000-0000-5000-9000-000000000004"), "Benjamin Scott", "CW0016", "CWFRR000000000016", 2600.00m, 2021 },
                    { new Guid("10000000-0000-4000-8000-000000000017"), new Guid("10000000-0000-5000-9000-000000000005"), "Harper Young", "CW0017", "CWTYT000000000017", 2500.01m, 2019 },
                    { new Guid("10000000-0000-4000-8000-000000000018"), new Guid("10000000-0000-5000-9000-000000000005"), "Henry Adams", "CW0018", "CWTYT000000000018", 3200.00m, 2023 },
                    { new Guid("10000000-0000-4000-8000-000000000019"), new Guid("10000000-0000-5000-9000-000000000005"), "Ruby Campbell", "CW0019", "CWTYT000000000019", 499.99m, 2016 },
                    { new Guid("10000000-0000-4000-8000-000000000020"), new Guid("10000000-0000-5000-9000-000000000005"), "Daniel Morgan", "CW0020", "CWTYT000000000020", 5800.00m, 2025 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Manufacturers_Name",
                table: "Manufacturers",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_ManufacturerId",
                table: "Vehicles",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_RegistrationNumber",
                table: "Vehicles",
                column: "RegistrationNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_Vin",
                table: "Vehicles",
                column: "Vin",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VehicleCategories");

            migrationBuilder.DropTable(
                name: "Vehicles");

            migrationBuilder.DropTable(
                name: "Manufacturers");
        }
    }
}
