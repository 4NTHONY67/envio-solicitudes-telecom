using MovistarTokenTest.Common;
using MovistarTokenTest.Data;

namespace MovistarTokenTest.Logic
{
    public interface ITokenRepository
    {
        Contexto GetContexto(string nombre);
        int GetLengthContext(string nombre);
        bool TokenExiste(string nombre, string nroToken);
        void SaveToken(TokenModel model, string nroToken);
        Token GetToken(TokenModelValidar model);
        void UpdateEstadoToken(EstadoToken estadoToken, Token token);
    }
}
