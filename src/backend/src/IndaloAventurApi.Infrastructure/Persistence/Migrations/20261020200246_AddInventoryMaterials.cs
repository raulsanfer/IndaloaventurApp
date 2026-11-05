using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IndaloAventurApi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryMaterials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InventarioMateriales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    MarcaModelo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    CategoriaId = table.Column<int>(type: "int", nullable: false),
                    EstadoId = table.Column<int>(type: "int", nullable: false),
                    FechaAdquisicion = table.Column<DateOnly>(type: "date", nullable: true),
                    AdquiridoDonado = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    NumeroSerie = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    NumeroSerieNormalizado = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventarioMateriales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventarioMateriales_InventarioCategorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "InventarioCategorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventarioMateriales_InventarioEstados_EstadoId",
                        column: x => x.EstadoId,
                        principalTable: "InventarioEstados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventarioMateriales_CategoriaId",
                table: "InventarioMateriales",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_InventarioMateriales_EstadoId",
                table: "InventarioMateriales",
                column: "EstadoId");

            migrationBuilder.CreateIndex(
                name: "IX_InventarioMateriales_NumeroSerieNormalizado",
                table: "InventarioMateriales",
                column: "NumeroSerieNormalizado",
                unique: true,
                filter: "[NumeroSerieNormalizado] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InventarioMateriales");
        }
    }
}
