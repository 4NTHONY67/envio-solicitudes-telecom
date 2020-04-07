using Microsoft.AspNetCore.Http;
using MovistarToken.Common;
using MovistarToken.Data;
using MovistarToken.Infrastructure;
using MovistarToken.Models;
using MovistarToken.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovistarToken.Services
{
    public class TokenService : ITokenService
    {
        private readonly ITokenRepository _tokenRepository;
        private const string Letters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string Numbers = "0123456789";

        public TokenService(ITokenRepository tokenRepository) => _tokenRepository = tokenRepository;

        #region Public Methods

        public bool GenerateToken(ITokenRequest tokenRequest, out string token, out string errorMessage)
        {
            token = string.Empty;
            var contexto = _tokenRepository.GetContexto(tokenRequest.Contexto);
            if (!ValidateTokenRequest(tokenRequest, contexto, out errorMessage)) { return false; }
            token = GenerateTokenFromContexto(contexto);
            _tokenRepository.SaveToken(tokenRequest, token);
            return true;
        }

        public bool ValidateToken(ITokenRequest tokenRequest, out string errorMessage)
        {
            errorMessage = string.Empty;
            var contexto = _tokenRepository.GetContexto(tokenRequest.Contexto);
            if (!ValidateTokenRequest(tokenRequest, contexto, out errorMessage)) { return false; }


            int longitud = tokenRequest.Campos.Count();
            int telefono = 0;
            for (int i = 0; i < longitud; i++)
            {
                if (tokenRequest.Campos[i].Llave == "telefono")
                {
                    telefono = Convert.ToInt32(tokenRequest.Campos[i].Valor);
                }

            }
            tokenRequest.Telefono = telefono;
            var token = _tokenRepository.GetToken(tokenRequest as ValidTokenRequest);
            if (!ValidateTokenContexto(token, tokenRequest as ValidTokenRequest, contexto, out errorMessage)) { return false; }
            return string.IsNullOrWhiteSpace(errorMessage);
        }
        #endregion



        #region Private Methods
        private bool ValidateTokenRequest(ITokenRequest tokenRequest, Contexto contexto, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (!ValidateContexto(out errorMessage)) {; }
            else if (!ValidateFieldsFromRequest(tokenRequest.Campos, out errorMessage)) {; }
            else if (!ValidateFieldsFromContexto()) errorMessage = "Los campos ingresados no pertenecen al contexto";
            return string.IsNullOrWhiteSpace(errorMessage);

            bool ValidateContexto(out string errorMessage1)
            {
                errorMessage1 = string.Empty;
                if (contexto == null) errorMessage1 = "El contexto no existe.";
                else if (!contexto.Estado) errorMessage1 = "El contexto no se encuentra activo.";
                return string.IsNullOrWhiteSpace(errorMessage1);

            }
            bool ValidateFieldsFromRequest(List<TokenField> requestFields, out string errorMessage1)
            {
                errorMessage1 = string.Empty;

                if (requestFields.Count < contexto.NroCampos) errorMessage1 = "La cantidad de campos no es la requerida por el contexto.";
                else if (requestFields.Count < contexto.NroCampos) errorMessage1 = "La cantidad de campos no es la requerida por el contexto.";
                else if (requestFields.Any(campo => string.IsNullOrWhiteSpace(campo.Llave) || string.IsNullOrWhiteSpace(campo.Valor))) errorMessage1 = "Un valor de los campos esta vacio.";
                else if (IsFieldsDuplicates()) errorMessage1 = "Una o mas llaves son inguales.";

                bool IsFieldsDuplicates()
                {
                    var xg = requestFields.GroupBy(x => x.Llave, StringComparer.InvariantCultureIgnoreCase).ToList();
                    var xc = xg.Where(x => x.Count() > 1).ToList();
                    var result = xc.Count > 0;
                    return result;
                }

                return string.IsNullOrWhiteSpace(errorMessage1);
            }
            bool ValidateFieldsFromContexto()
            {
                var fieldsInContexto = contexto.NombreCampos.Split(',').ToList();
                var fieldsInToken = tokenRequest.Campos.Select(x => x.Llave).ToList();
                foreach (var field in fieldsInContexto)
                {
                    if (!fieldsInToken.Contains(field.Trim(), StringComparer.CurrentCultureIgnoreCase)) return false;
                }
                return true;
            }
        }

        private string GenerateTokenFromContexto(Contexto contexto)
        {
            var source = contexto.EsAlfanumerico ? Letters : Numbers;
            return GenerateTokenCode(source, contexto.NroDigitos);

            string GenerateTokenCode(string letters, int maxLength)
            {
                Random rand = new Random();
                int maxRand = letters.Length - 1;

                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < maxLength; i++)
                {
                    int index = rand.Next(maxRand);
                    sb.Append(letters[index]);
                }
                var tokenCode = sb.ToString();
                return _tokenRepository.AnyTokenCode(contexto.Nombre, tokenCode) ? GenerateTokenCode(source, contexto.NroDigitos) : tokenCode;
            }
        }

        private bool ValidateTokenContexto(Token token, ValidTokenRequest tokenRequest, Contexto contexto, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (token is null) errorMessage = "El token no existe o ya expiro.";
            else if (!ValidateTime()) errorMessage = "El token ya expiro.";
            else if (!ValidateFields(out errorMessage)) {; }
            else _tokenRepository.UpdateToken(token, TokenType.Validado);
            return string.IsNullOrWhiteSpace(errorMessage);

            bool ValidateFields(out string errorMessage1)
            {
                errorMessage1 = string.Empty;
                var camposDic = tokenRequest.Campos.ToDictionary();
                var camposText = JsonConvert.SerializeObject(camposDic);
                if (!token.Campos.Equals(camposText, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (_tokenRepository.UpdateIntentoToken(token, TokenType.Intento, contexto.Reintentos) >= contexto.Reintentos)
                        errorMessage1 = "El token ha excedido sus intentos";
                    else
                        errorMessage1 = "Uno de los valores es incorrecto.";
                    return false;
                }
                return true;
            }
            bool ValidateTime()
            {
                if (DateTime.Now.Subtract(token.FechaGeneracion.Value).TotalSeconds > contexto.Vigencia)
                {
                    _tokenRepository.UpdateToken(token, TokenType.Expirado);
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        #endregion
    }
}
