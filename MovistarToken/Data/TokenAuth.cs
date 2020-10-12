using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovistarToken.Data
{
    public class TokenAuth
    {
        public int IdTokenAuth { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
