using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalFinanceTracker_EnterpriseEdition.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("1c95b1ff-39be-4c2b-8b40-2ce4818573ce"));

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "Users",
                newName: "Username");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "DeletedById", "Email", "IsDeleted", "Password", "Role", "Username" },
                values: new object[] { new Guid("4342af7d-9c75-47d9-a3d4-8c33898c4e77"), new DateTime(2025, 7, 15, 14, 24, 3, 68, DateTimeKind.Utc).AddTicks(8431), null, null, "boqiyev482@gmail.com", false, "$2b$10$l8.kb93NELpcC04DNcMdNOCx5qunKEBhGjZSy1NSL0THggMCR3etG", 20, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("4342af7d-9c75-47d9-a3d4-8c33898c4e77"));

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Users",
                newName: "UserName");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "DeletedById", "Email", "FirstName", "IsDeleted", "LastName", "Password", "Role", "UserName" },
                values: new object[] { new Guid("1c95b1ff-39be-4c2b-8b40-2ce4818573ce"), new DateTime(2025, 7, 14, 17, 19, 22, 46, DateTimeKind.Utc).AddTicks(7023), null, null, "boqiyev482@gmail.com", "Admin", false, null, "$2b$10$uikcMt2Z3WtLye15MeZMl.MoqqQrvGT3Ls3.dKdcwV7U9YWF1bWHe", 20, null });
        }
    }
}
