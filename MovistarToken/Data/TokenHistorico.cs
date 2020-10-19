using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovistarToken.Data
{
    public class TokenHistorico
    {
        public int IdTokenHistorico { get; set; }
        public string NombreContexto { get; set; }
        public string Campos { get; set; }
        public string NroToken { get; set; }
        public string Estado { get; set; }
        public DateTime? FechaGeneracion { get; set; }
        public DateTime? FechaValidacion { get; set; }
        public int IdToken { get; set; }
        public short? Intento { get; set; }
        public DateTime? FechaExpiracion { get; set; }
        public string TipoDoc { get; set; }
        public string NumeroDoc { get; set; }
        public string IdTransaccion { get; set; }
        public int Telefono { get; set; }
        public string DetalleEstado { get; set; }
        public string TokenIngresado { get; set; }
        public DateTime? FechaEjecucion { get; set; }
        public virtual ICollection<DetalleTokenHistorico> DetalleTokenHistorico { get; set; }
    }
}
