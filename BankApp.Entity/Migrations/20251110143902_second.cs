using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BankApp.Entity.Migrations
{
    /// <inheritdoc />
    public partial class second : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4be98c7f-8d54-4972-9135-6f978e7f4ac0");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4fc025c3-17b0-4ab3-95f0-6a07bdf69dd9");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "9a87a9cc-8b06-4164-9dd1-a67047b370a8", "b67760e3-e833-4c61-981e-c22db10d2c6c" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9a87a9cc-8b06-4164-9dd1-a67047b370a8");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b67760e3-e833-4c61-981e-c22db10d2c6c");

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "tblCustomers",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "tblCustomers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MobileNumber",
                table: "tblCustomers",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "AccountTypeID",
                table: "tblCustomerApplications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "tblCustomerApplications",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MobileNumber",
                table: "tblCustomerApplications",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "tblAccounts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "a6dd1722-0b7e-4af3-a8a7-444993b28862", null, "Customer", "CUSTOMER" },
                    { "a7998ed9-bf16-4d1d-8c74-8b2b192d3bb6", null, "Manager", "MANAGER" },
                    { "d0e2e62d-adc1-452b-9b46-f5b219692f16", null, "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "Email", "EmailConfirmed", "FullName", "IsActive", "IsDeleted", "LockoutEnabled", "LockoutEnd", "ModifiedBy", "ModifiedDate", "MustChangePassword", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "427b9f59-35f1-4ada-a8f1-4bf568b414ad", 0, "d9de921a-8fa4-43b5-bbf0-d794d4cca96e", "System", new DateTime(2025, 11, 10, 20, 8, 59, 708, DateTimeKind.Local).AddTicks(4115), null, null, "admin@bankapp.com", true, "System Administrator", true, false, false, null, null, null, false, "ADMIN@BANKAPP.COM", "ADMIN", "AQAAAAIAAYagAAAAECsM4iCfXUMdqtxHWskH1s1cjgTX8yC7vHrAvkvGLUvueTVWmwm1Je1wdtxFtuENfw==", null, false, "2a4335fe-0201-4450-af99-0a1d41600308", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "d0e2e62d-adc1-452b-9b46-f5b219692f16", "427b9f59-35f1-4ada-a8f1-4bf568b414ad" });

            migrationBuilder.CreateIndex(
                name: "IX_tblCustomers_AadharNumber",
                table: "tblCustomers",
                column: "AadharNumber",
                unique: true,
                filter: "[IsDeleted]=0");

            migrationBuilder.CreateIndex(
                name: "IX_tblCustomers_MobileNumber",
                table: "tblCustomers",
                column: "MobileNumber",
                unique: true,
                filter: "[IsDeleted]=0");

            migrationBuilder.CreateIndex(
                name: "IX_tblCustomers_PAN",
                table: "tblCustomers",
                column: "PAN",
                unique: true,
                filter: "[IsDeleted]=0");

            migrationBuilder.CreateIndex(
                name: "IX_tblCustomerApplications_AadharNumber",
                table: "tblCustomerApplications",
                column: "AadharNumber");

            migrationBuilder.CreateIndex(
                name: "IX_tblCustomerApplications_MobileNumber",
                table: "tblCustomerApplications",
                column: "MobileNumber");

            migrationBuilder.CreateIndex(
                name: "IX_tblCustomerApplications_PAN",
                table: "tblCustomerApplications",
                column: "PAN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_tblCustomers_AadharNumber",
                table: "tblCustomers");

            migrationBuilder.DropIndex(
                name: "IX_tblCustomers_MobileNumber",
                table: "tblCustomers");

            migrationBuilder.DropIndex(
                name: "IX_tblCustomers_PAN",
                table: "tblCustomers");

            migrationBuilder.DropIndex(
                name: "IX_tblCustomerApplications_AadharNumber",
                table: "tblCustomerApplications");

            migrationBuilder.DropIndex(
                name: "IX_tblCustomerApplications_MobileNumber",
                table: "tblCustomerApplications");

            migrationBuilder.DropIndex(
                name: "IX_tblCustomerApplications_PAN",
                table: "tblCustomerApplications");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a6dd1722-0b7e-4af3-a8a7-444993b28862");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a7998ed9-bf16-4d1d-8c74-8b2b192d3bb6");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "d0e2e62d-adc1-452b-9b46-f5b219692f16", "427b9f59-35f1-4ada-a8f1-4bf568b414ad" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d0e2e62d-adc1-452b-9b46-f5b219692f16");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "427b9f59-35f1-4ada-a8f1-4bf568b414ad");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "tblCustomers");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "tblCustomers");

            migrationBuilder.DropColumn(
                name: "MobileNumber",
                table: "tblCustomers");

            migrationBuilder.DropColumn(
                name: "AccountTypeID",
                table: "tblCustomerApplications");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "tblCustomerApplications");

            migrationBuilder.DropColumn(
                name: "MobileNumber",
                table: "tblCustomerApplications");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "tblAccounts");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "4be98c7f-8d54-4972-9135-6f978e7f4ac0", null, "Customer", "CUSTOMER" },
                    { "4fc025c3-17b0-4ab3-95f0-6a07bdf69dd9", null, "Manager", "MANAGER" },
                    { "9a87a9cc-8b06-4164-9dd1-a67047b370a8", null, "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "Email", "EmailConfirmed", "FullName", "IsActive", "IsDeleted", "LockoutEnabled", "LockoutEnd", "ModifiedBy", "ModifiedDate", "MustChangePassword", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "b67760e3-e833-4c61-981e-c22db10d2c6c", 0, "740947ec-171d-494f-9a94-49aea77a49fe", "System", new DateTime(2025, 11, 8, 19, 52, 25, 410, DateTimeKind.Local).AddTicks(7057), null, null, "admin@bankapp.com", true, "System Administrator", true, false, false, null, null, null, false, "ADMIN@BANKAPP.COM", "ADMIN", "AQAAAAIAAYagAAAAEAYu9cRL+6O12rC+uMZChrwsXIa5yAywlkOCRoVYeJUHjK34wWA5f1dpIWLhpQcvmA==", null, false, "9b310979-da41-4204-aa49-1ce2718485d1", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "9a87a9cc-8b06-4164-9dd1-a67047b370a8", "b67760e3-e833-4c61-981e-c22db10d2c6c" });
        }
    }
}
