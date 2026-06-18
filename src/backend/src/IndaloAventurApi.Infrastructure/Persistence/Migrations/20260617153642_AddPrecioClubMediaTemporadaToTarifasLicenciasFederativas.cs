using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IndaloAventurApi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPrecioClubMediaTemporadaToTarifasLicenciasFederativas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PrecioClubMediaTemporada",
                table: "TarifasLicenciasFederativas",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "TarifasLicenciasFederativas",
                keyColumn: "Id",
                keyValue: 1,
                column: "PrecioClubMediaTemporada",
                value: 45.0m);

            migrationBuilder.UpdateData(
                table: "TarifasLicenciasFederativas",
                keyColumn: "Id",
                keyValue: 2,
                column: "PrecioClubMediaTemporada",
                value: 11.0m);

            migrationBuilder.UpdateData(
                table: "TarifasLicenciasFederativas",
                keyColumn: "Id",
                keyValue: 3,
                column: "PrecioClubMediaTemporada",
                value: 11.0m);

            migrationBuilder.UpdateData(
                table: "TarifasLicenciasFederativas",
                keyColumn: "Id",
                keyValue: 4,
                column: "PrecioClubMediaTemporada",
                value: 58.0m);

            migrationBuilder.UpdateData(
                table: "TarifasLicenciasFederativas",
                keyColumn: "Id",
                keyValue: 5,
                column: "PrecioClubMediaTemporada",
                value: 26.0m);

            migrationBuilder.UpdateData(
                table: "TarifasLicenciasFederativas",
                keyColumn: "Id",
                keyValue: 6,
                column: "PrecioClubMediaTemporada",
                value: 26.0m);

            migrationBuilder.UpdateData(
                table: "TarifasLicenciasFederativas",
                keyColumn: "Id",
                keyValue: 7,
                column: "PrecioClubMediaTemporada",
                value: 43.0m);

            migrationBuilder.UpdateData(
                table: "TarifasLicenciasFederativas",
                keyColumn: "Id",
                keyValue: 8,
                column: "PrecioClubMediaTemporada",
                value: 9.0m);

            migrationBuilder.UpdateData(
                table: "TarifasLicenciasFederativas",
                keyColumn: "Id",
                keyValue: 9,
                column: "PrecioClubMediaTemporada",
                value: 9.0m);

            migrationBuilder.UpdateData(
                table: "TarifasLicenciasFederativas",
                keyColumn: "Id",
                keyValue: 10,
                column: "PrecioClubMediaTemporada",
                value: 19.0m);

            migrationBuilder.UpdateData(
                table: "TarifasLicenciasFederativas",
                keyColumn: "Id",
                keyValue: 11,
                column: "PrecioClubMediaTemporada",
                value: 8.5m);

            migrationBuilder.UpdateData(
                table: "TarifasLicenciasFederativas",
                keyColumn: "Id",
                keyValue: 12,
                column: "PrecioClubMediaTemporada",
                value: 8.5m);

            migrationBuilder.UpdateData(
                table: "TarifasLicenciasFederativas",
                keyColumn: "Id",
                keyValue: 13,
                column: "PrecioClubMediaTemporada",
                value: 33.0m);

            migrationBuilder.UpdateData(
                table: "TarifasLicenciasFederativas",
                keyColumn: "Id",
                keyValue: 14,
                column: "PrecioClubMediaTemporada",
                value: 23.0m);

            migrationBuilder.UpdateData(
                table: "TarifasLicenciasFederativas",
                keyColumn: "Id",
                keyValue: 15,
                column: "PrecioClubMediaTemporada",
                value: 23.0m);

            migrationBuilder.UpdateData(
                table: "TarifasLicenciasFederativas",
                keyColumn: "Id",
                keyValue: 16,
                column: "PrecioClubMediaTemporada",
                value: 28.0m);

            migrationBuilder.UpdateData(
                table: "TarifasLicenciasFederativas",
                keyColumn: "Id",
                keyValue: 17,
                column: "PrecioClubMediaTemporada",
                value: 32.0m);

            migrationBuilder.UpdateData(
                table: "TarifasLicenciasFederativas",
                keyColumn: "Id",
                keyValue: 18,
                column: "PrecioClubMediaTemporada",
                value: 8.5m);

            migrationBuilder.UpdateData(
                table: "TarifasLicenciasFederativas",
                keyColumn: "Id",
                keyValue: 19,
                column: "PrecioClubMediaTemporada",
                value: 8.5m);

            migrationBuilder.UpdateData(
                table: "TarifasLicenciasFederativas",
                keyColumn: "Id",
                keyValue: 20,
                column: "PrecioClubMediaTemporada",
                value: 52.0m);

            migrationBuilder.UpdateData(
                table: "TarifasLicenciasFederativas",
                keyColumn: "Id",
                keyValue: 21,
                column: "PrecioClubMediaTemporada",
                value: 22.0m);

            migrationBuilder.UpdateData(
                table: "TarifasLicenciasFederativas",
                keyColumn: "Id",
                keyValue: 22,
                column: "PrecioClubMediaTemporada",
                value: 22.0m);

            migrationBuilder.UpdateData(
                table: "TarifasLicenciasFederativas",
                keyColumn: "Id",
                keyValue: 23,
                column: "PrecioClubMediaTemporada",
                value: 69.0m);

            migrationBuilder.UpdateData(
                table: "TarifasLicenciasFederativas",
                keyColumn: "Id",
                keyValue: 24,
                column: "PrecioClubMediaTemporada",
                value: 36.0m);

            migrationBuilder.UpdateData(
                table: "TarifasLicenciasFederativas",
                keyColumn: "Id",
                keyValue: 25,
                column: "PrecioClubMediaTemporada",
                value: 36.0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrecioClubMediaTemporada",
                table: "TarifasLicenciasFederativas");
        }
    }
}
