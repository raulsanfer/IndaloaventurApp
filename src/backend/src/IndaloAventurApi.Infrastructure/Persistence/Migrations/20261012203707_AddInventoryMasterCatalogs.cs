using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IndaloAventurApi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryMasterCatalogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InventarioCategorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    NombreNormalizado = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    CategoriaPadreId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventarioCategorias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventarioCategorias_InventarioCategorias_CategoriaPadreId",
                        column: x => x.CategoriaPadreId,
                        principalTable: "InventarioCategorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InventarioEstados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    NombreNormalizado = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventarioEstados", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventarioCategorias_CategoriaPadreId_NombreNormalizado",
                table: "InventarioCategorias",
                columns: new[] { "CategoriaPadreId", "NombreNormalizado" },
                unique: true,
                filter: "[CategoriaPadreId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_InventarioCategorias_NombreNormalizado",
                table: "InventarioCategorias",
                column: "NombreNormalizado",
                unique: true,
                filter: "[CategoriaPadreId] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_InventarioEstados_NombreNormalizado",
                table: "InventarioEstados",
                column: "NombreNormalizado",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InventarioCategorias");

            migrationBuilder.DropTable(
                name: "InventarioEstados");
        }
    }
}
