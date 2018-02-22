using MovistarTokenTest.Infrastructure;

namespace MovistarTokenTest.Common
{
    public class EstadoToken : Enumeration<EstadoToken, string>
    {
        private EstadoToken(string value, string displayName) : base(value, displayName) { }

        public static readonly EstadoToken Generado = new EstadoToken("Generado", "Generado");
        public static readonly EstadoToken Validado = new EstadoToken("Validado", "Validado");
        public static readonly EstadoToken Expirado = new EstadoToken("Expirado", "Expirado");
    }

}
