using Microsoft.AspNetCore.Rewrite.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Identity;
using MovistarToken.BackgroundService;
using MovistarToken.Data;
using MovistarToken.Models;
using Newtonsoft.Json;
using Remotion.Linq.Clauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MovistarToken.ScheduleTask
{
    public class ProcessOffline : ScheduledProcessor
    {
        public ProcessOffline(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {

        }
        protected override string Schedule => "30 0 * * *";  // a las 00:30:00 todos los dias

        public override Task ProcessInScope(IServiceProvider scopeServiceProvider)
        {
           var   _context = scopeServiceProvider.GetRequiredService<TokenContext>();
           

            var query = (from x in _context.TokenServicio
                         where x.Estado == "activado"
                         select x).ToList();

            foreach (var item in query)
            {                
                var Hoy = DateTime.Now;
                var FechaFin = Hoy.AddMinutes(-35);
                var FechaInicio = Hoy.AddDays(-8);


                var query2 = (from x in _context.Token 
                               join y in _context.DetalleToken on x.IdToken equals y.IdToken
                               where x.FechaGeneracion >= FechaInicio
                               && x.FechaGeneracion <= FechaFin
                               && x.NombreContexto == item.NombreContexto
                               && y.EstadoEvent == false
                               select x).ToList();
                


                foreach (var item2 in query2)
                {
                    int IdtokenBuscar = item2.IdToken;
                    var detalletoken = (from x in _context.DetalleToken
                                        where x.IdToken == IdtokenBuscar
                                        select x).ToList();


                    var _accessToken = "";
                    var tokenAuth = (from x in _context.TokenAuth
                                     select x).ToList();

                    foreach (var item3  in tokenAuth)
                    {
                        _accessToken = item3.AccessToken;

                    }

                    EnvioEventNotificationRequest request = new EnvioEventNotificationRequest();
                    request.eventType = "dinamicNotification";
                    request.eventTime = "2020-07-14T02:22:56.979Z";
                    request.eventId = "-h020c2dbfq0pkqq2p1m";
                    request.eventSource = "GESTOKEN";
                    request.relatedEntity = new List<EnvioEventNotificationRequest.relatedEntity_> {
                    new EnvioEventNotificationRequest.relatedEntity_{ entityType = "string",
                                                                         id = "string",
                                                                         href ="string" }};
                    request.Event = new EnvioEventNotificationRequest.event_();
                    request.Event.@type = "dinamicNotification";
                    request.Event.@schemaLocation = new List<EnvioEventNotificationRequest.schemaLocation_>();
                    request.Event.@schemaLocation.Add(new EnvioEventNotificationRequest.schemaLocation_
                    {
                        dinamicNotification = new EnvioEventNotificationRequest.dinamicNotification_()
                        {
                            dinamicEntity = new List<EnvioEventNotificationRequest.dinamicEntity_>
                    {
                        new EnvioEventNotificationRequest.dinamicEntity_{
                            entityType = "TOPIC",
                            additionalData = new List<EnvioEventNotificationRequest.additionalData_>
                            {
                                new EnvioEventNotificationRequest.additionalData_
                                {
                                    KeyValueType = new EnvioEventNotificationRequest.KeyValueType_
                                    {
                                        Key = "queue-eventNotifications-tokens",
                                        Value = "YYYYY"
                                    }
                                }
                            }

                        },
                        new EnvioEventNotificationRequest.dinamicEntity_{
                            entityType = "Order",
                            additionalData = new List<EnvioEventNotificationRequest.additionalData_>
                            {
                                new EnvioEventNotificationRequest.additionalData_
                                {
                                    KeyValueType = new EnvioEventNotificationRequest.KeyValueType_
                                    {
                                        Key = "OrderID",
                                        Value = "12345676"
                                    }
                                }
                            }

                        },
                        new EnvioEventNotificationRequest.dinamicEntity_{
                            entityType = "Product",
                            additionalData = new List<EnvioEventNotificationRequest.additionalData_>
                            {
                                new EnvioEventNotificationRequest.additionalData_
                                {
                                    KeyValueType = new EnvioEventNotificationRequest.KeyValueType_
                                    {
                                        Key = "productType",
                                        Value = "WRLS"
                                    }
                                },
                                 new EnvioEventNotificationRequest.additionalData_
                                {
                                    KeyValueType = new EnvioEventNotificationRequest.KeyValueType_
                                    {
                                        Key = "App",
                                        Value = "GESTOKEN"
                                    }
                                },
                                 new EnvioEventNotificationRequest.additionalData_
                                {
                                    KeyValueType = new EnvioEventNotificationRequest.KeyValueType_
                                    {
                                        Key = "Telefono",
                                        Value = Convert.ToString(item2.Telefono)
                                    }
                                },
                                 new EnvioEventNotificationRequest.additionalData_
                                {
                                    KeyValueType = new EnvioEventNotificationRequest.KeyValueType_
                                    {
                                        Key = "OrdenId",
                                        Value = ""
                                    }
                                },
                                 new EnvioEventNotificationRequest.additionalData_
                                {
                                    KeyValueType = new EnvioEventNotificationRequest.KeyValueType_
                                    {
                                        Key = "TipoDoc",
                                        Value = item2.TipoDoc
                                    }
                                },
                                 new EnvioEventNotificationRequest.additionalData_
                                {
                                    KeyValueType = new EnvioEventNotificationRequest.KeyValueType_
                                    {
                                        Key = "NumeroDoc",
                                        Value = item2.NumeroDoc
                                    }
                                },
                                 new EnvioEventNotificationRequest.additionalData_
                                {
                                    KeyValueType = new EnvioEventNotificationRequest.KeyValueType_
                                    {
                                        Key = "IdTransaccion",
                                        Value = item2.IdTransaccion
                                    }
                                },
                                 new EnvioEventNotificationRequest.additionalData_
                                {
                                    KeyValueType = new EnvioEventNotificationRequest.KeyValueType_
                                    {
                                        Key = "NombreContexto",
                                        Value = item2.NombreContexto
                                    }
                                },
                                 new EnvioEventNotificationRequest.additionalData_
                                {
                                    KeyValueType = new EnvioEventNotificationRequest.KeyValueType_
                                    {
                                        Key = "TokenIngresado",
                                        Value = item2.NroToken
                                    }
                                },
                                 new EnvioEventNotificationRequest.additionalData_
                                {
                                    KeyValueType = new EnvioEventNotificationRequest.KeyValueType_
                                    {
                                        Key = "FechaGeneracion",
                                        Value = item2.FechaGeneracion.ToString()
                                    }
                                },
                                 new EnvioEventNotificationRequest.additionalData_
                                {
                                    KeyValueType = new EnvioEventNotificationRequest.KeyValueType_
                                    {
                                        Key = "FechaEnvioNotificacion",
                                        Value = DateTime.Now.ToString()
                                    }
                                },
                                 new EnvioEventNotificationRequest.additionalData_
                                {
                                    KeyValueType = new EnvioEventNotificationRequest.KeyValueType_
                                    {
                                        Key = "FechaValidacion",
                                        Value = item2.FechaValidacion.ToString()
                                    }
                                },
                                 new EnvioEventNotificationRequest.additionalData_
                                {
                                    KeyValueType = new EnvioEventNotificationRequest.KeyValueType_
                                    {
                                        Key = "Intento",
                                        Value = item2.Intento.ToString()
                                    }
                                },
                                 new EnvioEventNotificationRequest.additionalData_
                                {
                                    KeyValueType = new EnvioEventNotificationRequest.KeyValueType_
                                    {
                                        Key = "Estado",
                                        Value = item2.Estado
                                    }
                                }
                            }

                          }
                        }

                      }


                    });

                   

                    EnvioEventNotificationResponse rpta = EnviarTokenServicioEventNotification(request, _accessToken).Result;

                   

                    if (rpta.code == "201")
                    {
                     

                      
                       

                        foreach (var dt in detalletoken)
                        {
                            dt.IdToken = item2.IdToken;
                            dt.CodigoRespuestaEvent = rpta.code;
                            dt.MensajeRespuestaEvent = rpta.description;
                            dt.NumeroReintentosEvent = 1;
                            dt.FechaEnvioEvent = DateTime.Now;
                            dt.OrigenEnvioEvent = "offline";
                            dt.EstadoEvent = true;
                            dt.TokenValidado = item2.NroToken;
                            _context.Update(dt);
                            _context.SaveChanges();
                        }
                    }
                }
               
            }

            Console.WriteLine("Proceso Offline:" + DateTime.Now.ToString());
            return Task.CompletedTask;

        }


        public async Task<EnvioEventNotificationResponse> EnviarTokenServicioEventNotification(EnvioEventNotificationRequest request, string _accessToken)
        {
            EnvioEventNotificationResponse respuesta;

            var uri = "https://apisd.telefonica.com.pe/vp-tecnologia/bss/eventnotification/eventNotification/v2/EVENT";
            var json = JsonConvert.SerializeObject(request);


            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            client.DefaultRequestHeaders.Add("X-IBM-Client-Id", "88d1769b-521f-49d9-bfc9-35f15c336698");

            var response = await client.PostAsync(uri, stringContent);

            string response2 = await response.Content.ReadAsStringAsync();
            respuesta = JsonConvert.DeserializeObject<EnvioEventNotificationResponse>(response2);
            return respuesta;

        }

    }

}
