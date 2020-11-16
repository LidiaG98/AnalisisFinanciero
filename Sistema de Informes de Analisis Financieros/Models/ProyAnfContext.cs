using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Sistema_de_Informes_de_Analisis_Financieros.Models;

namespace Sistema_de_Informes_de_Analisis_Financieros.Models
{
    public partial class ProyAnfContext : IdentityDbContext<Usuario>
    {
        public ProyAnfContext()
        {
        }

        public ProyAnfContext(DbContextOptions<ProyAnfContext> options)
            : base(options)
        {
        }
        
        public virtual DbSet<Catalogodecuenta> Catalogodecuenta { get; set; }
        public virtual DbSet<Cuenta> Cuenta { get; set; }
        public virtual DbSet<Empresa> Empresa { get; set; }
        public virtual DbSet<Ratio> Ratio { get; set; }
        public virtual DbSet<Ratiobasesector> Ratiobasesector { get; set; }
        public virtual DbSet<Ratioempresa> Ratioempresa { get; set; }
        public virtual DbSet<Sector> Sector { get; set; }
        public virtual DbSet<Tipocuenta> Tipocuenta { get; set; }
        public virtual DbSet<Valoresdebalance> Valoresdebalance { get; set; }
        public virtual DbSet<Valoresestado> Valoresestado { get; set; }
        public virtual DbSet<Razon> Razon { get; set; }
        public virtual DbSet<MensajesAnalisis> MensajesAnalisis { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
               .SetBasePath(System.IO.Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json")
               .Build();
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           
            modelBuilder.Entity<Catalogodecuenta>(entity =>
            {
                entity.HasKey(e => new { e.Idempresa, e.Idcuenta })
                    .IsClustered(false);

                entity.ToTable("CATALOGODECUENTA");

                entity.HasIndex(e => e.Idcuenta)
                    .HasName("RELATIONSHIP_4_FK");

                entity.HasIndex(e => e.Idempresa)
                    .HasName("RELATIONSHIP_5_FK");

                entity.Property(e => e.Idempresa).HasColumnName("IDEMPRESA");

                entity.Property(e => e.Idcuenta).HasColumnName("IDCUENTA");

                entity.Property(e => e.Codcuentacatalogo)
                    .IsRequired()
                    .HasColumnName("CODCUENTACATALOGO")
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdcuentaNavigation)
                    .WithMany(p => p.Catalogodecuenta)
                    .HasForeignKey(d => d.Idcuenta)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CATALOGO_RELATIONS_CUENTA");

                entity.HasOne(d => d.IdempresaNavigation)
                    .WithMany(p => p.Catalogodecuenta)
                    .HasForeignKey(d => d.Idempresa)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CATALOGO_RELATIONS_EMPRESA");
            });

            modelBuilder.Entity<Cuenta>(entity =>
            {
                entity.HasKey(e => e.Idcuenta)
                    .IsClustered(false);

                entity.ToTable("CUENTA");

                entity.HasIndex(e => e.Idtipocuenta)
                    .HasName("RELATIONSHIP_2_FK");

                entity.Property(e => e.Idcuenta)
                    .HasColumnName("IDCUENTA")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Idtipocuenta).HasColumnName("IDTIPOCUENTA");

                entity.Property(e => e.Nomcuenta)
                    .IsRequired()
                    .HasColumnName("NOMCUENTA")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdtipocuentaNavigation)
                    .WithMany(p => p.Cuenta)
                    .HasForeignKey(d => d.Idtipocuenta)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CUENTA_RELATIONS_TIPOCUEN");
            });

