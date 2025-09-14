using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HarikaYemekTarifleri.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddRecipePhotoUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl",
                table: "Recipes",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoUrl",
                table: "Recipes");
        }
    }
}
