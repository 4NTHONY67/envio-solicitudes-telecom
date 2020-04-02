using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MovistarToken.Data
{
    public partial class TokenContext : DbContext
    {
        public TokenContext()
        {
        }

        public TokenContext(DbContextOptions<TokenContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Contexto> Contexto { get; set; }
        public virtual DbSet<Token> Token { get; set; }
        public virtual DbSet<DetalleToken> DetalleToken { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                //                optionsBuilder.UseNpgsql("Host=sl-us-south-1-portal.13.dblayer.com;Port=30972;Database=compose;Username=admin;Password=FZGFXCPUVSTYZUXM");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity<Contexto>(entity =>
            {
                entity.HasKey(e => e.Nombre)
                    .HasName("Contexto_pkey");

                entity.Property(e => e.Nombre)
                    .ValueGeneratedNever()
                    .ForNpgsqlHasComment("Nombre del contexto.");

                entity.Property(e => e.Estado).ForNpgsqlHasComment(@"Activo = true
Inactivo = false");

                entity.Property(e => e.FechaCrea).HasColumnType("date");

                entity.Property(e => e.NombreCampos).IsRequired();

                entity.Property(e => e.NroCampos).ForNpgsqlHasComment("Numero de campos del contexto para poder validarlo.");

                entity.Property(e => e.NroDigitos).ForNpgsqlHasComment(@"Cantidad de digitos del Token.
Minimo 4 pero si se pone un valor el token se creara con 4 digitos.
Maximo 6");

                entity.Property(e => e.Vigencia).ForNpgsqlHasComment("Tiempo de duracion del token en horas.");
            });

            modelBuilder.Entity<Token>(entity =>
            {
                entity.HasKey(e => e.IdToken)
                    .HasName("Token_pkey");

                entity.HasIndex(e => e.NombreContexto)
                    .HasName("fki_Token_Contexto_fkey");

                entity.Property(e => e.Estado).ForNpgsqlHasComment(@"Generado
Validado
Expirado");

                entity.Property(e => e.FechaGeneracion).HasColumnType("timestamp with time zone");

                entity.Property(e => e.FechaValidacion).HasColumnType("timestamp with time zone");

                entity.Property(e => e.Telefono).ForNpgsqlHasComment("Telefono");
                entity.Property(e => e.DetalleEstado).ForNpgsqlHasComment("DetalleEstado");
                entity.Property(e => e.IdTransaccion).ForNpgsqlHasComment("IdTransaccion");
                entity.Property(e => e.TipoDoc).ForNpgsqlHasComment("TipoDoc");
                entity.Property(e => e.NumeroDoc).ForNpgsqlHasComment("NumeroDoc");

                entity.HasOne(d => d.NombreContextoNavigation)
                    .WithMany(p => p.Token)
                    .HasForeignKey(d => d.NombreContexto)
                    .HasConstraintName("Token_NombreContexto_fkey");
            });


            modelBuilder.Entity<DetalleToken>(entity =>
            {
                entity.HasKey(e => e.IdDetalleToken).HasName("DetalleToken_pkey");

                entity.HasIndex(e => e.IdToken).HasName("fki_DetalleToken_Token_fkey");

                entity.Property(e => e.FechaEnvioNotificacion).HasColumnType("timestamp with time zone");

                entity.Property(e => e.FechaRespuestaNotificacion).HasColumnType("timestamp with time zone");

                entity.Property(e => e.CodigoNotificacion).ForNpgsqlHasComment(@"Codigo notificacion");

                entity.Property(e => e.OrigenNotificacion).ForNpgsqlHasComment(@"Origen notificacion");

                entity.Property(e => e.MensajeNotificacion).ForNpgsqlHasComment(@"Mensaje notificacion");

                entity.Property(e => e.FechaRespuestaServicioToken).HasColumnType("timestamp with time zone");


                entity.HasOne(d => d.IdTokenNavigation)
                    .WithMany(p => p.DetalleToken)
                    .HasForeignKey(d => d.IdToken).HasConstraintName("DetalleToken_IdToken_fkey");
            });



        }
    }
}
