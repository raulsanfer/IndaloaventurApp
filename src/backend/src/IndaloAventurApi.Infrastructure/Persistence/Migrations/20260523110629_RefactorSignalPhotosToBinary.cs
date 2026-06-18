using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IndaloAventurApi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RefactorSignalPhotosToBinary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Foto1",
                table: "Signals",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Foto2",
                table: "Signals",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE [Signals]
                SET [Foto1] = 0x,
                    [Foto2] = 0x
                WHERE [Foto1] IS NULL OR [Foto2] IS NULL;
                """);

            migrationBuilder.AlterColumn<byte[]>(
                name: "Foto1",
                table: "Signals",
                type: "varbinary(max)",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "Foto2",
                table: "Signals",
                type: "varbinary(max)",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(max)",
                oldNullable: true);

            migrationBuilder.DropColumn(
                name: "Fotos",
                table: "Signals");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Foto1",
                table: "Signals");

            migrationBuilder.DropColumn(
                name: "Foto2",
                table: "Signals");

            migrationBuilder.AddColumn<string>(
                name: "Fotos",
                table: "Signals",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: false,
                defaultValue: "");
        }
    }
}
