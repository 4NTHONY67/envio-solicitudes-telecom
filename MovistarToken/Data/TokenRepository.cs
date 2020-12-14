using Microsoft.AspNetCore.Http;
using MovistarToken.Common;
using MovistarToken.Infrastructure;
using MovistarToken.Models;
using MovistarToken.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MovistarToken.Data
{
    public class TokenRepository : ITokenRepository
    {
        private readonly TokenContext _context;

        public TokenRepository(TokenContext tokenContext) => _context = tokenContext;

        public Contexto GetContexto(string nombreContexto) => _context.Contexto.FirstOrDefault(c => c.Nombre == nombreContexto);


        public Token GetToken(ValidTokenRequest validTokenRequest) =>
                _context.Token.FirstOrDefault(x => x.NombreContexto == validTokenRequest.Contexto
                                       && x.NroToken == validTokenRequest.Token
                                       && x.Telefono == validTokenRequest.Telefono
                                       && x.Estado == TokenType.Generado.Value);

        public void SaveToken(ITokenRequest tokenRequest, string tokenCode)
        {
            var camposDic = tokenRequest.Campos.ToDictionary();
            var txtCampos = JsonConvert.SerializeObject(camposDic);

            //int telefono = tokenRequest.Telefono;
            //int telefono = Convert.ToInt32(tokenRequest.Campos[3].Valor);
            int longitud = tokenRequest.Campos.Count();
            int telefono = 0;
            for (int i = 0; i < longitud; i++)
            {
                if (tokenRequest.Campos[i].Llave == "telefono")
                {
                    telefono = Convert.ToInt32(tokenRequest.Campos[i].Valor);
                }

            }

            string idTransaccion = "";
            for (int j = 0; j < longitud; j++)
            {
                if (tokenRequest.Campos[j].Llave == "idTransaction")
                {
                    idTransaccion = Convert.ToString(tokenRequest.Campos[j].Valor);
                }

            }

            string tipoDoc = "";
            for (int k = 0; k < longitud; k++)
            {
                if (tokenRequest.Campos[k].Llave == "tipoDoc")
                {
                    tipoDoc = Convert.ToString(tokenRequest.Campos[k].Valor);
                }

            }

            string numeroDoc = "";
            for (int x = 0; x < longitud; x++)
            {
                if (tokenRequest.Campos[x].Llave == "numeroDoc")
                {
                    numeroDoc = Convert.ToString(tokenRequest.Campos[x].Valor);
                }

            }

            string dni = "";
            for (int y = 0; y < longitud; y++)
            {
                if (tokenRequest.Campos[y].Llave == "dni")
                {
                    dni = Convert.ToString(tokenRequest.Campos[y].Valor);
                }

            }


            /* string idTransaccion = tokenRequest.IdTransaccion;
             if (tokenRequest.IdTransaccion is null) { idTransaccion = ""; } 
             string tipoDoc = tokenRequest.TipoDoc;
             if (tokenRequest.TipoDoc is null) { tipoDoc = ""; }
             string numeroDoc = tokenRequest.NumeroDoc;
             if (tokenRequest.NumeroDoc is null) { numeroDoc = ""; }*/
            var current = DateTime.Now;
            var token = new Token
            {
                Campos = txtCampos,
                Estado = TokenType.Generado.Value,
                FechaGeneracion = current,
                NombreContexto = tokenRequest.Contexto,
                NroToken = tokenCode,
                //Telefono = tokenRequest.Telefono,
                //Telefono = Convert.ToInt32(tokenRequest.Campos[3].Valor),
                Telefono = telefono,
                IdTransaccion = idTransaccion,
                TipoDoc = tipoDoc,
                NumeroDoc = numeroDoc,
                DNI = dni
            };


            //obtener contexto
            Contexto contex = new Contexto();
            contex = GetContexto(tokenRequest.Contexto);

            //Asignar Fecha de Expiracion
            token.FechaExpiracion = current.AddSeconds(contex.Vigencia);

            //Validar existencia de token 250320
            if (telefono>0) { 

                if (EvaluarRepeticionesConfiguracion(token)>=2) {

                    ActualizarEstadoToken(token);

                }

            }
            _context.Token.Add(token);

            _context.SaveChanges();

            int tokenId = token.IdToken;

            
            //gatilla servicios SMS
            if (GatillaServiciosSMS(token))
            {
                #region "preparar mensaje envio servicio"
                
                EnvioSMSRequest request = new EnvioSMSRequest();
                request.TefHeaderReq.userLogin = "USRGESTOKEN";
                request.TefHeaderReq.serviceChannel = "MS";
                request.TefHeaderReq.application = "GESTOKEN";
                request.TefHeaderReq.idMessage = "e53a566e-e53a-e53a-e53a-e53a566eefd2";
                request.TefHeaderReq.ipAddress = "169.54.245.69";
                request.TefHeaderReq.functionalityCode = "ServicioToken";
                request.TefHeaderReq.transactionTimestamp = "2020-01-20T08:23:55.177";
                request.TefHeaderReq.serviceName = "notifyAllSubscribers";
                request.TefHeaderReq.version = "1.0";
                request.NotifyAllSubscribersRequestData.mediaType = "SMS";
                request.NotifyAllSubscribersRequestData.notifyDataList = new EnvioSMSRequest.notifyDataList_();
                
                request.NotifyAllSubscribersRequestData.notifyDataList.notifyData.Add(new EnvioSMSRequest.notifyData_ {

                    smsData = new EnvioSMSRequest.smsData_() {

                        msisdnList = new EnvioSMSRequest.msisdnList_()
                        {
                            msisdn = new List<EnvioSMSRequest.msisdn_> {

                            new EnvioSMSRequest.msisdn_ { number = token.Telefono }
                            },
                        },
                    },

                    templateId = new EnvioSMSRequest.templateId_ {
                        templateId = contex.CodigoPlantillaSmS
                    },

                    templateParameterList = new EnvioSMSRequest.templateParameterList_() {

                        templateParameter = new List<EnvioSMSRequest.templateParameter_>
                        {
                           new EnvioSMSRequest.templateParameter_ { key = "TOKEN", value=token.NroToken}
                        },

                    }

                });

                //request.NotifyAllSubscribersRequestData.notifyDataList[0].smsData.msisdnList[0].number = token.Telefono;
                //request.NotifyAllSubscribersRequestData.notifyDataList[0].templateId.templateId = "AVM010";
                //request.NotifyAllSubscribersRequestData.notifyDataList[0].templateParameterList[0].key = "TOKEN";
                //request.NotifyAllSubscribersRequestData.notifyDataList[0].templateParameterList[0].value = token.NroToken;
                #endregion

                //enviar token a servicio SMS
                EnvioSMSResponse rpta = EnviarTokenServicioSMS(request).Result;
                var dateResponService = DateTime.Now;
                DetalleToken detalletoken;

                if (rpta.notifyAllSubscribersResponseData.status.code == 1)
                {
                    detalletoken = new DetalleToken
                    {
                        IdToken = tokenId,
                        FechaEnvioNotificacion = dateResponService,
                        FechaRespuestaNotificacion = dateResponService,
                        CodigoNotificacion = "1",
                        MensajeNotificacion = "Se envio correctamente a servicio SMS",
                        OrigenNotificacion = "GN",
                        FechaRespuestaServicioToken = dateResponService
                    };                    
                }
                else
                {
                    detalletoken = new DetalleToken
                    {
                        IdToken = tokenId,
                        FechaEnvioNotificacion = dateResponService,
                        FechaRespuestaNotificacion = dateResponService,
                        CodigoNotificacion = "2",
                        MensajeNotificacion = "Servicio de SMS no disponible",
                        OrigenNotificacion = "GN",
                        FechaRespuestaServicioToken = dateResponService
                    };

                    token.Estado = "NoUsado";
                    token.DetalleEstado = "Servicio de SMS no disponible";
                }
                //Asignar Fecha de Expiracion
                token.FechaExpiracion = dateResponService.AddSeconds(contex.Vigencia);
                

                _context.Token.Update(token);
                _context.DetalleToken.Add(detalletoken);
                _context.SaveChanges();

            }

            if (tokenRequest.Contexto == "AVM") 
            {
                DetalleToken detalletokenAVM;
                detalletokenAVM = new DetalleToken
                {
                    IdToken = tokenId
                   
                };

                _context.DetalleToken.Add(detalletokenAVM);
                _context.SaveChanges();
            }

        }

        public void UpdateToken(Token token, TokenType tokenType)
        {
            if (tokenType.Value == "Validado")
            {
                if (ValidarEnvioEventNotification(token))
                {
                    var query = (from x in _context.DetalleToken
                                 where x.IdToken == token.IdToken
                                 select x).ToList();

                    DateTime? fechaEnvioNotificaciontoken;

                    foreach (var item in query)
                    {
                        fechaEnvioNotificaciontoken = item.FechaEnvioNotificacion;
                        EnviarEventNotification(token, fechaEnvioNotificaciontoken);
                    }
                    
                }
            }

            token.FechaValidacion = DateTime.Now;
            token.Estado = tokenType.Value;
            _context.Update(token);
            _context.SaveChanges();
        }
        public int UpdateIntentoToken(Token token, TokenType tokenType, int maxIntentos)
        {
            if (token.Intento is null) token.Intento = 0;

            token.Intento += 1;
            token.FechaValidacion = DateTime.Now;
            if (token.Intento >= maxIntentos)
                token.Estado = TokenType.Excedido.Value;
            _context.Update(token);
            _context.SaveChanges();
            return token.Intento.Value;
        }

        public bool AnyTokenCode(string nombreContexto, string tokenCode)
        {
            return _context.Token.Any(x => x.NombreContexto == nombreContexto
                                       && x.NroToken == tokenCode
                                       && x.Estado == TokenType.Generado.Value);
        }




        public int EvaluarRepeticionesConfiguracion(Token token)
        {
            return _context.Token.Count(x => x.NombreContexto == token.NombreContexto
             && x.Telefono == token.Telefono
             && x.Estado == "Generado");
        }

        public void ActualizarEstadoToken(Token token)
        {
            var query = (from x in _context.Token
                         where x.NombreContexto == token.NombreContexto
                         && x.Telefono == token.Telefono
                         && x.Estado == "Generado"
                         select x).ToList();

            foreach (var item in query)
            {
                item.Estado = "No Usado";
                item.DetalleEstado = "Se generó nuevo Token";

                _context.Update(item);
                _context.SaveChanges();

            }


        }



        public bool GatillaServiciosSMS(Token token)
        {
            return _context.Contexto.Any(x => x.Nombre == token.NombreContexto
            && x.EnvioNotificacion == true
            && x.TipoNotificacion == "sms"
            );
        }


        public async Task<EnvioSMSResponse> EnviarTokenServicioSMS(EnvioSMSRequest request)
        {
            EnvioSMSResponse respuesta;

            var uri = "https://api.us.apiconnect.ibmcloud.com/telefonica-del-peru-production/ter/ws-notificationManagement/notifyAllSubscribers";
            var json = JsonConvert.SerializeObject(request);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("X-IBM-Client-Id", "486c5aa3-d9a7-47e1-b39e-bbbff1c1069c");
            client.DefaultRequestHeaders.Add("X-IBM-Client-Secret", "hW8gL3cJ4pG4cA3nJ6cX2cW6yV7lB5aG8eW5oU5vN6hL5gL5wN");

            var response = await client.PostAsync(uri, stringContent);
                                   
            string response2 = await response.Content.ReadAsStringAsync();
            respuesta = JsonConvert.DeserializeObject<EnvioSMSResponse>(response2);
            return respuesta;

        }

        public bool ValidarEnvioEventNotification(Token token)
        {
            return _context.Contexto.Any(x => x.Nombre == token.NombreContexto
             && x.EnvioEvent == true
            );
        }

        public void EnviarEventNotification(Token token, DateTime? fechaEnvioNotificacion)
        {
            var NumDoc = "";
            var TipoDoc = "";
            if (token.NumeroDoc == "" || token.TipoDoc =="")
            {
                TipoDoc = "DNI";
                NumDoc = token.DNI;
            }
            else {
                NumDoc = token.NumeroDoc;
                TipoDoc = token.TipoDoc;
            }

            var fechaEnvioNotificacionDesfase = fechaEnvioNotificacion.ToString();

            if (fechaEnvioNotificacionDesfase != null)
            {
                fechaEnvioNotificacionDesfase = Convert.ToDateTime(fechaEnvioNotificacion).AddHours(-5).ToString("yyyy-MM-ddTHH:mm:ss.fffffffK");
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
                                        Value = Convert.ToString(token.IdTransaccion)
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
                                        Value = Convert.ToString(token.Telefono)
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
                                        Value = token.NroToken
                                    }
                                },
                                 new EnvioEventNotificationRequest.additionalData_
                                {
                                    KeyValueType = new EnvioEventNotificationRequest.KeyValueType_
                                    {
                                        Key = "fechaGeneracionToken",
                                        Value = Convert.ToDateTime(token.FechaGeneracion).AddHours(-5).ToString("yyyy-MM-ddTHH:mm:ss.fffffffK")
                                    }
                                },
                                 new EnvioEventNotificationRequest.additionalData_
                                {
                                    KeyValueType = new EnvioEventNotificationRequest.KeyValueType_
                                    {
                                        Key = "fechaEnvioToken",
                                        Value = fechaEnvioNotificacionDesfase
                                    }
                                },
                                 new EnvioEventNotificationRequest.additionalData_
                                {
                                    KeyValueType = new EnvioEventNotificationRequest.KeyValueType_
                                    {
                                        Key = "fechaValidacionToken",
                                        Value = DateTime.Now.AddHours(-5).ToString("yyyy-MM-ddTHH:mm:ss.fffffffK")
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
                                        Value = "Validado"
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

            EnvioEventNotificationResponse rpta = EnviarTokenServicioEventNotification(request).Result;
            if (rpta.code == "201")
            {

                var query = (from x in _context.Token
                             where x.NumeroDoc == token.NumeroDoc
                             && x.Telefono == token.Telefono
                             && x.NombreContexto == token.NombreContexto
                             select x).ToList();

                foreach (var item in query)
                {
                    var query2 = (from x in _context.DetalleToken
                                 where x.IdToken == item.IdToken
                                 && x.EstadoEvent == false
                                 select x).ToList();

                    foreach (var item2 in query2) 
                    {
                        item2.EstadoEvent = true;
                        item2.TokenValidado = token.NroToken;
                        _context.Update(item2);
                        _context.SaveChanges();
                    }

                }


            var detalletoken = (from x in _context.DetalleToken
                               where x.IdToken == token.IdToken
                              select x).ToList();

            var current = DateTime.Now;

            foreach (var dt in detalletoken)
            {
                dt.IdToken = token.IdToken;
                dt.CodigoRespuestaEvent = rpta.code;
                dt.MensajeRespuestaEvent = rpta.description;
                dt.NumeroReintentosEvent = 1;
                dt.FechaEnvioEvent = current;
                dt.OrigenEnvioEvent = "online";
                dt.EstadoEvent = true;
                dt.TokenValidado = token.NroToken;
                    //dt.EstadoEvent = "Enviado";
                    _context.Update(dt);
                _context.SaveChanges();
            }
    
            } 
            /*Si rpta true, se actualizan todos los valores de estadoEnvio en detalleToken a enviado 
            Para todos los usuarios que tengan el mismo celular y el mismo nro documento y asi no se 
            consideren en el proceso offline.*/
        }

        public async Task<EnvioEventNotificationResponse> EnviarTokenServicioEventNotification(EnvioEventNotificationRequest request)
        {
            EnvioEventNotificationResponse respuesta;

            var uri = "https://apisd.telefonica.com.pe/vp-tecnologia/bss/eventnotification/eventNotification/v2/EVENT";
            var json = JsonConvert.SerializeObject(request);


            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            Console.WriteLine("JSON ONLINE:" + json.ToString());

            var client = new HttpClient();

            var _accessToken = "";
            var tokenAuth = (from x in _context.TokenAuth
                          select x).ToList();

            foreach (var item in tokenAuth)
            {
                _accessToken = item.AccessToken;
                
            }
            

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            client.DefaultRequestHeaders.Add("X-IBM-Client-Id", "3d980602-0c5f-4ec7-ac3f-d51ce62ff476");
            //añadir generacion de token oAuth

            var response = await client.PostAsync(uri, stringContent);

            string response2 = await response.Content.ReadAsStringAsync();
            respuesta = JsonConvert.DeserializeObject<EnvioEventNotificationResponse>(response2);
            return respuesta;

        }

        public void IniciarDepuracion()
        {

           /* var query = (from x in _context.Token
                         where x.IdToken == 494
                         select x).ToList();

            foreach (var item in query)
            {
                item.Estado = "Validado";


                _context.Update(item);
                _context.SaveChanges();

            }*/


            /* var c = _context.TokenDepuracion;
             var query = (from x in _context.TokenDepuracion
                          where x.Estado == "activado"
                          select x).ToList();



             var FechaHoy = DateTime.Now;

             foreach (var item in query)
            {                        */
            /*  if (item.FechaEjecucion == FechaHoy) {



                  _context.Update(item);
                  _context.SaveChanges();
              }*/

            /*  if (item.Estado == "activado")
              {
                  item.Estado = "desactivado";
              }
              else if (item.Estado == "desactivado") 
              {
                  item.Estado = "activado";
              }

              _context.Update(item);
              _context.SaveChanges();

          }*/


        }

    }
}
