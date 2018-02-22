using System;
using System.Linq;
using MovistarTokenTest.Common;
using MovistarTokenTest.Infrastructure;
using MovistarTokenTest.Logic;
using Newtonsoft.Json;

namespace MovistarTokenTest.Data
{
    public class TokenRepository : ITokenRepository
    {
        private readonly TokenContext _context;
        public TokenRepository(TokenContext context) => _context = context;
        public Contexto GetContexto(string nombre) => _context.Contexto.FirstOrDefault(x => x.Nombre == nombre);
        public int GetLengthContext(string nombre) => Convert.ToInt32(_context.Contexto.First(x => x.Nombre == nombre).NroDigitos);
        public bool TokenExiste(string nombre, string nroToken) => _context.Token.Any(x => x.NombreContexto == nombre && x.NroToken == nroToken && x.Estado == EstadoToken.Generado.Value);

        public void SaveToken(TokenModel model, string nroToken)
        {
            var camposDic = model.Campos.ToDictionary();
            var txtCampos = JsonConvert.SerializeObject(camposDic);

            var token = new Token
            {
                Campos = txtCampos,
                Estado = EstadoToken.Generado.Value,
                FechaGeneracion = DateTime.Now,
                NombreContexto = model.Contexto,
                NroToken = nroToken
            };

            _context.Token.Add(token);
            _context.SaveChanges();
        }

        public Token GetToken(TokenModelValidar model) => _context.Token.First(x => x.NombreContexto == model.Contexto && x.NroToken == model.Token && x.Estado == EstadoToken.Generado.Value);

        public void UpdateEstadoToken(EstadoToken estadoToken, Token token)
        {
            token.FechaValidacion = DateTime.Now;
            token.Estado = estadoToken.Value;
            _context.Update(token);
            _context.SaveChanges();
        }


    }
}
