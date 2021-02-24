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
                var FechaFinD = Hoy.AddMinutes(-35);
                var FechaFin = FechaFinD.AddHours(5);
                var FechaInicioD = Hoy.AddDays(-item.PeriodoEjecucion);
                var FechaInicio = FechaInicioD.AddHours(5);


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
                    DateTime? fechaValidacion = DateTime.Now;

                    if (item2.FechaValidacion != null)
                        fechaValidacion = Convert.ToDateTime(item2.FechaValidacion).AddHours(-5);
                    else
                        fechaValidacion = item2.FechaValidacion;

                    DateTime? fechaEnvioNotificacion = DateTime.Now ;

                    var detalletoken = (from x in _context.DetalleToken
                                        where x.IdToken == IdtokenBuscar
                                        select x).ToList();

                    foreach (var detalle in detalletoken) {

                        if (detalle.FechaEnvioNotificacion != null)
                            fechaEnvioNotificacion = Convert.ToDateTime(detalle.FechaEnvioNotificacion).AddHours(-5);
                        else
                            fechaEnvioNotificacion = detalle.FechaEnvioNotificacion;
                    }

                    //var fechaEnvioNotificacionToken = fechaEnvioNotificacion;

                    var _accessToken = "";
                    var tokenAuth = (from x in _context.TokenAuth
                                     select x).ToList();

                    foreach (var item3  in tokenAuth)
                    {
                        _accessToken = item3.AccessToken;

                    }


                    var NumDoc = "";
                    var TipoDoc = "";
                    if (item2.NumeroDoc == "" || item2.TipoDoc == "")
                    {
                        TipoDoc = "DNI";
                        NumDoc = item2.DNI;
                    }
                    else
                    {
                        NumDoc = item2.NumeroDoc;
                        TipoDoc = item2.TipoDoc;
                    }

                    EnvioEventNotificationRequest request = new EnvioEventNotificationRequest();
                    request.eventType = "dinamicNotification";
                    //request.eventTime = "2020-07-14T02:22:56.979Z";
                    request.eventTime = Convert.ToDateTime(DateTime.Now).AddHours(-5).ToString("yyyy-MM-ddTHH:mm:ss.fffffffK");
                    request.eventId = "-h020c2dbfq0pkqq2p1m";
                    request.eventSource = "GESTOKEN";
                    //request.relatedEntity = new List<EnvioEventNotificationRequest.relatedEntity_>();
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
                            entityType = "Customer",
                            additionalData = new List<EnvioEventNotificationRequest.additionalData_>
                            {
                                new EnvioEventNotificationRequest.additionalData_
                                {
                                    KeyValueType = new EnvioEventNotificationRequest.KeyValueType_
                                    {
                                        Key = "nationalIDType",
                                        Value = TipoDoc
                                    }
                                },
                                new EnvioEventNotificationRequest.additionalData_
                                {
                                    KeyValueType = new EnvioEventNotificationRequest.KeyValueType_
                                    {
                                        Key = "nationalID",
                                        Value = NumDoc
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
                                        Value = Convert.ToString(item2.IdTransaccion)
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
                                        Key = "publicId",
                                        Value = Convert.ToString(item2.Telefono)
                                    }
                                }
                            }

                        },
                        new EnvioEventNotificationRequest.dinamicEntity_{
                            entityType = "NotificacionSMS",
                            additionalData = new List<EnvioEventNotificationRequest.additionalData_>
                            {
                                 new EnvioEventNotificationRequest.additionalData_
                                {
                                    KeyValueType = new EnvioEventNotificationRequest.KeyValueType_
                                    {
                                        Key = "type",
                                        Value = "SMS"
                                    }
                                },
                                 new EnvioEventNotificationRequest.additionalData_
                                {
                                    KeyValueType = new EnvioEventNotificationRequest.KeyValueType_
                                    {
                                        Key = "Token",
                                        Value = item2.NroToken
                                    }
                                },
                                 new EnvioEventNotificationRequest.additionalData_
                                {
                                    KeyValueType = new EnvioEventNotificationRequest.KeyValueType_
                                    {
                                        Key = "fechaGeneracionToken",
                                        Value = Convert.ToDateTime(item2.FechaGeneracion).AddHours(-5).ToString("yyyy-MM-ddTHH:mm:ss.fffffffK")
                                    }
                                },
                                 new EnvioEventNotificationRequest.additionalData_
                                {
                                    KeyValueType = new EnvioEventNotificationRequest.KeyValueType_
                                    {
                                        Key = "fechaEnvioToken",
                                        Value = ( fechaEnvioNotificacion is null ) ? fechaEnvioNotificacion.ToString(): Convert.ToDateTime(fechaEnvioNotificacion).ToString("yyyy-MM-ddTHH:mm:ss.fffffffK")
                                    }
                                },
                                 new EnvioEventNotificationRequest.additionalData_
                                {
                                    KeyValueType = new EnvioEventNotificationRequest.KeyValueType_
                                    {
                                        Key = "fechaValidacionToken",
                                        Value = ( fechaValidacion is null ) ? fechaValidacion.ToString(): Convert.ToDateTime(fechaValidacion).ToString("yyyy-MM-ddTHH:mm:ss.fffffffK")
                                    }
                                },
                                 new EnvioEventNotificationRequest.additionalData_
                                {
                                    KeyValueType = new EnvioEventNotificationRequest.KeyValueType_
                                    {
                                        Key = "numeroEnviosToken",
                                        Value = "1"
                                    }
                                },
                                 new EnvioEventNotificationRequest.additionalData_
                                {
                                    KeyValueType = new EnvioEventNotificationRequest.KeyValueType_
                                    {
                                        Key = "estadoValidacionToken",
                                        Value = item2.Estado
                                    }
                                },
                                 new EnvioEventNotificationRequest.additionalData_
                                {
                                    KeyValueType = new EnvioEventNotificationRequest.KeyValueType_
                                    {
                                        Key = "cantidadEnviosToken",
                                        Value = "1"
                                    }
                                }
                            }

                        }
                    }

                        }


                    });


                    EnvioEventNotificationResponse rpta = EnviarTokenServicioEventNotification(request, _accessToken).Result;


                    var enviar = "No";
                    if (rpta.code == "201" || rpta.code =="200")
                    {
                        enviar = "Si";
                    }

                    if (enviar =="Si" )
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
                    } else {
                        foreach (var dt in detalletoken)
                        {
                            dt.IdToken = item2.IdToken;
                            dt.CodigoRespuestaEvent = "0";
                            dt.MensajeRespuestaEvent = "Error envio event";
                            dt.NumeroReintentosEvent = 1;
                            dt.FechaEnvioEvent = DateTime.Now;
                            dt.OrigenEnvioEvent = "offline";
                            //dt.EstadoEvent = true;
                            //dt.TokenValidado = item2.NroToken;
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

            var uri = "https://apis.telefonica.com.pe/vp-tecnologia/bss/eventnotification/eventNotification/v2/EVENT";
            var json = JsonConvert.SerializeObject(request);


            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            Console.WriteLine("JSON OFFLINE:" + json.ToString());

            var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            client.DefaultRequestHeaders.Add("X-IBM-Client-Id", "bf76b7fe-d5b5-45e3-8b18-e7c1976be3aa");

            var response = await client.PostAsync(uri, stringContent);

            string response2 = await response.Content.ReadAsStringAsync();
            respuesta = JsonConvert.DeserializeObject<EnvioEventNotificationResponse>(response2);
            return respuesta;

        }

    }

}
