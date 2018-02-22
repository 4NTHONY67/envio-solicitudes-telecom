using Microsoft.EntityFrameworkCore;

namespace MovistarTokenTest.Data
{
    public partial class TokenContext : DbContext
    {
        public virtual DbSet<Contexto> Contexto { get; set; }
        public virtual DbSet<Token> Token { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseNpgsql(@"Host=sl-us-south-1-portal.13.dblayer.com;Port=30972;Database=compose;Username=admin;Password=FZGFXCPUVSTYZUXM");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Contexto>(entity =>
            {
                entity.HasKey(e => e.Nombre);

                entity.Property(e => e.Nombre)
                    .HasColumnType("name")
                    .ValueGeneratedNever()
                    .ForNpgsqlHasComment("Nombre del contexto.");

                entity.Property(e => e.Estado).ForNpgsqlHasComment("Activo = true Inactivo = false");

                entity.Property(e => e.NroCampos).ForNpgsqlHasComment("Numero de campos del contexto para poder validarlo.");

                entity.Property(e => e.NroDigitos).ForNpgsqlHasComment("Cantidad de digitos del Token. Minimo 4 pero si se pone un valor el token se creara con 4 digitos. Maximo 6");

                entity.Property(e => e.Vigencia).ForNpgsqlHasComment("Tiempo de duracion del token en horas.");
            });

            modelBuilder.Entity<Token>(entity =>
            {
                entity.HasKey(e => e.IdToken);

                entity.HasIndex(e => e.NombreContexto)
                    .HasName("fki_Token_Contexto_fkey");

                entity.Property(e => e.Estado)
                    .HasColumnType("name")
                    .ForNpgsqlHasComment("Generado Validado Expirado");

                entity.Property(e => e.FechaGeneracion).HasColumnType("timestamptz");

                entity.Property(e => e.FechaValidacion).HasColumnType("timestamptz");

                entity.Property(e => e.NombreContexto).HasColumnType("name");

                entity.Property(e => e.NroToken).HasColumnType("name");

                entity.HasOne(d => d.NombreContextoNavigation)
                    .WithMany(p => p.Token)
                    .HasForeignKey(d => d.NombreContexto)
                    .HasConstraintName("Token_NombreContexto_fkey");
            });
        }
    }
}
