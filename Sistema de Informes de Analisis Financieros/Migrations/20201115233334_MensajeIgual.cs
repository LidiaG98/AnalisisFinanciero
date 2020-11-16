using Microsoft.EntityFrameworkCore.Migrations;

namespace Sistema_de_Informes_de_Analisis_Financieros.Migrations
{
    public partial class MensajeIgual : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "mensajeIgualBase",
                table: "MensajesAnalisis",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "mensajeIgualEmp",
                table: "MensajesAnalisis",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "mensajeIgualBase",
                table: "MensajesAnalisis");

            migrationBuilder.DropColumn(
                name: "mensajeIgualEmp",
                table: "MensajesAnalisis");
        }
    }
}
