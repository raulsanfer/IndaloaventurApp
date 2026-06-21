using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IndaloAventurApi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class StoreSignalImagesInFilesystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Foto1Path",
                table: "Signals",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Foto2Path",
                table: "Signals",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Foto1Path",
                table: "Signals");

            migrationBuilder.DropColumn(
                name: "Foto2Path",
                table: "Signals");

            migrationBuilder.Sql(
                """
                IF COL_LENGTH('Signals', 'Foto1') IS NULL
                BEGIN
                    ALTER TABLE [Signals] ADD [Foto1] varbinary(max) NOT NULL CONSTRAINT [DF_Signals_Foto1] DEFAULT 0x;
                END
                """);

            migrationBuilder.Sql(
                """
                IF COL_LENGTH('Signals', 'Foto2') IS NULL
                BEGIN
                    ALTER TABLE [Signals] ADD [Foto2] varbinary(max) NOT NULL CONSTRAINT [DF_Signals_Foto2] DEFAULT 0x;
                END
                """);
        }
    }
}
