using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IndaloAventurApi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUsuarioIsMember : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsMember",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMember",
                table: "AspNetUsers");
        }
    }
}
