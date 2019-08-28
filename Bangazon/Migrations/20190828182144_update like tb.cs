using Microsoft.EntityFrameworkCore.Migrations;

namespace Bangazon.Migrations
{
    public partial class updateliketb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Opinion",
                table: "UserProductOpinions",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "00000000-ffff-ffff-ffff-ffffffffffff",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "a5f286bf-9aba-47c9-93c5-8e50ecfa7273", "AQAAAAEAACcQAAAAEOBsvStuYtEH7U8Zmeti2TZU6ny+ZLocp+enLLdaFQQJSjW5gZNwj6gUYHziyor9OA==" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Opinion",
                table: "UserProductOpinions");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "00000000-ffff-ffff-ffff-ffffffffffff",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "5d63e953-07f4-4b56-b91d-44a2d1ff699e", "AQAAAAEAACcQAAAAECQheSWNuu93FZHMt0FcEoI5SIxeFyLS+cK9WlooYWKqB2qRXMS3sxF85t1hRRbTaw==" });
        }
    }
}
