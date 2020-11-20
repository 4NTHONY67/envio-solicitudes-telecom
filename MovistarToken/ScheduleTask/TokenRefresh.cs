using Microsoft.Extensions.DependencyInjection;
using MovistarToken.BackgroundService;
using MovistarToken.Data;
using MovistarToken.Models;
using MovistarToken.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MovistarToken.ScheduleTask
{
    public class TokenRefresh : ScheduledProcessor
    {
        public TokenRefresh(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {

        }


        protected override string Schedule => "*/50 * * * *"; // a las 00:01:00 ,el dia 1 de cada mes

        public override Task ProcessInScope(IServiceProvider scopeServiceProvider)
        {
            var _context = scopeServiceProvider.GetRequiredService<TokenContext>();

            var _accessToken = "";

            var tokenAuth = (from x in _context.TokenAuth
                             select x).ToList();

            foreach (var item in tokenAuth)
            {
                _accessToken = item.AccessToken;

            }

            TokenRefreshResponse rpta = ActualizarToken(_accessToken).Result;


            var tokenUpdate = (from x in _context.TokenAuth
                               select x).ToList();


            foreach (var dt in tokenUpdate)
            {
                dt.AccessToken = rpta.access_token;
                dt.RefreshToken = rpta.refresh_token;
 
                _context.Update(dt);
                _context.SaveChanges();
            }


            Console.WriteLine("Token Refresh:" + DateTime.Now.ToString());
            return Task.CompletedTask;
        }



        public async Task<TokenRefreshResponse> ActualizarToken(string  refreshToken)
        {
            TokenRefreshResponse respuesta;

            var url = "https://apisd.telefonica.com.pe/vp-tecnologia/bss/public/oauth2/token";
            var dict = new Dictionary<string, string>();
            dict.Add("refresh_token", refreshToken);
            dict.Add("client_id", "3d980602-0c5f-4ec7-ac3f-d51ce62ff476");
            dict.Add("grant_type", "refresh_token");
            dict.Add("scope", "scope1");
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Content-Type", "application/x-www-form-urlencoded");
            var req = new HttpRequestMessage(HttpMethod.Post, url) { Content = new FormUrlEncodedContent(dict) };
            var  response = await client.SendAsync(req);

            string response2 = await response.Content.ReadAsStringAsync();
            respuesta = JsonConvert.DeserializeObject<TokenRefreshResponse>(response2);
            return respuesta;
        }

    }
}
