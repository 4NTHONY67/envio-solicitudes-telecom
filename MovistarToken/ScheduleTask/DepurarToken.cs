using Microsoft.Extensions.DependencyInjection;
using MovistarToken.BackgroundService;
using MovistarToken.Data;
using MovistarToken.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovistarToken.ScheduleTask
{
    public class DepurarToken : ScheduledProcessor
    {
        // private readonly ITokenRepository _tokenRepository;
        //protected List<string> list = new List<string>();
        // private readonly ITokenRepository _tokenRepository;
        //TokenRepository tokenRepository = new TokenRepository();
        public DepurarToken(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {
            // _tokenRepository = tokenRepository;
        }


        protected override string Schedule => "*/1 * * * *"; // cada 1 minuto

        public override Task ProcessInScope(IServiceProvider scopeServiceProvider)
        {


            ITokenRepository tokenRepository = scopeServiceProvider.GetRequiredService<ITokenRepository>();
            tokenRepository.IniciarDepuracion();

            Console.WriteLine("Depurar Token:" + DateTime.Now.ToString());
            return Task.CompletedTask;
        }
    }
}
