using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IndaloAventurApi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFichaSocio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FichasSocio",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Apellidos = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Dni = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    FechaNacimiento = table.Column<DateOnly>(type: "date", nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    CodigoPostal = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    Poblacion = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Provincia = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Tlf = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: false),
                    Alergias = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    AceptaPoliticaPrivacidad = table.Column<bool>(type: "bit", nullable: false),
                    AceptaUsoImagenes = table.Column<bool>(type: "bit", nullable: false),
                    AceptaCobroCuenta = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FichasSocio", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FichasSocio_UserId",
                table: "FichasSocio",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FichasSocio");
        }
    }
}
