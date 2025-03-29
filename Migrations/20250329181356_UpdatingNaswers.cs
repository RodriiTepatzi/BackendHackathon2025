using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendHackathon2025.Migrations
{
    /// <inheritdoc />
    public partial class UpdatingNaswers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Alimentacion",
                table: "Promedios");

            migrationBuilder.DropColumn(
                name: "Clima",
                table: "Promedios");

            migrationBuilder.DropColumn(
                name: "Interaccion",
                table: "Promedios");

            migrationBuilder.DropColumn(
                name: "Ocio",
                table: "Promedios");

            migrationBuilder.RenameColumn(
                name: "Temperatura",
                table: "Promedios",
                newName: "TemperaturaPromedio");

            migrationBuilder.RenameColumn(
                name: "Luz",
                table: "Promedios",
                newName: "RuidoPromedio");

            migrationBuilder.RenameColumn(
                name: "FrecuenciaDeRuido",
                table: "Promedios",
                newName: "MovimientoPromedio");

            migrationBuilder.RenameColumn(
                name: "CantidadDeMovimiento",
                table: "Promedios",
                newName: "LuzPromedio");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRegistro",
                table: "Promedios",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "PersonalizedQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalizedQuestions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuestionResponses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Response = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionResponses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionResponses_PersonalizedQuestions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "PersonalizedQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuestionResponses_QuestionId",
                table: "QuestionResponses",
                column: "QuestionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuestionResponses");

            migrationBuilder.DropTable(
                name: "PersonalizedQuestions");

            migrationBuilder.DropColumn(
                name: "FechaRegistro",
                table: "Promedios");

            migrationBuilder.RenameColumn(
                name: "TemperaturaPromedio",
                table: "Promedios",
                newName: "Temperatura");

            migrationBuilder.RenameColumn(
                name: "RuidoPromedio",
                table: "Promedios",
                newName: "Luz");

            migrationBuilder.RenameColumn(
                name: "MovimientoPromedio",
                table: "Promedios",
                newName: "FrecuenciaDeRuido");

            migrationBuilder.RenameColumn(
                name: "LuzPromedio",
                table: "Promedios",
                newName: "CantidadDeMovimiento");

            migrationBuilder.AddColumn<string>(
                name: "Alimentacion",
                table: "Promedios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Clima",
                table: "Promedios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Interaccion",
                table: "Promedios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ocio",
                table: "Promedios",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
