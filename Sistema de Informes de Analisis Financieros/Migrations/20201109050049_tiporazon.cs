using Microsoft.EntityFrameworkCore.Migrations;

namespace Sistema_de_Informes_de_Analisis_Financieros.Migrations
{
    public partial class tiporazon : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "tipo",
                table: "Razon",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "tipo",
                table: "Razon");
        }
    }
}
