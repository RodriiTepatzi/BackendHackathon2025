using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendHackathon2025.Migrations
{
    /// <inheritdoc />
    public partial class AddingModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Medibles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Temperatura = table.Column<float>(type: "real", nullable: true),
                    CantidadDeMovimiento = table.Column<float>(type: "real", nullable: true),
                    FrecuenciaDeRuido = table.Column<float>(type: "real", nullable: true),
                    Luz = table.Column<float>(type: "real", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medibles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Medibles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "NoMedibles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Clima = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Alimentacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Interaccion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ocio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Productividad = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActividadFisica = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NoMedibles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NoMedibles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Promedios",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Clima = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Alimentacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Interaccion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ocio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Temperatura = table.Column<float>(type: "real", nullable: true),
                    CantidadDeMovimiento = table.Column<float>(type: "real", nullable: true),
                    FrecuenciaDeRuido = table.Column<float>(type: "real", nullable: true),
                    Luz = table.Column<float>(type: "real", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Promedios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Promedios_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Medibles_UserId",
                table: "Medibles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_NoMedibles_UserId",
                table: "NoMedibles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Promedios_UserId",
                table: "Promedios",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Medibles");

            migrationBuilder.DropTable(
                name: "NoMedibles");

            migrationBuilder.DropTable(
                name: "Promedios");
        }
    }
}
