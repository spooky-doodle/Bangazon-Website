using Microsoft.EntityFrameworkCore.Migrations;

namespace Bangazon.Migrations
{
    public partial class addingopiniontotb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "00000000-ffff-ffff-ffff-ffffffffffff",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "5d63e953-07f4-4b56-b91d-44a2d1ff699e", "AQAAAAEAACcQAAAAECQheSWNuu93FZHMt0FcEoI5SIxeFyLS+cK9WlooYWKqB2qRXMS3sxF85t1hRRbTaw==" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "00000000-ffff-ffff-ffff-ffffffffffff",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "ac3e6024-68c6-4338-bbb2-8a7bd5754dae", "AQAAAAEAACcQAAAAEJGJPUkKPDq0cKU1M8ZidzQmHKif3XUL2EqYMOAnxCnExXl74cDjLytFgmgSdTZDug==" });
        }
    }
}
