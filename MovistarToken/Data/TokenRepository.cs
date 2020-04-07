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

            var token = new Token
            {
                Campos = txtCampos,
                Estado = TokenType.Generado.Value,
                FechaGeneracion = DateTime.Now,
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
                //preparar mensaje envio servicio
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


                //enviar token a servicio SMS
                EnvioSMSResponse rpta = EnviarTokenServicioSMS(request).Result;

                DetalleToken detalletoken;

                if (rpta.notifyAllSubscribersResponseData.status.code == 1)
                {

                    detalletoken = new DetalleToken
                    {
                        IdToken = tokenId,
                        FechaEnvioNotificacion = DateTime.Now,
                        FechaRespuestaNotificacion = DateTime.Now,
                        CodigoNotificacion = "",
                        MensajeNotificacion = "Se envio correctamente a servicio SMS",
                        OrigenNotificacion = "",
                        FechaRespuestaServicioToken = DateTime.Now



                    };
                }
                else
                {
                    detalletoken = new DetalleToken
                    {
                        IdToken = tokenId,
                        FechaEnvioNotificacion = DateTime.Now,
                        FechaRespuestaNotificacion = DateTime.Now,
                        CodigoNotificacion = "",
                        MensajeNotificacion = "No se pudo enviar",
                        OrigenNotificacion = "",
                        FechaRespuestaServicioToken = DateTime.Now
                    };

                }
                _context.DetalleToken.Add(detalletoken);
                _context.SaveChanges();

            }








        }

        public void UpdateToken(Token token, TokenType tokenType)
        {
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



    }
}
