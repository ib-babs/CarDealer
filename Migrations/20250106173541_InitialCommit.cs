using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarDealer.Migrations
{
    /// <inheritdoc />
    public partial class InitialCommit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CarItems",
                table: "CarItems");

            migrationBuilder.RenameTable(
                name: "CarItems",
                newName: "Cars");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cars",
                table: "Cars",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Cars",
                table: "Cars");

            migrationBuilder.RenameTable(
                name: "Cars",
                newName: "CarItems");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CarItems",
                table: "CarItems",
                column: "Id");
        }
    }
}
