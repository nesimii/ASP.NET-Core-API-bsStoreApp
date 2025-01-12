using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    public partial class AddRolesToDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "620f25b3-acc2-4635-a7eb-51489ed77fef", "fcef20d1-e469-41a4-8e7d-41255a089cd6", "Administrator", "ADMINISTRATOR" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "6d3fd37c-bcd5-4bdb-aa73-01d9d6efb07c", "fc6c0419-86b8-4359-a51c-26ae0400ef83", "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "b7e28d8e-589f-42da-b37a-d4812aa48a12", "1a64919c-7748-46dc-8112-afe3ed86ce5b", "Editor", "EDITOR" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "620f25b3-acc2-4635-a7eb-51489ed77fef");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6d3fd37c-bcd5-4bdb-aa73-01d9d6efb07c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b7e28d8e-589f-42da-b37a-d4812aa48a12");
        }
    }
}
