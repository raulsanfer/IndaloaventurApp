using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IndaloAventurApi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RefactorTarifaLicenciaFederativaMediaTemporada : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TarifasLicenciasFederativas_Temporada_Licencia_Categoria",
                table: "TarifasLicenciasFederativas");

            migrationBuilder.AddColumn<bool>(
                name: "MediaTemporada",
                table: "TarifasLicenciasFederativas",
                type: "bit",
                nullable: false,
                defaultValue: false);


            migrationBuilder.Sql(
                """
                INSERT INTO TarifasLicenciasFederativas (Temporada, Licencia, Categoria, PrecioClub, PrecioIndependiente, Territorio, MediaTemporada)
                SELECT Temporada, Licencia, Categoria, PrecioClubMediaTemporada, PrecioIndependiente, Territorio, CAST(1 AS bit)
                FROM TarifasLicenciasFederativas
                WHERE MediaTemporada = 0;
                """);

            migrationBuilder.DropColumn(
                name: "PrecioClubMediaTemporada",
                table: "TarifasLicenciasFederativas");

            migrationBuilder.CreateIndex(
                name: "IX_TarifasLicenciasFederativas_Temporada_Licencia_Categoria_MediaTemporada",
                table: "TarifasLicenciasFederativas",
                columns: new[] { "Temporada", "Licencia", "Categoria", "MediaTemporada" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TarifasLicenciasFederativas_Temporada_Licencia_Categoria_MediaTemporada",
                table: "TarifasLicenciasFederativas");

            migrationBuilder.AddColumn<decimal>(
                name: "PrecioClubMediaTemporada",
                table: "TarifasLicenciasFederativas",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.Sql(
                """
                UPDATE temporadaCompleta
                SET temporadaCompleta.PrecioClubMediaTemporada = ISNULL(mediaTemporada.PrecioClub, temporadaCompleta.PrecioClub)
                FROM TarifasLicenciasFederativas AS temporadaCompleta
                LEFT JOIN TarifasLicenciasFederativas AS mediaTemporada
                    ON mediaTemporada.Temporada = temporadaCompleta.Temporada
                   AND mediaTemporada.Licencia = temporadaCompleta.Licencia
                   AND mediaTemporada.Categoria = temporadaCompleta.Categoria
                   AND mediaTemporada.MediaTemporada = 1
                WHERE temporadaCompleta.MediaTemporada = 0;

                DELETE FROM TarifasLicenciasFederativas
                WHERE MediaTemporada = 1;
                """);

            migrationBuilder.DropColumn(
                name: "MediaTemporada",
                table: "TarifasLicenciasFederativas");

            migrationBuilder.CreateIndex(
                name: "IX_TarifasLicenciasFederativas_Temporada_Licencia_Categoria",
                table: "TarifasLicenciasFederativas",
                columns: new[] { "Temporada", "Licencia", "Categoria" },
                unique: true);
        }
    }
}
