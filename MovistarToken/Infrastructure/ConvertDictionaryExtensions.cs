using MovistarToken.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovistarToken.Infrastructure
{
    public static class ConvertDictionaryExtensions
    {
        public static Dictionary<string, string> ToDictionary(this List<TokenField> campos) =>
            new Dictionary<string, string>(campos.Select(campo => new KeyValuePair<string, string>(campo.Llave?.ToLower(), campo.Valor?.ToLower())));

        public static List<TokenField> ToCampos(this Dictionary<string, string> dictionary) => dictionary.Select(kvp => new TokenField { Llave = kvp.Key, Valor = kvp.Value }).ToList();

    }
}
