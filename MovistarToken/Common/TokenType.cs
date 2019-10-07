using MovistarToken.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovistarToken.Common
{
    public class TokenType : Enumeration<TokenType, string>
    {
        private TokenType(string value, string displayName) : base(value, displayName) { }
        public static readonly TokenType Generado = new TokenType("Generado", "Generado");
        public static readonly TokenType Validado = new TokenType("Validado", "Validado");
        public static readonly TokenType Expirado = new TokenType("Expirado", "Expirado");
        public static readonly TokenType Intento = new TokenType("Intento", "Intento");
        public static readonly TokenType Excedido = new TokenType("Excedido", "Excedido");
    }
}
