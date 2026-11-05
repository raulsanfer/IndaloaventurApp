using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IndaloAventurApi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryMaterialReservations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "InventarioMateriales",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateTable(
                name: "ReservasMaterial",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InventarioMaterialId = table.Column<int>(type: "int", nullable: false),
                    CantidadSolicitada = table.Column<int>(type: "int", nullable: false),
                    FechaSolicitudUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaRetiradaPrevista = table.Column<DateOnly>(type: "date", nullable: false),
                    FechaRetirada = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaDevolucionEstimada = table.Column<DateOnly>(type: "date", nullable: false),
                    FechaDevolucionEfectiva = table.Column<DateOnly>(type: "date", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ObservacionAdministracion = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    ConfirmacionDevolucionSocio = table.Column<bool>(type: "bit", nullable: false),
                    ConfirmacionDevolucionAdmin = table.Column<bool>(type: "bit", nullable: false),
                    ConcurrencyToken = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservasMaterial", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReservasMaterial_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReservasMaterial_InventarioMateriales_InventarioMaterialId",
                        column: x => x.InventarioMaterialId,
                        principalTable: "InventarioMateriales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReservasMaterial_InventarioMaterialId_Estado",
                table: "ReservasMaterial",
                columns: new[] { "InventarioMaterialId", "Estado" });

            migrationBuilder.CreateIndex(
                name: "IX_ReservasMaterial_UserId_FechaSolicitudUtc",
                table: "ReservasMaterial",
                columns: new[] { "UserId", "FechaSolicitudUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReservasMaterial");

            migrationBuilder.DropColumn(
                name: "Activo",
                table: "InventarioMateriales");
        }
    }
}
