using MovistarToken.Common;
using MovistarToken.Infrastructure;
using MovistarToken.Models;
using MovistarToken.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovistarToken.Data
{
    public class TokenRepository : ITokenRepository
    {
        private readonly TokenContext _context;

        public TokenRepository(TokenContext tokenContext) => _context = tokenContext;

        public Contexto GetContexto(string nombreContexto) => _context.Contexto.FirstOrDefault(c => c.Nombre == nombreContexto);


        public Token GetToken(ValidTokenRequest validTokenRequest) =>
                _context.Token.FirstOrDefault(x => x.NombreContexto == validTokenRequest.Contexto
                                       && x.NroToken == validTokenRequest.Token
                                       && x.Estado == TokenType.Generado.Value);

        public void SaveToken(ITokenRequest tokenRequest, string tokenCode)
        {
            var camposDic = tokenRequest.Campos.ToDictionary();
            var txtCampos = JsonConvert.SerializeObject(camposDic);

            var token = new Token
            {
                Campos = txtCampos,
                Estado = TokenType.Generado.Value,
                FechaGeneracion = DateTime.Now,
                NombreContexto = tokenRequest.Contexto,
                NroToken = tokenCode
            };

            _context.Token.Add(token);
            _context.SaveChanges();
        }

        public void UpdateToken(Token token, TokenType tokenType)
        {
            token.FechaValidacion = DateTime.Now;
            token.Estado = tokenType.Value;
            _context.Update(token);
            _context.SaveChanges();
        }
        public int UpdateIntentoToken(Token token, TokenType tokenType, int maxIntentos)
        {
            if (token.Intento is null) token.Intento = 0;

            token.Intento += 1;
            token.FechaValidacion = DateTime.Now;
            if (token.Intento >= maxIntentos)
                token.Estado = TokenType.Excedido.Value;
            _context.Update(token);
            _context.SaveChanges();
            return token.Intento.Value;
        }

        public bool AnyTokenCode(string nombreContexto, string tokenCode)
        {
            return _context.Token.Any(x => x.NombreContexto == nombreContexto
                                       && x.NroToken == tokenCode
                                       && x.Estado == TokenType.Generado.Value);
        }
    }
}
