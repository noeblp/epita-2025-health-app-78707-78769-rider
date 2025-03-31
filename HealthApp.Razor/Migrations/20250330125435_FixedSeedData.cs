using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HealthApp.Razor.Migrations
{
    /// <inheritdoc />
    public partial class FixedSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1a28dd30-d5f2-42e5-b6b6-ada514479012");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6266fa61-e054-4f9d-8e04-c0f90ecde3d4");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c618474f-3747-496d-9a31-c49471847f0a");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1a28dd30-d5f2-42e5-b6b6-ada514479012", null, "Admin", "ADMIN" },
                    { "6266fa61-e054-4f9d-8e04-c0f90ecde3d4", null, "Patient", "PATIENT" },
                    { "c618474f-3747-496d-9a31-c49471847f0a", null, "Doctor", "DOCTOR" }
                });
        }
    }
}
