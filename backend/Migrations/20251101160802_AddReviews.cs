using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddReviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                column: "PasswordHash",
                value: "$2a$11$AyYyz1fFYkZSyy0cPW5Fke0LnHsfmzGqVf0BbYz.6EBs7Gk8VsNOi");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2L,
                column: "PasswordHash",
                value: "$2a$11$./hbegWTXObMiesQ/CyVY.qJ4eAMXgwmzg80Su0Ha8UfZnylTM1Wy");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3L,
                column: "PasswordHash",
                value: "$2a$11$3fKpbcjMmjgrl..PScYD7OCYW3Vapg6EK1r3RHmJLXOgXPtigILp2");
        }
    }
}
