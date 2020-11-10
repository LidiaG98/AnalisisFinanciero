﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sistema_de_Informes_de_Analisis_Financieros.Models;

namespace Sistema_de_Informes_de_Analisis_Financieros.Migrations
{
    [DbContext(typeof(ProyAnfContext))]
    [Migration("20201109032247_RazonesFormulas")]
    partial class RazonesFormulas
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(128)")
                        .HasMaxLength(128);

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(128)")
                        .HasMaxLength(128);

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(128)")
                        .HasMaxLength(128);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(128)")
                        .HasMaxLength(128);

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("Sistema_de_Informes_de_Analisis_Financieros.Models.Catalogodecuenta", b =>
                {
                    b.Property<int>("Idempresa")
                        .HasColumnName("IDEMPRESA")
                        .HasColumnType("int");

                    b.Property<int>("Idcuenta")
                        .HasColumnName("IDCUENTA")
                        .HasColumnType("int");

                    b.Property<string>("Codcuentacatalogo")
                        .IsRequired()
                        .HasColumnName("CODCUENTACATALOGO")
                        .HasColumnType("varchar(150)")
                        .HasMaxLength(150)
                        .IsUnicode(false);

                    b.HasKey("Idempresa", "Idcuenta")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("Idcuenta")
                        .HasName("RELATIONSHIP_4_FK");

                    b.HasIndex("Idempresa")
                        .HasName("RELATIONSHIP_5_FK");

                    b.ToTable("CATALOGODECUENTA");
                });

