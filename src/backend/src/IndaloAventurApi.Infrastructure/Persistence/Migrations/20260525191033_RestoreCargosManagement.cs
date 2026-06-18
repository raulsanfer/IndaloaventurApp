using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IndaloAventurApi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RestoreCargosManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF OBJECT_ID(N'[Cargos]', N'U') IS NULL
                BEGIN
                    CREATE TABLE [Cargos](
                        [Id] INT NOT NULL,
                        [Descripcion] NVARCHAR(200) NOT NULL,
                        CONSTRAINT [PK_Cargos] PRIMARY KEY ([Id])
                    );
                END
                """);

            migrationBuilder.Sql(
                """
                IF NOT EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE name = N'IX_Cargos_Descripcion'
                      AND object_id = OBJECT_ID(N'[Cargos]')
                )
                BEGIN
                    CREATE UNIQUE INDEX [IX_Cargos_Descripcion] ON [Cargos] ([Descripcion]);
                END
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF OBJECT_ID(N'[Cargos]', N'U') IS NOT NULL
                BEGIN
                    DROP TABLE [Cargos];
                END
                """);
        }
    }
}
