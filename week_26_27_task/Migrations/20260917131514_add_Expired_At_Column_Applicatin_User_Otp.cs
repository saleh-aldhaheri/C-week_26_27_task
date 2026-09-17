using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace week_26_27.Migrations
{
    /// <inheritdoc />
    public partial class add_Expired_At_Column_Applicatin_User_Otp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiredAt",
                table: "ApplicationUserOtps",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpiredAt",
                table: "ApplicationUserOtps");
        }
    }
}
