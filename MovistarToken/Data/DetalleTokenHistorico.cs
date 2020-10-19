using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovistarToken.Data
{
    public class DetalleTokenHistorico
    {
        public int IdDetalleTokenHistorico { get; set; }
        public int IdTokenHistorico { get; set; }
        public int IdToken { get; set; }
        public int IdDetalleToken { get; set; }
        public DateTime? FechaEnvioNotificacion { get; set; }
        public DateTime? FechaRespuestaNotificacion { get; set; }
        public string CodigoNotificacion { get; set; }
        public string OrigenNotificacion { get; set; }
        public string MensajeNotificacion { get; set; }
        public DateTime? FechaRespuestaServicioToken { get; set; }
        public string CodigoRespuestaEvent { get; set; }
        public string MensajeRespuestaEvent { get; set; }
        public int NumeroReintentosEvent { get; set; }
        public DateTime? FechaEnvioEvent { get; set; }
        public string OrigenEnvioEvent { get; set; }
        public bool EstadoEvent { get; set; }
        public string TokenValidado { get; set; }
        public DateTime? FechaDepuracion { get; set; }
        public virtual TokenHistorico IdTokenHistoricoNavigation { get; set; }
    }
}
