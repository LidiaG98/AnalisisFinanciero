using Microsoft.EntityFrameworkCore.Migrations;

namespace Sistema_de_Informes_de_Analisis_Financieros.Migrations
{
    public partial class Inicial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RATIO",
                columns: table => new
                {
                    IDRATIO = table.Column<int>(nullable: false),
                    NOMBRERATIOB = table.Column<string>(unicode: false, maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RATIO", x => x.IDRATIO)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "SECTOR",
                columns: table => new
                {
                    IDSECTOR = table.Column<int>(nullable: false),
                    NOMSECTOR = table.Column<string>(unicode: false, maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SECTOR", x => x.IDSECTOR)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "TIPOCUENTA",
                columns: table => new
                {
                    IDTIPOCUENTA = table.Column<int>(nullable: false),
                    NOMTIPOCUENTA = table.Column<string>(unicode: false, maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TIPOCUENTA", x => x.IDTIPOCUENTA)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "EMPRESA",
                columns: table => new
                {
                    IDEMPRESA = table.Column<int>(nullable: false),
                    IDSECTOR = table.Column<int>(nullable: false),
                    NOMEMPRESA = table.Column<string>(unicode: false, maxLength: 100, nullable: false),
                    DESCEMPRESA = table.Column<string>(unicode: false, maxLength: 350, nullable: true),
                    RAZONSOCIAL = table.Column<string>(unicode: false, maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EMPRESA", x => x.IDEMPRESA)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_EMPRESA_RELATIONS_SECTOR",
                        column: x => x.IDSECTOR,
                        principalTable: "SECTOR",
                        principalColumn: "IDSECTOR",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RATIOBASESECTOR",
                columns: table => new
                {
                    IDRATIO = table.Column<int>(nullable: false),
                    IDSECTOR = table.Column<int>(nullable: false),
                    VALORRATIOB = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RATIOBASESECTOR", x => new { x.IDRATIO, x.IDSECTOR })
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_RATIOBAS_RELATIONS_RATIO",
                        column: x => x.IDRATIO,
                        principalTable: "RATIO",
                        principalColumn: "IDRATIO",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RATIOBAS_RELATIONS_SECTOR",
                        column: x => x.IDSECTOR,
                        principalTable: "SECTOR",
                        principalColumn: "IDSECTOR",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CUENTA",
                columns: table => new
                {
                    IDCUENTA = table.Column<int>(nullable: false),
                    IDTIPOCUENTA = table.Column<int>(nullable: false),
                    NOMCUENTA = table.Column<string>(unicode: false, maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CUENTA", x => x.IDCUENTA)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_CUENTA_RELATIONS_TIPOCUEN",
                        column: x => x.IDTIPOCUENTA,
                        principalTable: "TIPOCUENTA",
                        principalColumn: "IDTIPOCUENTA",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RATIOEMPRESA",
                columns: table => new
                {
                    IDRATIOEMPRESA = table.Column<int>(nullable: false),
                    IDRATIO = table.Column<int>(nullable: false),
                    IDEMPRESA = table.Column<int>(nullable: false),
                    VALORRATIOEMPRESA = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RATIOEMPRESA", x => x.IDRATIOEMPRESA)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_RATIOEMP_RELATIONS_EMPRESA",
                        column: x => x.IDEMPRESA,
                        principalTable: "EMPRESA",
                        principalColumn: "IDEMPRESA",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RATIOEMP_RELATIONS_RATIO",
                        column: x => x.IDRATIO,
                        principalTable: "RATIO",
                        principalColumn: "IDRATIO",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CATALOGODECUENTA",
                columns: table => new
                {
                    IDEMPRESA = table.Column<int>(nullable: false),
                    IDCUENTA = table.Column<int>(nullable: false),
                    CODCUENTACATALOGO = table.Column<string>(unicode: false, maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CATALOGODECUENTA", x => new { x.IDEMPRESA, x.IDCUENTA })
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_CATALOGO_RELATIONS_CUENTA",
                        column: x => x.IDCUENTA,
                        principalTable: "CUENTA",
                        principalColumn: "IDCUENTA",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CATALOGO_RELATIONS_EMPRESA",
                        column: x => x.IDEMPRESA,
                        principalTable: "EMPRESA",
                        principalColumn: "IDEMPRESA",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VALORESDEBALANCE",
                columns: table => new
                {
                    IDBALANCE = table.Column<int>(nullable: false),
                    IDEMPRESA = table.Column<int>(nullable: false),
                    IDCUENTA = table.Column<int>(nullable: false),
                    VALORCUENTA = table.Column<double>(nullable: false),
                    ANIO = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VALORESDEBALANCE", x => x.IDBALANCE)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_VALORESD_RELATIONS_CATALOGO",
                        columns: x => new { x.IDEMPRESA, x.IDCUENTA },
                        principalTable: "CATALOGODECUENTA",
                        principalColumns: new[] { "IDEMPRESA", "IDCUENTA" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VALORESESTADO",
                columns: table => new
                {
                    IDVALORE = table.Column<int>(nullable: false),
                    IDEMPRESA = table.Column<int>(nullable: false),
                    IDCUENTA = table.Column<int>(nullable: false),
                    NOMBREVALORE = table.Column<string>(unicode: false, maxLength: 150, nullable: false),
                    VALORESTADO = table.Column<double>(nullable: false),
                    ANIO = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VALORESESTADO", x => x.IDVALORE)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_VALORESE_RELATIONS_CATALOGO",
                        columns: x => new { x.IDEMPRESA, x.IDCUENTA },
                        principalTable: "CATALOGODECUENTA",
                        principalColumns: new[] { "IDEMPRESA", "IDCUENTA" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "RELATIONSHIP_4_FK",
                table: "CATALOGODECUENTA",
                column: "IDCUENTA");

            migrationBuilder.CreateIndex(
                name: "RELATIONSHIP_5_FK",
                table: "CATALOGODECUENTA",
                column: "IDEMPRESA");

            migrationBuilder.CreateIndex(
                name: "RELATIONSHIP_2_FK",
                table: "CUENTA",
                column: "IDTIPOCUENTA");

            migrationBuilder.CreateIndex(
                name: "RELATIONSHIP_1_FK",
                table: "EMPRESA",
                column: "IDSECTOR");

            migrationBuilder.CreateIndex(
                name: "RELATIONSHIP_8_FK",
                table: "RATIOBASESECTOR",
                column: "IDRATIO");

            migrationBuilder.CreateIndex(
                name: "RELATIONSHIP_7_FK",
                table: "RATIOBASESECTOR",
                column: "IDSECTOR");

            migrationBuilder.CreateIndex(
                name: "RELATIONSHIP_10_FK",
                table: "RATIOEMPRESA",
                column: "IDEMPRESA");

            migrationBuilder.CreateIndex(
                name: "RELATIONSHIP_11_FK",
                table: "RATIOEMPRESA",
                column: "IDRATIO");

            migrationBuilder.CreateIndex(
                name: "RELATIONSHIP_9_FK",
                table: "VALORESDEBALANCE",
                columns: new[] { "IDEMPRESA", "IDCUENTA" });

            migrationBuilder.CreateIndex(
                name: "RELATIONSHIP_13_FK",
                table: "VALORESESTADO",
                columns: new[] { "IDEMPRESA", "IDCUENTA" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RATIOBASESECTOR");

            migrationBuilder.DropTable(
                name: "RATIOEMPRESA");

            migrationBuilder.DropTable(
                name: "VALORESDEBALANCE");

            migrationBuilder.DropTable(
                name: "VALORESESTADO");

            migrationBuilder.DropTable(
                name: "RATIO");

            migrationBuilder.DropTable(
                name: "CATALOGODECUENTA");

            migrationBuilder.DropTable(
                name: "CUENTA");

            migrationBuilder.DropTable(
                name: "EMPRESA");

            migrationBuilder.DropTable(
                name: "TIPOCUENTA");

            migrationBuilder.DropTable(
                name: "SECTOR");
        }
    }
}
