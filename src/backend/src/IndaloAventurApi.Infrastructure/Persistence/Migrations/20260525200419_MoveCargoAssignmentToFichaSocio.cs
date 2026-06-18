using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IndaloAventurApi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MoveCargoAssignmentToFichaSocio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CargoId",
                table: "FichasSocio",
                type: "int",
                nullable: true);

            migrationBuilder.Sql(
                """
                IF COL_LENGTH('AspNetUsers', 'CargoId') IS NOT NULL
                BEGIN
                    EXEC(N'
                        UPDATE fs
                        SET fs.CargoId = u.CargoId
                        FROM FichasSocio fs
                        INNER JOIN AspNetUsers u ON fs.UserId = u.Id
                        WHERE u.CargoId IS NOT NULL;
                    ');
                END
                """);

            migrationBuilder.CreateIndex(
                name: "IX_FichasSocio_CargoId",
                table: "FichasSocio",
                column: "CargoId");

            migrationBuilder.AddForeignKey(
                name: "FK_FichasSocio_Cargos_CargoId",
                table: "FichasSocio",
                column: "CargoId",
                principalTable: "Cargos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql(
                """
                IF COL_LENGTH('AspNetUsers', 'CargoId') IS NOT NULL
                BEGIN
                    IF EXISTS (
                        SELECT 1
                        FROM sys.foreign_keys
                        WHERE name = N'FK_AspNetUsers_Cargos_CargoId'
                    )
                    BEGIN
                        ALTER TABLE AspNetUsers DROP CONSTRAINT [FK_AspNetUsers_Cargos_CargoId];
                    END

                    IF EXISTS (
                        SELECT 1
                        FROM sys.indexes
                        WHERE name = N'IX_AspNetUsers_CargoId'
                          AND object_id = OBJECT_ID(N'[AspNetUsers]')
                    )
                    BEGIN
                        DROP INDEX [IX_AspNetUsers_CargoId] ON [AspNetUsers];
                    END

                    ALTER TABLE AspNetUsers DROP COLUMN CargoId;
                END
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FichasSocio_Cargos_CargoId",
                table: "FichasSocio");

            migrationBuilder.DropIndex(
                name: "IX_FichasSocio_CargoId",
                table: "FichasSocio");

            migrationBuilder.DropColumn(
                name: "CargoId",
                table: "FichasSocio");
        }
    }
}
