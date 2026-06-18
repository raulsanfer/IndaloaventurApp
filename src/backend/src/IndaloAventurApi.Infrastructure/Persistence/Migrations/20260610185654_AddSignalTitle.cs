using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IndaloAventurApi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSignalTitle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Titulo",
                table: "Signals",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE [Signals]
                SET [Titulo] = [Descripcion]
                WHERE [Titulo] IS NULL
                """);

            migrationBuilder.AlterColumn<string>(
                name: "Titulo",
                table: "Signals",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Titulo",
                table: "Signals");
        }
    }
}
