using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IndaloAventurApi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveFederativeLicenseAnioVigencia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TarifasLicenciasFederativas_Temporada_AnioVigencia_Licencia_Categoria",
                table: "TarifasLicenciasFederativas");

            migrationBuilder.DropColumn(
                name: "AnioVigencia",
                table: "TarifasLicenciasFederativas");

            migrationBuilder.CreateIndex(
                name: "IX_TarifasLicenciasFederativas_Temporada_Licencia_Categoria",
                table: "TarifasLicenciasFederativas",
                columns: new[] { "Temporada", "Licencia", "Categoria" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TarifasLicenciasFederativas_Temporada_Licencia_Categoria",
                table: "TarifasLicenciasFederativas");

            migrationBuilder.AddColumn<int>(
                name: "AnioVigencia",
                table: "TarifasLicenciasFederativas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "TarifasLicenciasFederativas",
                keyColumn: "Id",
                keyValue: 1,
                column: "AnioVigencia",
                value: 2026);

            migrationBuilder.UpdateData(
                table: "TarifasLicenciasFederativas",
                keyColumn: "Id",
                keyValue: 2,
                column: "AnioVigencia",
                value: 2026);

            migrationBuilder.UpdateData(
                table: "TarifasLicenciasFederativas",
                keyColumn: "Id",
                keyValue: 3,
                column: "AnioVigencia",
                value: 2026);

            migrationBuilder.UpdateData(
                table: "TarifasLicenciasFederativas",
                keyColumn: "Id",
                keyValue: 4,
                column: "AnioVigencia",
                value: 2026);

            migrationBuilder.UpdateData(
                table: "TarifasLicenciasFederativas",
                keyColumn: "Id",
                keyValue: 5,
                column: "AnioVigencia",
                value: 2026);

            migrationBuilder.UpdateData(
                table: "TarifasLicenciasFederativas",
                keyColumn: "Id",
                keyValue: 6,
                column: "AnioVigencia",
                value: 2026);

            migrationBuilder.UpdateData(
                table: "TarifasLicenciasFederativas",
                keyColumn: "Id",
                keyValue: 7,
                column: "AnioVigencia",
                value: 2026);

            migrationBuilder.UpdateData(
                table: "TarifasLicenciasFederativas",
                keyColumn: "Id",
                keyValue: 8,
                column: "AnioVigencia",
                value: 2026);

            migrationBuilder.UpdateData(
                table: "TarifasLicenciasFederativas",
                keyColumn: "Id",
                keyValue: 9,
                column: "AnioVigencia",
                value: 2026);

            migrationBuilder.UpdateData(
                table: "TarifasLicenciasFederativas",
                keyColumn: "Id",
                keyValue: 10,
                column: "AnioVigencia",
                value: 2026);

            migrationBuilder.UpdateData(
                table: "TarifasLicenciasFederativas",
                keyColumn: "Id",
                keyValue: 11,
                column: "AnioVigencia",
                value: 2026);

            migrationBuilder.UpdateData(
                table: "TarifasLicenciasFederativas",
                keyColumn: "Id",
                keyValue: 12,
                column: "AnioVigencia",
                value: 2026);

            migrationBuilder.UpdateData(
                table: "TarifasLicenciasFederativas",
                keyColumn: "Id",
                keyValue: 13,
                column: "AnioVigencia",
                value: 2026);

            migrationBuilder.UpdateData(
                table: "TarifasLicenciasFederativas",
                keyColumn: "Id",
                keyValue: 14,
                column: "AnioVigencia",
                value: 2026);

            migrationBuilder.UpdateData(
                table: "TarifasLicenciasFederativas",
                keyColumn: "Id",
                keyValue: 15,
                column: "AnioVigencia",
                value: 2026);

            migrationBuilder.UpdateData(
                table: "TarifasLicenciasFederativas",
                keyColumn: "Id",
                keyValue: 16,
                column: "AnioVigencia",
                value: 2026);

            migrationBuilder.UpdateData(
                table: "TarifasLicenciasFederativas",
                keyColumn: "Id",
                keyValue: 17,
                column: "AnioVigencia",
                value: 2026);

            migrationBuilder.UpdateData(
                table: "TarifasLicenciasFederativas",
                keyColumn: "Id",
                keyValue: 18,
                column: "AnioVigencia",
                value: 2026);

            migrationBuilder.UpdateData(
                table: "TarifasLicenciasFederativas",
                keyColumn: "Id",
                keyValue: 19,
                column: "AnioVigencia",
                value: 2026);

            migrationBuilder.UpdateData(
                table: "TarifasLicenciasFederativas",
                keyColumn: "Id",
                keyValue: 20,
                column: "AnioVigencia",
                value: 2026);

            migrationBuilder.UpdateData(
                table: "TarifasLicenciasFederativas",
                keyColumn: "Id",
                keyValue: 21,
                column: "AnioVigencia",
                value: 2026);

            migrationBuilder.UpdateData(
                table: "TarifasLicenciasFederativas",
                keyColumn: "Id",
                keyValue: 22,
                column: "AnioVigencia",
                value: 2026);

            migrationBuilder.UpdateData(
                table: "TarifasLicenciasFederativas",
                keyColumn: "Id",
                keyValue: 23,
                column: "AnioVigencia",
                value: 2026);

            migrationBuilder.UpdateData(
                table: "TarifasLicenciasFederativas",
                keyColumn: "Id",
                keyValue: 24,
                column: "AnioVigencia",
                value: 2026);

            migrationBuilder.UpdateData(
                table: "TarifasLicenciasFederativas",
                keyColumn: "Id",
                keyValue: 25,
                column: "AnioVigencia",
                value: 2026);

            migrationBuilder.CreateIndex(
                name: "IX_TarifasLicenciasFederativas_Temporada_AnioVigencia_Licencia_Categoria",
                table: "TarifasLicenciasFederativas",
                columns: new[] { "Temporada", "AnioVigencia", "Licencia", "Categoria" },
                unique: true);
        }
    }
}
