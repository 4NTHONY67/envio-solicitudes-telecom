using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovistarToken.Models
{
    public class TokenField
    {
        public string Llave { get; set; }
        public string Valor { get; set; }
    }
    public class TokenRequest : ITokenRequest
    {
        public string Contexto { get; set; }
        public List<TokenField> Campos { get; set; }
    }

    public class ValidTokenRequest : ITokenRequest
    {
        public string Contexto { get; set; }
        public List<TokenField> Campos { get; set; }
        public string Token { get; set; }
    }

    public interface ITokenRequest
    {
        string Contexto { get; set; }
        List<TokenField> Campos { get; set; }
    }
}
