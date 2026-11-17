using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IndaloAventurApi.Infrastructure.Persistence.Migrations;

public partial class AddFederativeLicenseTypes : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "TiposLicencias",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false),
                Nombre = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_TiposLicencias", x => x.Id));
        migrationBuilder.InsertData("TiposLicencias", new[] { "Id", "Nombre" }, new object[,]
        {
            { 1, "Completa" }, { 2, "Media 01/07 - 31/12" }, { 3, "Día" }
        });
        migrationBuilder.CreateIndex("IX_TiposLicencias_Nombre", "TiposLicencias", "Nombre", unique: true);

        migrationBuilder.AddColumn<int>("TipoLicenciaId", "TarifasLicenciasFederativas", type: "int", nullable: true);
        migrationBuilder.AddColumn<DateOnly>("FechaInicio", "SolicitudesLicenciasFederativas", type: "date", nullable: true);
        migrationBuilder.AddColumn<DateOnly>("FechaFin", "SolicitudesLicenciasFederativas", type: "date", nullable: true);

        // Convert every row, including manually maintained rates and other seasons.
        // Keep the legacy column until the backfill and validation have completed.
        ExecuteDeferred(migrationBuilder, """
            UPDATE TarifasLicenciasFederativas
            SET TipoLicenciaId = CASE WHEN MediaTemporada = 1 THEN 2 ELSE 1 END;
            IF EXISTS (SELECT 1 FROM TarifasLicenciasFederativas WHERE TipoLicenciaId IS NULL OR TipoLicenciaId NOT IN (1, 2))
                THROW 51000, N'No se ha podido asignar el tipo a todas las tarifas federativas.', 1;
            """);
        migrationBuilder.AlterColumn<int>("TipoLicenciaId", "TarifasLicenciasFederativas", type: "int", nullable: false,
            oldClrType: typeof(int), oldType: "int", oldNullable: true);
        migrationBuilder.AddForeignKey("FK_TarifasLicenciasFederativas_TiposLicencias_TipoLicenciaId",
            "TarifasLicenciasFederativas", "TipoLicenciaId", "TiposLicencias", principalColumn: "Id", onDelete: ReferentialAction.Restrict);
        migrationBuilder.CreateIndex("IX_TarifasLicenciasFederativas_TipoLicenciaId", "TarifasLicenciasFederativas", "TipoLicenciaId");
        migrationBuilder.DropIndex("IX_TarifasLicenciasFederativas_Temporada_Licencia_Categoria_MediaTemporada", "TarifasLicenciasFederativas");
        migrationBuilder.CreateIndex("IX_TarifasLicenciasFederativas_Temporada_TipoLicenciaId_Licencia_Categoria",
            "TarifasLicenciasFederativas", new[] { "Temporada", "TipoLicenciaId", "Licencia", "Categoria" }, unique: true);
        migrationBuilder.DropIndex("IX_SolicitudesLicenciasFederativas_UserId_Temporada", "SolicitudesLicenciasFederativas");
        migrationBuilder.CreateIndex("IX_SolicitudesLicenciasFederativas_UserId_Temporada", "SolicitudesLicenciasFederativas", new[] { "UserId", "Temporada" });
        migrationBuilder.AddCheckConstraint("CK_SolicitudesLicenciasFederativas_Cobertura", "SolicitudesLicenciasFederativas",
            "([FechaInicio] IS NULL AND [FechaFin] IS NULL) OR ([FechaInicio] IS NOT NULL AND [FechaFin] IS NOT NULL AND [FechaInicio] <= [FechaFin])");
        migrationBuilder.AddCheckConstraint("CK_SolicitudesLicenciasFederativas_CoberturaTemporada", "SolicitudesLicenciasFederativas",
            "[FechaInicio] IS NULL OR (YEAR([FechaInicio]) = [Temporada] AND YEAR([FechaFin]) = [Temporada])");
        migrationBuilder.DropColumn("MediaTemporada", "TarifasLicenciasFederativas");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Guard before any schema/data modification. A day rate has no boolean equivalent.
        ExecuteDeferred(migrationBuilder, """
            IF EXISTS (SELECT 1 FROM TarifasLicenciasFederativas WHERE TipoLicenciaId NOT IN (1, 2))
                OR EXISTS (SELECT 1 FROM SolicitudesLicenciasFederativas WHERE FechaInicio IS NOT NULL OR FechaFin IS NOT NULL)
                OR EXISTS (SELECT 1 FROM SolicitudesLicenciasFederativas GROUP BY UserId, Temporada HAVING COUNT(*) > 1)
                THROW 51001, N'No se puede revertir: existen tipos, coberturas o solicitudes incompatibles con el modelo anterior. Es necesaria una migración de conservación.', 1;
            """);
        migrationBuilder.AddColumn<bool>("MediaTemporada", "TarifasLicenciasFederativas", type: "bit", nullable: false, defaultValue: false);
        ExecuteDeferred(migrationBuilder, "UPDATE TarifasLicenciasFederativas SET MediaTemporada = CASE WHEN TipoLicenciaId = 2 THEN 1 ELSE 0 END;");
        migrationBuilder.DropForeignKey("FK_TarifasLicenciasFederativas_TiposLicencias_TipoLicenciaId", "TarifasLicenciasFederativas");
        migrationBuilder.DropIndex("IX_TarifasLicenciasFederativas_Temporada_TipoLicenciaId_Licencia_Categoria", "TarifasLicenciasFederativas");
        migrationBuilder.DropIndex("IX_TarifasLicenciasFederativas_TipoLicenciaId", "TarifasLicenciasFederativas");
        migrationBuilder.DropIndex("IX_SolicitudesLicenciasFederativas_UserId_Temporada", "SolicitudesLicenciasFederativas");
        migrationBuilder.CreateIndex("IX_TarifasLicenciasFederativas_Temporada_Licencia_Categoria_MediaTemporada",
            "TarifasLicenciasFederativas", new[] { "Temporada", "Licencia", "Categoria", "MediaTemporada" }, unique: true);
        migrationBuilder.CreateIndex("IX_SolicitudesLicenciasFederativas_UserId_Temporada", "SolicitudesLicenciasFederativas",
            new[] { "UserId", "Temporada" }, unique: true);
        migrationBuilder.DropCheckConstraint("CK_SolicitudesLicenciasFederativas_Cobertura", "SolicitudesLicenciasFederativas");
        migrationBuilder.DropCheckConstraint("CK_SolicitudesLicenciasFederativas_CoberturaTemporada", "SolicitudesLicenciasFederativas");
        migrationBuilder.DropColumn("FechaInicio", "SolicitudesLicenciasFederativas");
        migrationBuilder.DropColumn("FechaFin", "SolicitudesLicenciasFederativas");
        migrationBuilder.DropColumn("TipoLicenciaId", "TarifasLicenciasFederativas");
        migrationBuilder.DropTable("TiposLicencias");
    }

    // Idempotent scripts share a batch with ADD/DROP COLUMN. Defer binding of the
    // changed columns until the guarded operation actually runs, including reruns.
    private static void ExecuteDeferred(MigrationBuilder migrationBuilder, string sql)
        => migrationBuilder.Sql("EXEC(N'" + sql.Replace("'", "''") + "');");
}
