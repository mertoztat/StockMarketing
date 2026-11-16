using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockMarkets.Migrations
{
    /// <inheritdoc />
    /// <remarks>
    /// Hand-edited. The scaffolded version deleted the old (random-Id) roles and inserted the new ones,
    /// which would cascade-delete AspNetUserRoles rows and fail on the unique RoleNameIndex.
    /// Here every role is replaced in four steps so existing user-role links are kept:
    /// rename old -> insert new -> repoint references -> delete old.
    /// </remarks>
    public partial class FixedRoleIds : Migration
    {
        private const string OldAdminId = "aa8318e0-85bd-4b82-ad4f-6dc060883c42";
        private const string OldUserId = "ed0ed828-19b6-4c54-b8de-8d0d6f712829";
        private const string NewAdminId = "83914810-9a2d-429f-8e7e-0297b0989493";
        private const string NewUserId = "938d001f-ef22-4687-8bd9-ec3f9027b00c";

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            ReplaceRole(migrationBuilder, OldAdminId, NewAdminId, "Admin", "ADMIN");
            ReplaceRole(migrationBuilder, OldUserId, NewUserId, "User", "USER");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            ReplaceRole(migrationBuilder, NewAdminId, OldAdminId, "Admin", "ADMIN");
            ReplaceRole(migrationBuilder, NewUserId, OldUserId, "User", "USER");
        }

        private static void ReplaceRole(
            MigrationBuilder migrationBuilder, string fromId, string toId, string name, string normalizedName)
        {
            // 1. Free the unique name so the replacement role can use it.
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: fromId,
                columns: new[] { "Name", "NormalizedName" },
                values: new object[] { name + "_OLD", normalizedName + "_OLD" });

            // 2. Insert the replacement role.
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { toId, null, name, normalizedName });

            // 3. Move everything that references the old role to the new one.
            migrationBuilder.Sql(
                $"UPDATE AspNetUserRoles SET RoleId = '{toId}' WHERE RoleId = '{fromId}'");
            migrationBuilder.Sql(
                $"UPDATE AspNetRoleClaims SET RoleId = '{toId}' WHERE RoleId = '{fromId}'");

            // 4. Remove the old role.
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: fromId);
        }
    }
}
