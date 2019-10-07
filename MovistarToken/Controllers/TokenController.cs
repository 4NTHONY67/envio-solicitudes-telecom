using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovistarToken.Models;
using MovistarToken.Services;

namespace MovistarToken.Controllers
{
    [Route("token")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly ITokenService _tokenService;

        public TokenController(ITokenService tokenService) => _tokenService = tokenService;

        [HttpPost]
        public dynamic Post([FromBody] TokenRequest tokenRequest)
        {
            if (_tokenService.GenerateToken(tokenRequest, out string token, out string errorMessage))
                return new TokenResponse { Token = token };
            else
                return new ErrorResponse { ResultCode = "1", ResultDesc = errorMessage };
        }

        [HttpPost("validar")]
        public dynamic ValidarToken([FromBody] ValidTokenRequest model)
        {
            if (_tokenService.ValidateToken(model, out string errorMessage))
                return new ValidTokenResponse { ResultCode = "0" };
            else
                return new ErrorResponse { ResultCode = "1", ResultDesc = errorMessage };
        }
    }
}