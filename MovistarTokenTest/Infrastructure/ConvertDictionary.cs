using System.Collections.Generic;
using System.Linq;

namespace MovistarTokenTest.Infrastructure
{
    public static class ConvertDictionaryExtensions
    {
        public static Dictionary<string, string> ToDictionary(this List<CampoModel> campos) =>
            new Dictionary<string, string>(campos.Select(campo => new KeyValuePair<string, string>(campo.Llave?.ToLower(), campo.Valor?.ToLower())));

        public static List<CampoModel> ToCampos(this Dictionary<string, string> dictionary) => dictionary.Select(kvp => new CampoModel { Llave = kvp.Key, Valor = kvp.Value }).ToList();
           
    }
}
