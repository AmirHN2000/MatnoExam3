using Microsoft.EntityFrameworkCore.Migrations;

namespace Competition3.Migrations
{
    public partial class changePayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RefCode",
                table: "Payments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "d6bcc0c2-9b20-425f-8c0e-06d2b26048d9");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "a74dfc3e-d712-4166-be57-d8464ca214b2");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d7a3f653-455e-485f-a73a-b7505a3b82ad", "AQAAAAEAACcQAAAAEAV1a4EtvGh63eHyXhM7YiAwCta0pWKKkMYwGx6XYFhg4nW6EWfspMBf6PfeK33r5Q==", "40e8bc1f-6b2f-4236-926f-d47ae4640fb4" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RefCode",
                table: "Payments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "bd59eab3-a299-49a9-be3a-7b6558b42fa3");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "050dea2c-d1f5-481b-9969-4bb0b666d612");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "25496783-e01f-4662-b2c0-7d12c346e034", "AQAAAAEAACcQAAAAEL5asLMllHDsw0rP1v2O3a4Ug6cn7NawRlCNXVNiaThFEmd9CWswBljy5NElyE+puA==", "642c84b2-6690-411f-9c48-df7602ac1983" });
        }
    }
}
