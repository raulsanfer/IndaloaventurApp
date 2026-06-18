using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IndaloAventurApi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAutoincrementToCargoAndSignalTypeIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FichasSocio_Cargos_CargoId",
                table: "FichasSocio");

            migrationBuilder.DropForeignKey(
                name: "FK_Signals_SignalTypes_Tipo",
                table: "Signals");

            migrationBuilder.RenameTable(
                name: "Cargos",
                newName: "Cargos_Legacy");

            migrationBuilder.RenameTable(
                name: "SignalTypes",
                newName: "SignalTypes_Legacy");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cargos",
                table: "Cargos_Legacy");

            migrationBuilder.DropIndex(
                name: "IX_Cargos_Descripcion",
                table: "Cargos_Legacy");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SignalTypes",
                table: "SignalTypes_Legacy");

            migrationBuilder.CreateTable(
                name: "Cargos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cargos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SignalTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Icono = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SignalTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cargos_Descripcion",
                table: "Cargos",
                column: "Descripcion",
                unique: true);

            migrationBuilder.Sql("""
                SET IDENTITY_INSERT [Cargos] ON;
                INSERT INTO [Cargos] ([Id], [Descripcion])
                SELECT [Id], [Descripcion]
                FROM [Cargos_Legacy];
                SET IDENTITY_INSERT [Cargos] OFF;
                """);

            migrationBuilder.Sql("""
                SET IDENTITY_INSERT [SignalTypes] ON;
                INSERT INTO [SignalTypes] ([Id], [Nombre], [Icono])
                SELECT [Id], [Nombre], [Icono]
                FROM [SignalTypes_Legacy];
                SET IDENTITY_INSERT [SignalTypes] OFF;
                """);

            migrationBuilder.DropTable(
                name: "Cargos_Legacy");

            migrationBuilder.DropTable(
                name: "SignalTypes_Legacy");

            migrationBuilder.AddForeignKey(
                name: "FK_FichasSocio_Cargos_CargoId",
                table: "FichasSocio",
                column: "CargoId",
                principalTable: "Cargos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Signals_SignalTypes_Tipo",
                table: "Signals",
                column: "Tipo",
                principalTable: "SignalTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FichasSocio_Cargos_CargoId",
                table: "FichasSocio");

            migrationBuilder.DropForeignKey(
                name: "FK_Signals_SignalTypes_Tipo",
                table: "Signals");

            migrationBuilder.RenameTable(
                name: "Cargos",
                newName: "Cargos_Identity");

            migrationBuilder.RenameTable(
                name: "SignalTypes",
                newName: "SignalTypes_Identity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cargos",
                table: "Cargos_Identity");

            migrationBuilder.DropIndex(
                name: "IX_Cargos_Descripcion",
                table: "Cargos_Identity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SignalTypes",
                table: "SignalTypes_Identity");

            migrationBuilder.CreateTable(
                name: "Cargos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cargos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SignalTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Icono = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SignalTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cargos_Descripcion",
                table: "Cargos",
                column: "Descripcion",
                unique: true);

            migrationBuilder.Sql("""
                INSERT INTO [Cargos] ([Id], [Descripcion])
                SELECT [Id], [Descripcion]
                FROM [Cargos_Identity];
                """);

            migrationBuilder.Sql("""
                INSERT INTO [SignalTypes] ([Id], [Nombre], [Icono])
                SELECT [Id], [Nombre], [Icono]
                FROM [SignalTypes_Identity];
                """);

            migrationBuilder.DropTable(
                name: "Cargos_Identity");

            migrationBuilder.DropTable(
                name: "SignalTypes_Identity");

            migrationBuilder.AddForeignKey(
                name: "FK_FichasSocio_Cargos_CargoId",
                table: "FichasSocio",
                column: "CargoId",
                principalTable: "Cargos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Signals_SignalTypes_Tipo",
                table: "Signals",
                column: "Tipo",
                principalTable: "SignalTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
