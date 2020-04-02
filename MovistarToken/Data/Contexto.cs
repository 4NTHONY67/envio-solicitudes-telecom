using System;
using System.Collections.Generic;

namespace MovistarToken.Data
{
    public partial class Contexto
    {
        public Contexto()
        {
            Token = new HashSet<Token>();
        }

        public string Nombre { get; set; }
        public short NroCampos { get; set; }
        public bool Estado { get; set; }
        public short Vigencia { get; set; }
        public short NroDigitos { get; set; }
        public bool EsAlfanumerico { get; set; }
        public short Reintentos { get; set; }
        public string NombreCampos { get; set; }
        public DateTime? FechaCrea { get; set; }
        public bool EnvioNotificacion { get; set; }
        public string TipoNotificacion { get; set; }
        public string CodigoPlantillaSmS { get; set; }
        public string CodigoPlantillaEmail { get; set; }
        public virtual ICollection<Token> Token { get; set; }
    }
}
