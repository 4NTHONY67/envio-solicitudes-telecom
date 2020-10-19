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
        public DepurarToken(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {
           
        }


        protected override string Schedule => "*/5 * * * *"; // cada 1 minuto

        public override Task ProcessInScope(IServiceProvider scopeServiceProvider)
        {
            var _context = scopeServiceProvider.GetRequiredService<TokenContext>();

            var query = (from x in _context.TokenDepuracion 
                         where x.Estado == "activado"
                         select x).ToList();

            var allowedStatus = new[] { "Validado", "Expirado", "NoUsado","Excedido","No Usado" };

            var Hoy = DateTime.Now;
            var FechaInicio = Hoy.AddMonths(-2);
            var FechaFin = Hoy.AddDays(-20);

            foreach (var item in query)
            {
                string anioDepuracion = Convert.ToDateTime(item.FechaEjecucion).ToString("yyyy");
                string mesDepuracion = Convert.ToDateTime(item.FechaEjecucion).ToString("MM");
                string diaDepuracion = Convert.ToDateTime(item.FechaEjecucion).ToString("dd");
                string diadia = Convert.ToDateTime(item.FechaEjecucion).ToString("dd");

                if (anioDepuracion == DateTime.Now.ToString("yyyy") && mesDepuracion == DateTime.Now.ToString("MM") && diaDepuracion == DateTime.Now.ToString("dd"))
                {
                    var tokenDepurar = (from x in _context.Token
                                        where allowedStatus.Contains(x.Estado)
                                        && x.FechaGeneracion >= FechaInicio
                                        //&& x.FechaGeneracion >= item.FechaEjecucion
                                        //&& x.FechaGeneracion <= FechaFin
                                        //&& x.FechaGeneracion >= FechaInicio
                                        // && x.NombreContexto == item.NombreContexto
                                        select x).ToList();

                    foreach(var item2 in tokenDepurar) 
                    {
                        var tokenHistorico = new TokenHistorico
                        {
                            NombreContexto = item2.NombreContexto,
                            Campos = item2.Campos,
                            NroToken = item2.NroToken,
                            Estado = item2.Estado,
                            FechaGeneracion = item2.FechaGeneracion,
                            FechaValidacion = item2.FechaValidacion,
                            IdToken = item2.IdToken,
                            Intento = item2.Intento,
                            FechaExpiracion = item2.FechaExpiracion,
                            TipoDoc = item2.TipoDoc,
                            NumeroDoc = item2.NumeroDoc,
                            IdTransaccion = item2.IdTransaccion,
                            Telefono = item2.Telefono,
                            DetalleEstado = item2.DetalleEstado,
                            TokenIngresado = "",
                            FechaEjecucion = DateTime.Now
                        };

                        _context.TokenHistorico.Add(tokenHistorico);

                        _context.SaveChanges();
                        int ID = tokenHistorico.IdTokenHistorico;

                        var detalleTokenDelete = (from x in _context.DetalleToken
                                     where x.IdToken == item2.IdToken
                                     select x).ToList();

                        foreach (var item3 in detalleTokenDelete)
                        {
                            
                            DetalleTokenHistorico dth = new DetalleTokenHistorico();

                            dth.IdTokenHistorico = ID;
                            dth.IdToken = item3.IdToken;
                            dth.IdDetalleToken = item3.IdDetalleToken;
                            dth.FechaEnvioNotificacion = item3.FechaEnvioNotificacion;
                            dth.FechaRespuestaNotificacion = item3.FechaRespuestaNotificacion;
                            dth.CodigoNotificacion = item3.CodigoNotificacion;
                            dth.OrigenNotificacion = item3.OrigenNotificacion;
                            dth.MensajeNotificacion = item3.MensajeNotificacion;
                            dth.FechaRespuestaServicioToken = item3.FechaRespuestaServicioToken;
                            dth.CodigoRespuestaEvent = item3.CodigoRespuestaEvent;
                            dth.MensajeRespuestaEvent = item3.MensajeRespuestaEvent;
                            dth.NumeroReintentosEvent = item3.NumeroReintentosEvent;
                            dth.FechaEnvioEvent = item3.FechaEnvioEvent;
                            dth.OrigenEnvioEvent = item3.OrigenEnvioEvent;
                            dth.EstadoEvent = item3.EstadoEvent;
                            dth.TokenValidado = item3.TokenValidado;
                            dth.FechaDepuracion = DateTime.Now;

                            _context.DetalleTokenHistorico.Add(dth);
                           // _context.SaveChanges();

                            _context.DetalleToken.Remove(item3);
                            _context.SaveChanges();
                        }

                        _context.Token.Remove(item2);
                        _context.SaveChanges();
                    }
                    
                }

            }

            Console.WriteLine("Depurar Token:" + DateTime.Now.ToString());
            return Task.CompletedTask;
        }
    }
}
