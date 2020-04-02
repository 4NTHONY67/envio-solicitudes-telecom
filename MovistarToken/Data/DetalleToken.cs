using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovistarToken.Data
{
    public class DetalleToken
    {
        public int IdDetalleToken { get; set; }
        public int IdToken { get; set; }
        public DateTime? FechaEnvioNotificacion { get; set; }
        public DateTime? FechaRespuestaNotificacion { get; set; }
        public string CodigoNotificacion { get; set; }
        public string OrigenNotificacion { get; set; }
        public string MensajeNotificacion { get; set; }
        public DateTime? FechaRespuestaServicioToken { get; set; }
        public virtual Token IdTokenNavigation { get; set; }

    }
}
