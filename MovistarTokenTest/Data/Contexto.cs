using System.Collections.Generic;

namespace MovistarTokenTest.Data
{
    public partial class Contexto
    {
        public Contexto()
        {
            Token = new HashSet<Token>();
        }

        public string Nombre { get; set; }
        public short? NroCampos { get; set; }
        public bool Estado { get; set; }
        public short? Vigencia { get; set; }
        public short? NroDigitos { get; set; }

        public ICollection<Token> Token { get; set; }
    }
}
