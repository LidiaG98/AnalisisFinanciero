using Microsoft.EntityFrameworkCore.Migrations;

namespace Sistema_de_Informes_de_Analisis_Financieros.Migrations
{
    public partial class MensajesRatios : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {            
            migrationBuilder.CreateTable(
                name: "MensajesAnalisis",
                columns: table => new
                {
                    idMensaje = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    mensajeMayorBase = table.Column<string>(nullable: true),
                    mensajeMenorBase = table.Column<string>(nullable: true),
                    mensajeMayorEmp = table.Column<string>(nullable: true),
                    mensajeMenorEmp = table.Column<string>(nullable: true),
                    idRatio = table.Column<int>(nullable: false),
                    RatioIdratio = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MensajesAnalisis", x => x.idMensaje);
                    table.ForeignKey(
                        name: "FK_MensajesAnalisis_RATIO_RatioIdratio",
                        column: x => x.RatioIdratio,
                        principalTable: "RATIO",
                        principalColumn: "IDRATIO",
                        onDelete: ReferentialAction.Restrict);
                });
            

            migrationBuilder.CreateIndex(
                name: "IX_MensajesAnalisis_RatioIdratio",
                table: "MensajesAnalisis",
                column: "RatioIdratio");
            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {            
            migrationBuilder.DropTable(
                name: "MensajesAnalisis");           
        }
    }
}
