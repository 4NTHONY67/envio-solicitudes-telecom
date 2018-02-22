using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using MovistarTokenTest.Logic;

namespace MovistarTokenTest.Controllers
{
    //[Produces("application/json")]
    [Route("token")]
    public class TokenController : Controller
    {
        private readonly TokenLogic _tokenLogic;

        public TokenController(TokenLogic tokenLogic) => _tokenLogic = tokenLogic;

        [HttpPost]
        public dynamic Post([FromBody] TokenModel model)
        {
            if (!_tokenLogic.ValidarRequestToken(model, out string errorMessage))
                return new ErrorResponse { ResultCode = "1", ResultDesc = errorMessage };
            var nroToken = _tokenLogic.SaveToken(model);
            return new TokenResponse { Token = nroToken };
        }

        [HttpPost("validar")]
        public dynamic ValidarToken([FromBody] TokenModelValidar model)
        {

            if (_tokenLogic.ValidarTokenModel(model, out string errorMessage))
            {
                return new TokenValidateResponse
                {
                    ResultCode = "0"
                };
            }
            else
            {
                return new ErrorResponse
                {
                    ResultCode = "1",
                    ResultDesc = errorMessage
                };
            }
        }
    }
}