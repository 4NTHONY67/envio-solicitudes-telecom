using System.Collections.Generic;

namespace MovistarTokenTest
{
    public class TokenModel
    {
        public string Contexto { get; set; }
        public List<CampoModel> Campos { get; set; }
    }

    public class CampoModel
    {
        public string Llave { get; set; }
        public string Valor { get; set; }
    }

    public class TokenModelValidar
    {
        public string Contexto { get; set; }
        public List<CampoModel> Campos { get; set; }
        public string Token { get; set; }
    }

    public class TokenValidateResponse
    {
        public string ResultCode { get; set; }
        //public string ResultDesc { get; set; }
    }

    public class TokenResponse
    {
        public string Token { get; set; }
        //public string ErrorDesc { get; set; }
    }

    public class ErrorResponse
    {
        public string ResultCode { get; set; }
        public string ResultDesc { get; set; }
    }
}
