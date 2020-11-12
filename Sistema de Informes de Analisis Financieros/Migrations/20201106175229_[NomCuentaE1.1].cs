using Microsoft.EntityFrameworkCore.Migrations;

namespace Sistema_de_Informes_de_Analisis_Financieros.Migrations
{
    public partial class NomCuentaE11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CATALOGODECUENTA_NomCuentaE_nomCuentaEID",
                table: "CATALOGODECUENTA");

            migrationBuilder.AlterColumn<int>(
                name: "nomCuentaEID",
                table: "CATALOGODECUENTA",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_CATALOGODECUENTA_NomCuentaE_nomCuentaEID",
                table: "CATALOGODECUENTA",
                column: "nomCuentaEID",
                principalTable: "NomCuentaE",
                principalColumn: "nomCuentaEID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CATALOGODECUENTA_NomCuentaE_nomCuentaEID",
                table: "CATALOGODECUENTA");

            migrationBuilder.AlterColumn<int>(
                name: "nomCuentaEID",
                table: "CATALOGODECUENTA",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CATALOGODECUENTA_NomCuentaE_nomCuentaEID",
                table: "CATALOGODECUENTA",
                column: "nomCuentaEID",
                principalTable: "NomCuentaE",
                principalColumn: "nomCuentaEID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
