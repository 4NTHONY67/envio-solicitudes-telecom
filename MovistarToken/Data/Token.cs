using System;
using System.Collections.Generic;

namespace MovistarToken.Data
{
    public partial class Token
    {
        public string NombreContexto { get; set; }
        public string Campos { get; set; }
        public string NroToken { get; set; }
        public string Estado { get; set; }
        public DateTime? FechaGeneracion { get; set; }
        public DateTime? FechaValidacion { get; set; }
        public int IdToken { get; set; }
        public short? Intento { get; set; }

        public int Telefono { get; set; }
        public string DetalleEstado { get; set; }
        public string IdTransaccion { get; set; }
        public string TipoDoc { get; set; }
        public string NumeroDoc { get; set; }

        //añadidos ultimo
        public string DNI { get; set; }
        //public TimeS? FechaExpiracion { get; set; }

        public virtual Contexto NombreContextoNavigation { get; set; }
        public virtual ICollection<DetalleToken> DetalleToken { get; set; }
    }
}