            modelBuilder.Entity("Sistema_de_Informes_de_Analisis_Financieros.Models.Cuenta", b =>
                {
                    b.Property<int>("Idcuenta")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("IDCUENTA")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Idtipocuenta")
                        .HasColumnName("IDTIPOCUENTA")
                        .HasColumnType("int");

                    b.Property<string>("Nomcuenta")
                        .IsRequired()
                        .HasColumnName("NOMCUENTA")
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100)
                        .IsUnicode(false);

                    b.HasKey("Idcuenta")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("Idtipocuenta")
                        .HasName("RELATIONSHIP_2_FK");

                    b.ToTable("CUENTA");
                });

            modelBuilder.Entity("Sistema_de_Informes_de_Analisis_Financieros.Models.Empresa", b =>
                {
                    b.Property<int>("Idempresa")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("IDEMPRESA")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Descempresa")
                        .HasColumnName("DESCEMPRESA")
                        .HasColumnType("varchar(350)")
                        .HasMaxLength(350)
                        .IsUnicode(false);

                    b.Property<int>("Idsector")
                        .HasColumnName("IDSECTOR")
                        .HasColumnType("int");

                    b.Property<string>("Nomempresa")
                        .IsRequired()
                        .HasColumnName("NOMEMPRESA")
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100)
                        .IsUnicode(false);

                    b.Property<string>("Razonsocial")
                        .HasColumnName("RAZONSOCIAL")
                        .HasColumnType("varchar(200)")
                        .HasMaxLength(200)
                        .IsUnicode(false);

                    b.HasKey("Idempresa")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("Idsector")
                        .HasName("RELATIONSHIP_1_FK");

                    b.ToTable("EMPRESA");
                });

            modelBuilder.Entity("Sistema_de_Informes_de_Analisis_Financieros.Models.Ratio", b =>
                {
                    b.Property<int>("Idratio")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("IDRATIO")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Nombreratiob")
                        .IsRequired()
                        .HasColumnName("NOMBRERATIOB")
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100)
                        .IsUnicode(false);

                    b.HasKey("Idratio")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.ToTable("RATIO");
                });

            modelBuilder.Entity("Sistema_de_Informes_de_Analisis_Financieros.Models.Ratiobasesector", b =>
                {
                    b.Property<int>("Idratio")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("IDRATIO")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Idsector")
                        .HasColumnName("IDSECTOR")
                        .HasColumnType("int");

                    b.Property<double>("Valorratiob")
                        .HasColumnName("VALORRATIOB")
                        .HasColumnType("float");

                    b.HasKey("Idratio", "Idsector")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("Idratio")
                        .HasName("RELATIONSHIP_8_FK");

                    b.HasIndex("Idsector")
                        .HasName("RELATIONSHIP_7_FK");

                    b.ToTable("RATIOBASESECTOR");
                });

            modelBuilder.Entity("Sistema_de_Informes_de_Analisis_Financieros.Models.Ratioempresa", b =>
                {
                    b.Property<int>("Idratioempresa")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("IDRATIOEMPRESA")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Idempresa")
                        .HasColumnName("IDEMPRESA")
                        .HasColumnType("int");

                    b.Property<int>("Idratio")
                        .HasColumnName("IDRATIO")
                        .HasColumnType("int");

                    b.Property<double>("Valorratioempresa")
                        .HasColumnName("VALORRATIOEMPRESA")
                        .HasColumnType("float");

                    b.HasKey("Idratioempresa")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("Idempresa")
                        .HasName("RELATIONSHIP_10_FK");

                    b.HasIndex("Idratio")
                        .HasName("RELATIONSHIP_11_FK");

                    b.ToTable("RATIOEMPRESA");
                });

            modelBuilder.Entity("Sistema_de_Informes_de_Analisis_Financieros.Models.Razon", b =>
                {
                    b.Property<int>("idRazon")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("denominador")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("nombreRazon")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("numerador")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("idRazon");

                    b.ToTable("Razon");
                });

            modelBuilder.Entity("Sistema_de_Informes_de_Analisis_Financieros.Models.Sector", b =>
                {
                    b.Property<int>("Idsector")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("IDSECTOR")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Nomsector")
                        .IsRequired()
                        .HasColumnName("NOMSECTOR")
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100)
                        .IsUnicode(false);

                    b.HasKey("Idsector")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.ToTable("SECTOR");
                });

            modelBuilder.Entity("Sistema_de_Informes_de_Analisis_Financieros.Models.Tipocuenta", b =>
                {
                    b.Property<int>("Idtipocuenta")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("IDTIPOCUENTA")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Nomtipocuenta")
                        .IsRequired()
                        .HasColumnName("NOMTIPOCUENTA")
                        .HasColumnType("varchar(150)")
                        .HasMaxLength(150)
                        .IsUnicode(false);

                    b.HasKey("Idtipocuenta")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.ToTable("TIPOCUENTA");
                });

            modelBuilder.Entity("Sistema_de_Informes_de_Analisis_Financieros.Models.Usuario", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<int?>("Idempresa1")
                        .HasColumnType("int");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("Idempresa1");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Sistema_de_Informes_de_Analisis_Financieros.Models.Valoresdebalance", b =>
                {
                    b.Property<int>("Idbalance")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("IDBALANCE")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Anio")
                        .HasColumnName("ANIO")
                        .HasColumnType("int");

                    b.Property<int>("Idcuenta")
                        .HasColumnName("IDCUENTA")
                        .HasColumnType("int");

                    b.Property<int>("Idempresa")
                        .HasColumnName("IDEMPRESA")
                        .HasColumnType("int");

                    b.Property<double>("Valorcuenta")
                        .HasColumnName("VALORCUENTA")
                        .HasColumnType("float");

                    b.HasKey("Idbalance")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("Idempresa", "Idcuenta")
                        .HasName("RELATIONSHIP_9_FK");

                    b.ToTable("VALORESDEBALANCE");
                });

            modelBuilder.Entity("Sistema_de_Informes_de_Analisis_Financieros.Models.Valoresestado", b =>
                {
                    b.Property<int>("Idvalore")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("IDVALORE")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Anio")
                        .HasColumnName("ANIO")
                        .HasColumnType("int");

                    b.Property<int>("Idcuenta")
                        .HasColumnName("IDCUENTA")
                        .HasColumnType("int");

                    b.Property<int>("Idempresa")
                        .HasColumnName("IDEMPRESA")
                        .HasColumnType("int");

                    b.Property<string>("Nombrevalore")
                        .IsRequired()
                        .HasColumnName("NOMBREVALORE")
                        .HasColumnType("varchar(150)")
                        .HasMaxLength(150)
                        .IsUnicode(false);

                    b.Property<double>("Valorestado")
                        .HasColumnName("VALORESTADO")
                        .HasColumnType("float");

                    b.HasKey("Idvalore")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("Idempresa", "Idcuenta")
                        .HasName("RELATIONSHIP_13_FK");

                    b.ToTable("VALORESESTADO");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Sistema_de_Informes_de_Analisis_Financieros.Models.Usuario", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Sistema_de_Informes_de_Analisis_Financieros.Models.Usuario", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Sistema_de_Informes_de_Analisis_Financieros.Models.Usuario", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Sistema_de_Informes_de_Analisis_Financieros.Models.Usuario", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Sistema_de_Informes_de_Analisis_Financieros.Models.Catalogodecuenta", b =>
                {
                    b.HasOne("Sistema_de_Informes_de_Analisis_Financieros.Models.Cuenta", "IdcuentaNavigation")
                        .WithMany("Catalogodecuenta")
                        .HasForeignKey("Idcuenta")
                        .HasConstraintName("FK_CATALOGO_RELATIONS_CUENTA")
                        .IsRequired();

                    b.HasOne("Sistema_de_Informes_de_Analisis_Financieros.Models.Empresa", "IdempresaNavigation")
                        .WithMany("Catalogodecuenta")
                        .HasForeignKey("Idempresa")
                        .HasConstraintName("FK_CATALOGO_RELATIONS_EMPRESA")
                        .IsRequired();
                });

            modelBuilder.Entity("Sistema_de_Informes_de_Analisis_Financieros.Models.Cuenta", b =>
                {
                    b.HasOne("Sistema_de_Informes_de_Analisis_Financieros.Models.Tipocuenta", "IdtipocuentaNavigation")
                        .WithMany("Cuenta")
                        .HasForeignKey("Idtipocuenta")
                        .HasConstraintName("FK_CUENTA_RELATIONS_TIPOCUEN")
                        .IsRequired();
                });

            modelBuilder.Entity("Sistema_de_Informes_de_Analisis_Financieros.Models.Empresa", b =>
                {
                    b.HasOne("Sistema_de_Informes_de_Analisis_Financieros.Models.Sector", "IdsectorNavigation")
                        .WithMany("Empresa")
                        .HasForeignKey("Idsector")
                        .HasConstraintName("FK_EMPRESA_RELATIONS_SECTOR")
                        .IsRequired();
                });

            modelBuilder.Entity("Sistema_de_Informes_de_Analisis_Financieros.Models.Ratiobasesector", b =>
                {
                    b.HasOne("Sistema_de_Informes_de_Analisis_Financieros.Models.Ratio", "IdratioNavigation")
                        .WithMany("Ratiobasesector")
                        .HasForeignKey("Idratio")
                        .HasConstraintName("FK_RATIOBAS_RELATIONS_RATIO")
                        .IsRequired();

                    b.HasOne("Sistema_de_Informes_de_Analisis_Financieros.Models.Sector", "IdsectorNavigation")
                        .WithMany("Ratiobasesector")
                        .HasForeignKey("Idsector")
                        .HasConstraintName("FK_RATIOBAS_RELATIONS_SECTOR")
                        .IsRequired();
                });

            modelBuilder.Entity("Sistema_de_Informes_de_Analisis_Financieros.Models.Ratioempresa", b =>
                {
                    b.HasOne("Sistema_de_Informes_de_Analisis_Financieros.Models.Empresa", "IdempresaNavigation")
                        .WithMany("Ratioempresa")
                        .HasForeignKey("Idempresa")
                        .HasConstraintName("FK_RATIOEMP_RELATIONS_EMPRESA")
                        .IsRequired();

                    b.HasOne("Sistema_de_Informes_de_Analisis_Financieros.Models.Ratio", "IdratioNavigation")
                        .WithMany("Ratioempresa")
                        .HasForeignKey("Idratio")
                        .HasConstraintName("FK_RATIOEMP_RELATIONS_RATIO")
                        .IsRequired();
                });

            modelBuilder.Entity("Sistema_de_Informes_de_Analisis_Financieros.Models.Usuario", b =>
                {
                    b.HasOne("Sistema_de_Informes_de_Analisis_Financieros.Models.Empresa", "Idempresa")
                        .WithMany()
                        .HasForeignKey("Idempresa1");
                });

            modelBuilder.Entity("Sistema_de_Informes_de_Analisis_Financieros.Models.Valoresdebalance", b =>
                {
                    b.HasOne("Sistema_de_Informes_de_Analisis_Financieros.Models.Catalogodecuenta", "Id")
                        .WithMany("Valoresdebalance")
                        .HasForeignKey("Idempresa", "Idcuenta")
                        .HasConstraintName("FK_VALORESD_RELATIONS_CATALOGO")
                        .IsRequired();
                });

            modelBuilder.Entity("Sistema_de_Informes_de_Analisis_Financieros.Models.Valoresestado", b =>
                {
                    b.HasOne("Sistema_de_Informes_de_Analisis_Financieros.Models.Catalogodecuenta", "Id")
                        .WithMany("Valoresestado")
                        .HasForeignKey("Idempresa", "Idcuenta")
                        .HasConstraintName("FK_VALORESE_RELATIONS_CATALOGO")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
