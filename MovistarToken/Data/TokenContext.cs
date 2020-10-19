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
        public virtual DbSet<TokenAuth> TokenAuth { get; set; }
        public virtual DbSet<TokenHistorico> TokenHistorico { get; set; }
        public virtual DbSet<DetalleTokenHistorico> DetalleTokenHistorico { get; set; }
        public virtual DbSet<TokenDepuracion> TokenDepuracion { get; set; }
        public virtual DbSet<TokenServicio> TokenServicio { get; set; }

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

                entity.Property(e => e.Estado).ForNpgsqlHasComment(@"GeneradoValidadoExpirado");

                entity.Property(e => e.FechaGeneracion).HasColumnType("timestamp with time zone");
                entity.Property(e => e.FechaValidacion).HasColumnType("timestamp with time zone");
                entity.Property(e => e.FechaExpiracion).HasColumnType("timestamp with time zone");

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
                //entity.Property(e => e.EstadoEvent).ForNpgsqlHasComment(@"Estado Event");
                entity.Property(e => e.EstadoEvent).ForNpgsqlHasComment(@"Activo = true Inactivo = false");
                entity.HasOne(d => d.IdTokenNavigation)
                    .WithMany(p => p.DetalleToken)
                    .HasForeignKey(d => d.IdToken).HasConstraintName("DetalleToken_IdToken_fkey");
            });

            modelBuilder.Entity<TokenAuth>(entity =>
            {
                entity.HasKey(e => e.IdTokenAuth).HasName("TokenAuth_pkey");

                entity.Property(e => e.AccessToken).ForNpgsqlHasComment(@"Access Token");

                entity.Property(e => e.RefreshToken).ForNpgsqlHasComment(@"Refresh Token");

            });

            modelBuilder.Entity<TokenDepuracion>(entity =>
            {
                entity.HasKey(e => e.IdTokenDepuracion).HasName("TokenDepuracion_pkey");

                entity.Property(e => e.NombreContexto).ForNpgsqlHasComment(@"Nombre Contexto");

                entity.Property(e => e.Estado).ForNpgsqlHasComment(@"Estado");
                entity.Property(e => e.PeriodoEjecucion).ForNpgsqlHasComment("Periodo Ejecucion");
                entity.Property(e => e.FechaEjecucion).HasColumnType("date");
                entity.Property(e => e.FechaRegistro).HasColumnType("date");

            });

            modelBuilder.Entity<TokenHistorico>(entity =>
            {
                entity.HasKey(e => e.IdTokenHistorico).HasName("TokenHistorico_pkey");

                entity.Property(e => e.NombreContexto).ForNpgsqlHasComment(@"Nombre Contexto");
                entity.Property(e => e.Campos).ForNpgsqlHasComment(@"Campos");
                entity.Property(e => e.NroToken).ForNpgsqlHasComment(@"NroToken");
                entity.Property(e => e.Estado).ForNpgsqlHasComment(@"Estado");
               // entity.Property(e => e.PeriodoEjecucion).ForNpgsqlHasComment("Periodo Ejecucion");
                entity.Property(e => e.FechaGeneracion).HasColumnType("timestamp with time zone");
                entity.Property(e => e.FechaValidacion).HasColumnType("timestamp with time zone");
                entity.Property(e => e.IdToken).ForNpgsqlHasComment("IdToken");
                entity.Property(e => e.Intento).ForNpgsqlHasComment("Intento");
                entity.Property(e => e.FechaExpiracion).HasColumnType("timestamp with time zone");
                entity.Property(e => e.TipoDoc).ForNpgsqlHasComment(@"TipoDoc");
                entity.Property(e => e.NumeroDoc).ForNpgsqlHasComment(@"NumeroDoc");
                entity.Property(e => e.IdTransaccion).ForNpgsqlHasComment(@"IdTransaccion");
                entity.Property(e => e.Telefono).ForNpgsqlHasComment("Telefono");
                entity.Property(e => e.DetalleEstado).ForNpgsqlHasComment(@"Detalle Estado");
                entity.Property(e => e.TokenIngresado).ForNpgsqlHasComment(@"Token Ingresado");
                entity.Property(e => e.FechaEjecucion).HasColumnType("timestamp with time zone");


            });

            modelBuilder.Entity<DetalleTokenHistorico>(entity =>
            {
                entity.HasKey(e => e.IdDetalleTokenHistorico).HasName("DetalleTokenHistorico_pkey");

                entity.HasIndex(e => e.IdTokenHistorico).HasName("fki_DetalleTokenHistorico_TokenHistorico_fkey");
                
                entity.Property(e => e.IdToken).ForNpgsqlHasComment("IdToken");
                entity.Property(e => e.IdDetalleToken).ForNpgsqlHasComment("IdDetalleToken");

                entity.Property(e => e.FechaEnvioNotificacion).HasColumnType("timestamp with time zone");

                entity.Property(e => e.FechaRespuestaNotificacion).HasColumnType("timestamp with time zone");

                entity.Property(e => e.CodigoNotificacion).ForNpgsqlHasComment(@"Codigo notificacion");

                entity.Property(e => e.OrigenNotificacion).ForNpgsqlHasComment(@"Origen notificacion");

                entity.Property(e => e.MensajeNotificacion).ForNpgsqlHasComment(@"Mensaje notificacion");

                entity.Property(e => e.FechaRespuestaServicioToken).HasColumnType("timestamp with time zone");
                entity.Property(e => e.EstadoEvent).ForNpgsqlHasComment(@"Estado Event");
                entity.Property(e => e.FechaDepuracion).HasColumnType("timestamp with time zone");

                entity.HasOne(d => d.IdTokenHistoricoNavigation)
                    .WithMany(p => p.DetalleTokenHistorico)
                    .HasForeignKey(d => d.IdTokenHistorico).HasConstraintName("DetalleTokenHistorico_IdTokenHistorico_fkey");
            });

            modelBuilder.Entity<TokenServicio>(entity =>
            {
                entity.HasKey(e => e.IdTokenServicio).HasName("TokenServicio_pkey");

                entity.Property(e => e.NombreContexto).ForNpgsqlHasComment(@"Nombre Contexto");

                entity.Property(e => e.Estado).ForNpgsqlHasComment(@"Estado");
                entity.Property(e => e.PeriodoEjecucion).ForNpgsqlHasComment("Periodo Ejecucion");
                entity.Property(e => e.FechaEjecucion).HasColumnType("date");
                entity.Property(e => e.FechaRegistro).HasColumnType("date");

            });

        }
    }
}
