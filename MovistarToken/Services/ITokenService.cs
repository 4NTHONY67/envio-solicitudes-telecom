using MovistarToken.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovistarToken.Services
{
    public interface ITokenService
    {
        bool GenerateToken(ITokenRequest tokenRequest, out string token, out string errorMessage);
        bool ValidateToken(ITokenRequest tokenRequest, out string errorMessage);
    }
}
