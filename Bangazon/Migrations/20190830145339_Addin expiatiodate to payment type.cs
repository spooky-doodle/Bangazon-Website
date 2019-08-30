using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Bangazon.Migrations
{
    public partial class Addinexpiatiodatetopaymenttype : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpirationDate",
                table: "PaymentType",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "00000000-ffff-ffff-ffff-ffffffffffff",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "1e1c92bf-460c-4bfb-b7c0-49fbf77128cd", "AQAAAAEAACcQAAAAEJF5+Fm3EOJ9Xp6dbUoBPn8LesXtRqhSuUQi6YAPthFQ+rczkFvmjqAfWhYqKa4i+g==" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpirationDate",
                table: "PaymentType");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "00000000-ffff-ffff-ffff-ffffffffffff",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "87e33c2f-5388-4574-9fc7-7def7502fe0b", "AQAAAAEAACcQAAAAEAGCU0mg9yqG/0kn9Njhr6I6vTmKvSMV4XKHkFgTuCzKVkeXkDYbwq8V2KkFOt03BA==" });
        }
    }
}
