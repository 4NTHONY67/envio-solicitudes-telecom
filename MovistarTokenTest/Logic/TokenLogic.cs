using System;
using System.Collections.Generic;
using System.Linq;
using MovistarTokenTest.Common;
using MovistarTokenTest.Data;
using MovistarTokenTest.Infrastructure;
using Newtonsoft.Json;

namespace MovistarTokenTest.Logic
{
    public class TokenLogic
    {
        private readonly ITokenRepository _tokenRepository;

        public TokenLogic(ITokenRepository tokenRepository) => _tokenRepository = tokenRepository;

        public bool ValidarRequestToken(TokenModel model, out string errorMessage)
        {
            errorMessage = string.Empty;
            var contexto = _tokenRepository.GetContexto(model.Contexto);
            if (contexto == null) errorMessage = "El contexto no existe.";
            else if (!contexto.Estado) errorMessage = "El contexto no se encuentra activo.";
            else if (model.Campos.Count < contexto.NroCampos) errorMessage = "La cantidad de campos no es la requerida por el contexto.";
            else if (model.Campos.Any(campo => string.IsNullOrWhiteSpace(campo.Llave) || string.IsNullOrWhiteSpace(campo.Valor))) errorMessage = "Un valor de los campos esta vacio."; ;

            return string.IsNullOrWhiteSpace(errorMessage);
        }


        public string SaveToken(TokenModel model)
        {
            var nroToken = GenerarToken(model);
            _tokenRepository.SaveToken(model, nroToken);
            return nroToken;
        }

        public bool ValidarTokenModel(TokenModelValidar model, out string errorMessage)
        {
            errorMessage = string.Empty;
            var contexto = _tokenRepository.GetContexto(model.Contexto);

            if (contexto == null) errorMessage = "El contexto no existe.";
            else if (!contexto.Estado) errorMessage = "El contexto no se encuentra activo.";
            else if (model.Campos.Count < contexto.NroCampos) errorMessage = "La cantidad de campos no es la requerida por el contexto.";
            else if (model.Campos.Any(campo => string.IsNullOrWhiteSpace(campo.Llave) || string.IsNullOrWhiteSpace(campo.Valor))) errorMessage = "Un valor de los campos no es válido.";
            else if (!_tokenRepository.TokenExiste(model.Contexto, model.Token)) errorMessage = "El token no existe o ya expiro.";
            else if (!ValidarToken(model, contexto, out var msg)) errorMessage = msg;

            return string.IsNullOrWhiteSpace(errorMessage);
        }

        private string GenerarToken(TokenModel model)
        {
            var nroToken = GetNroToken(_tokenRepository.GetLengthContext(model.Contexto));
            return _tokenRepository.TokenExiste(model.Contexto, nroToken) ? GenerarToken(model) : nroToken;
        }

        private string GetNroToken(int lengthContext)
        {
            var nroMaxStr = "".PadLeft(lengthContext, '9');
            var nroMax = int.Parse(nroMaxStr);
            Random generator = new Random();
            var nroToken = generator.Next(0, nroMax).ToString($"D{lengthContext}");
            return nroToken;
        }

        private bool ValidarToken(TokenModelValidar model, Contexto contexto, out string errorMessage)
        {
            errorMessage = string.Empty;
            var token = _tokenRepository.GetToken(model);

            if (DateTime.Now.Subtract(token.FechaGeneracion.Value).TotalHours > contexto.Vigencia)
            {
                errorMessage = "El token ya expiro.";
                _tokenRepository.UpdateEstadoToken(EstadoToken.Expirado, token);
            }
            else if (!ValidarCampos(model, token)) errorMessage = "Los parametros enviados no corresponde al contexto";
            else _tokenRepository.UpdateEstadoToken(EstadoToken.Validado, token);
            return string.IsNullOrWhiteSpace(errorMessage);
        }

        private bool ValidarCampos(TokenModelValidar model, Token token)
        {
            var camposDicBD = JsonConvert.DeserializeObject<Dictionary<string, string>>(token.Campos);

            Dictionary<string, string> camposRequest = model.Campos.ToDictionary();

            return camposDicBD.Keys.All(key => camposRequest.ContainsKey(key) && camposRequest[key] == camposDicBD[key]);
        }

    }
}