            modelBuilder.Entity<Empresa>(entity =>
            {
                entity.HasKey(e => e.Idempresa)
                    .IsClustered(false);

                entity.ToTable("EMPRESA");

                entity.HasIndex(e => e.Idsector)
                    .HasName("RELATIONSHIP_1_FK");

                entity.Property(e => e.Idempresa)
                    .HasColumnName("IDEMPRESA")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Descempresa)
                    .HasColumnName("DESCEMPRESA")
                    .HasMaxLength(350)
                    .IsUnicode(false);                

                entity.Property(e => e.Idsector).HasColumnName("IDSECTOR");

                entity.Property(e => e.Nomempresa)
                    .IsRequired()
                    .HasColumnName("NOMEMPRESA")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Razonsocial)
                    .HasColumnName("RAZONSOCIAL")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdsectorNavigation)
                    .WithMany(p => p.Empresa)
                    .HasForeignKey(d => d.Idsector)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EMPRESA_RELATIONS_SECTOR");
            });

            modelBuilder.Entity<Ratio>(entity =>
            {
                entity.HasKey(e => e.Idratio)
                    .IsClustered(false);

                entity.ToTable("RATIO");

                entity.Property(e => e.Idratio)
                    .HasColumnName("IDRATIO")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Nombreratiob)
                    .IsRequired()
                    .HasColumnName("NOMBRERATIOB")
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Ratiobasesector>(entity =>
            {
                entity.HasKey(e => new { e.Idratio, e.Idsector })
                    .IsClustered(false);

                entity.ToTable("RATIOBASESECTOR");

                entity.HasIndex(e => e.Idratio)
                    .HasName("RELATIONSHIP_8_FK");

                entity.HasIndex(e => e.Idsector)
                    .HasName("RELATIONSHIP_7_FK");

                entity.Property(e => e.Idratio).HasColumnName("IDRATIO");

                entity.Property(e => e.Idsector).HasColumnName("IDSECTOR");

                entity.Property(e => e.Valorratiob).HasColumnName("VALORRATIOB");

                entity.HasOne(d => d.IdratioNavigation)
                    .WithMany(p => p.Ratiobasesector)
                    .HasForeignKey(d => d.Idratio)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RATIOBAS_RELATIONS_RATIO");

                entity.HasOne(d => d.IdsectorNavigation)
                    .WithMany(p => p.Ratiobasesector)
                    .HasForeignKey(d => d.Idsector)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RATIOBAS_RELATIONS_SECTOR");
            });

            modelBuilder.Entity<Ratioempresa>(entity =>
            {
                entity.HasKey(e => e.Idratioempresa)
                    .IsClustered(false);

                entity.ToTable("RATIOEMPRESA");

                entity.HasIndex(e => e.Idempresa)
                    .HasName("RELATIONSHIP_10_FK");

                entity.HasIndex(e => e.Idratio)
                    .HasName("RELATIONSHIP_11_FK");

                entity.Property(e => e.Idratioempresa)
                    .HasColumnName("IDRATIOEMPRESA")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Idempresa).HasColumnName("IDEMPRESA");

                entity.Property(e => e.Idratio).HasColumnName("IDRATIO");

                entity.Property(e => e.Valorratioempresa).HasColumnName("VALORRATIOEMPRESA");

                entity.HasOne(d => d.IdempresaNavigation)
                    .WithMany(p => p.Ratioempresa)
                    .HasForeignKey(d => d.Idempresa)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RATIOEMP_RELATIONS_EMPRESA");

                entity.HasOne(d => d.IdratioNavigation)
                    .WithMany(p => p.Ratioempresa)
                    .HasForeignKey(d => d.Idratio)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RATIOEMP_RELATIONS_RATIO");
            });

            modelBuilder.Entity<Sector>(entity =>
            {
                entity.HasKey(e => e.Idsector)
                    .IsClustered(false);

                entity.ToTable("SECTOR");

                entity.Property(e => e.Idsector)
                    .HasColumnName("IDSECTOR")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Nomsector)
                    .IsRequired()
                    .HasColumnName("NOMSECTOR")
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Tipocuenta>(entity =>
            {
                entity.HasKey(e => e.Idtipocuenta)
                    .IsClustered(false);

                entity.ToTable("TIPOCUENTA");

                entity.Property(e => e.Idtipocuenta)
                    .HasColumnName("IDTIPOCUENTA")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Nomtipocuenta)
                    .IsRequired()
                    .HasColumnName("NOMTIPOCUENTA")
                    .HasMaxLength(150)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Valoresdebalance>(entity =>
            {
                entity.HasKey(e => e.Idbalance)
                    .IsClustered(false);

                entity.ToTable("VALORESDEBALANCE");

                entity.HasIndex(e => new { e.Idempresa, e.Idcuenta })
                    .HasName("RELATIONSHIP_9_FK");

                entity.Property(e => e.Idbalance)
                    .HasColumnName("IDBALANCE")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Anio).HasColumnName("ANIO");

                entity.Property(e => e.Idcuenta).HasColumnName("IDCUENTA");

                entity.Property(e => e.Idempresa).HasColumnName("IDEMPRESA");

                entity.Property(e => e.Valorcuenta).HasColumnName("VALORCUENTA");

                entity.HasOne(d => d.Id)
                    .WithMany(p => p.Valoresdebalance)
                    .HasForeignKey(d => new { d.Idempresa, d.Idcuenta })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VALORESD_RELATIONS_CATALOGO");
            });

            modelBuilder.Entity<Valoresestado>(entity =>
            {
                entity.HasKey(e => e.Idvalore)
                    .IsClustered(false);

                entity.ToTable("VALORESESTADO");

                entity.HasIndex(e => new { e.Idempresa, e.Idcuenta })
                    .HasName("RELATIONSHIP_13_FK");

                entity.Property(e => e.Idvalore)
                    .HasColumnName("IDVALORE")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Anio).HasColumnName("ANIO");

                entity.Property(e => e.Idcuenta).HasColumnName("IDCUENTA");

                entity.Property(e => e.Idempresa).HasColumnName("IDEMPRESA");

                entity.Property(e => e.Nombrevalore)
                    .IsRequired()
                    .HasColumnName("NOMBREVALORE")
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Valorestado).HasColumnName("VALORESTADO");

                entity.HasOne(d => d.Id)
                    .WithMany(p => p.Valoresestado)
                    .HasForeignKey(d => new { d.Idempresa, d.Idcuenta })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VALORESE_RELATIONS_CATALOGO");
            });

            OnModelCreatingPartial(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        public DbSet<Sistema_de_Informes_de_Analisis_Financieros.Models.NomCuentaE> NomCuentaE { get; set; }
    }

    public class Usuario : IdentityUser
    {
        public virtual Empresa Idempresa { get; set; }
    }
}
