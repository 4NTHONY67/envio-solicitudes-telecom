using MovistarToken.Common;
using MovistarToken.Data;
using MovistarToken.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovistarToken.Repositories
{
    public interface ITokenRepository
    {
        Contexto GetContexto(string nombreContexto);
        Token GetToken(ValidTokenRequest validTokenRequest);
        void SaveToken(ITokenRequest tokenRequest, string tokenCode);
        void UpdateToken(Token token, TokenType tokenType);
        int UpdateIntentoToken(Token token, TokenType tokenType, int maxIntentos);
        bool AnyTokenCode(string nombreContexto, string tokenCode);
        void EnviarEventNotification(Token token);
    }
}
