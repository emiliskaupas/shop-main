using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                column: "PasswordHash",
                value: "$2a$11$BbMMt.42L2iXd91aCm2qKOWLpjB1NaSCV6.jiK/SqCZoWZPwm2Jv.");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2L,
                column: "PasswordHash",
                value: "$2a$11$Ii7loMC3ucGQYINIbAdU3./u1yIMwbvvr7CTwM/xxZ/8wZt.ri8/G");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3L,
                column: "PasswordHash",
                value: "$2a$11$f5mO1DIdGORYG/s99NU3POFJzg/R0VwgrYauFU2ilz1AxiudHrJpu");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                column: "PasswordHash",
                value: "$2a$11$dlhdCo0kUbquu2qtG2SUT.9/oYOU/Des4ROBs0bmqVk7ocwKiqdGi");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2L,
                column: "PasswordHash",
                value: "$2a$11$Q0T9jYtqYqGdb0zBN3WwfO0xfwpXvNgJNoGF..LNGIRYCh6wbJBS.");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3L,
                column: "PasswordHash",
                value: "$2a$11$FDxHRRGNl4nzIOhYAK2MseqBfAcAY2DMPBiBVdZz.y4cFn5NJdsH6");
        }
    }
}
