using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IndaloAventurApi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFederativeLicenses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TarifasLicenciasFederativas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Temporada = table.Column<int>(type: "int", nullable: false),
                    AnioVigencia = table.Column<int>(type: "int", nullable: false),
                    Licencia = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Categoria = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    PrecioClub = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    PrecioIndependiente = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    Territorio = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TarifasLicenciasFederativas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudesLicenciasFederativas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Temporada = table.Column<int>(type: "int", nullable: false),
                    TarifaLicenciaFederativaId = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudesLicenciasFederativas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolicitudesLicenciasFederativas_TarifasLicenciasFederativas_TarifaLicenciaFederativaId",
                        column: x => x.TarifaLicenciaFederativaId,
                        principalTable: "TarifasLicenciasFederativas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "TarifasLicenciasFederativas",
                columns: new[] { "Id", "AnioVigencia", "Categoria", "Licencia", "PrecioClub", "PrecioIndependiente", "Temporada", "Territorio" },
                values: new object[,]
                {
                    { 1, 2026, "Mayores", "A", 45.0m, 65.0m, 2026, "Andalucia, Ceuta y Melilla" },
                    { 2, 2026, "Juveniles", "A", 11.0m, 31.0m, 2026, "Andalucia, Ceuta y Melilla" },
                    { 3, 2026, "Infantiles", "A", 11.0m, 31.0m, 2026, "Andalucia, Ceuta y Melilla" },
                    { 4, 2026, "Mayores", "A+", 58.0m, 78.0m, 2026, "Andalucia, Ceuta y Melilla" },
                    { 5, 2026, "Juveniles", "A+", 26.0m, 46.0m, 2026, "Andalucia, Ceuta y Melilla" },
                    { 6, 2026, "Infantiles", "A+", 26.0m, 46.0m, 2026, "Andalucia, Ceuta y Melilla" },
                    { 7, 2026, "Mayores", "A Familiar", 43.0m, 63.0m, 2026, "Andalucia, Ceuta y Melilla" },
                    { 8, 2026, "Juveniles", "A Familiar", 9.0m, 29.0m, 2026, "Andalucia, Ceuta y Melilla" },
                    { 9, 2026, "Infantiles", "A Familiar", 9.0m, 29.0m, 2026, "Andalucia, Ceuta y Melilla" },
                    { 10, 2026, "Mayores", "A-IS Inclusion Social", 19.0m, null, 2026, "Andalucia, Ceuta y Melilla" },
                    { 11, 2026, "Juveniles", "A-IS Inclusion Social", 8.5m, null, 2026, "Andalucia, Ceuta y Melilla" },
                    { 12, 2026, "Infantiles", "A-IS Inclusion Social", 8.5m, null, 2026, "Andalucia, Ceuta y Melilla" },
                    { 13, 2026, "Mayores", "A+IS Inclusion Social", 33.0m, null, 2026, "Andalucia, Ceuta y Melilla" },
                    { 14, 2026, "Juveniles", "A+IS Inclusion Social", 23.0m, null, 2026, "Andalucia, Ceuta y Melilla" },
                    { 15, 2026, "Infantiles", "A+IS Inclusion Social", 23.0m, null, 2026, "Andalucia, Ceuta y Melilla" },
                    { 16, 2026, "Mayores", "A ESP", 28.0m, null, 2026, "Andalucia, Ceuta y Melilla" },
                    { 17, 2026, "Mayores", "A65", 32.0m, 52.0m, 2026, "Andalucia, Ceuta y Melilla" },
                    { 18, 2026, "Juveniles", "A REF", 8.5m, null, 2026, "Andalucia, Ceuta y Melilla" },
                    { 19, 2026, "Infantiles", "A REF", 8.5m, null, 2026, "Andalucia, Ceuta y Melilla" },
                    { 20, 2026, "Mayores", "A NAC", 52.0m, 72.0m, 2026, "Espana, Andorra, Pirineo Frances, Portugal y Marruecos" },
                    { 21, 2026, "Juveniles", "A NAC", 22.0m, 42.0m, 2026, "Espana, Andorra, Pirineo Frances, Portugal y Marruecos" },
                    { 22, 2026, "Infantiles", "A NAC", 22.0m, 42.0m, 2026, "Espana, Andorra, Pirineo Frances, Portugal y Marruecos" },
                    { 23, 2026, "Mayores", "A NAC +", 69.0m, 89.0m, 2026, "Espana, Andorra, Pirineo Frances, Portugal y Marruecos" },
                    { 24, 2026, "Juveniles", "A NAC +", 36.0m, 56.0m, 2026, "Espana, Andorra, Pirineo Frances, Portugal y Marruecos" },
                    { 25, 2026, "Infantiles", "A NAC +", 36.0m, 56.0m, 2026, "Espana, Andorra, Pirineo Frances, Portugal y Marruecos" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesLicenciasFederativas_TarifaLicenciaFederativaId",
                table: "SolicitudesLicenciasFederativas",
                column: "TarifaLicenciaFederativaId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesLicenciasFederativas_UserId_Temporada",
                table: "SolicitudesLicenciasFederativas",
                columns: new[] { "UserId", "Temporada" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TarifasLicenciasFederativas_Temporada_AnioVigencia_Licencia_Categoria",
                table: "TarifasLicenciasFederativas",
                columns: new[] { "Temporada", "AnioVigencia", "Licencia", "Categoria" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SolicitudesLicenciasFederativas");

            migrationBuilder.DropTable(
                name: "TarifasLicenciasFederativas");
        }
    }
}
