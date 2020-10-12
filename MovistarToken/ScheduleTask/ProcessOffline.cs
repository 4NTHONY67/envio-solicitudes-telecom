using Microsoft.Extensions.DependencyInjection;
using MovistarToken.BackgroundService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovistarToken.ScheduleTask
{
    public class ProcessOffline : ScheduledProcessor
    {
        public ProcessOffline(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {

        }
        protected override string Schedule => "*/1 * * * *"; // cada 1 minuto

        public override Task ProcessInScope(IServiceProvider scopeServiceProvider)
        {
            Console.WriteLine("Proceso Offline:" + DateTime.Now.ToString());
            return Task.CompletedTask;
        }
    }
}
