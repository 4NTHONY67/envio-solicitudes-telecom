using System;

namespace MovistarTokenTest.Data
{
    public partial class Token
    {
        public string NombreContexto { get; set; }
        public string Campos { get; set; }
        public string NroToken { get; set; }
        public int IdToken { get; set; }
        public string Estado { get; set; }
        public DateTime? FechaGeneracion { get; set; }
        public DateTime? FechaValidacion { get; set; }

        public Contexto NombreContextoNavigation { get; set; }
    }
}
