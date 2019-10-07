using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovistarToken.Models
{
    public class TokenResponse
    {
        public string Token { get; set; }
    }

    public class ValidTokenResponse
    {
        public string ResultCode { get; set; }
    }

    public class ErrorResponse
    {
        public string ResultCode { get; set; }
        public string ResultDesc { get; set; }
    }
}
