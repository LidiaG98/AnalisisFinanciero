using Microsoft.EntityFrameworkCore.Migrations;

namespace Sistema_de_Informes_de_Analisis_Financieros.Migrations
{
    public partial class NomCuentaE : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "nomCuentaEID",
                table: "CATALOGODECUENTA",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "NomCuentaE",
                columns: table => new
                {
                    nomCuentaEID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nomCuentaE = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NomCuentaE", x => x.nomCuentaEID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CATALOGODECUENTA_nomCuentaEID",
                table: "CATALOGODECUENTA",
                column: "nomCuentaEID");

            migrationBuilder.AddForeignKey(
                name: "FK_CATALOGODECUENTA_NomCuentaE_nomCuentaEID",
                table: "CATALOGODECUENTA",
                column: "nomCuentaEID",
                principalTable: "NomCuentaE",
                principalColumn: "nomCuentaEID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CATALOGODECUENTA_NomCuentaE_nomCuentaEID",
                table: "CATALOGODECUENTA");

            migrationBuilder.DropTable(
                name: "NomCuentaE");

            migrationBuilder.DropIndex(
                name: "IX_CATALOGODECUENTA_nomCuentaEID",
                table: "CATALOGODECUENTA");

            migrationBuilder.DropColumn(
                name: "nomCuentaEID",
                table: "CATALOGODECUENTA");
        }
    }
}
