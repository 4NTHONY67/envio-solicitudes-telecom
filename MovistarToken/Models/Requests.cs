using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovistarToken.Models
{
    public class TokenField
    {
        public string Llave { get; set; }
        public string Valor { get; set; }
    }
    public class TokenRequest : ITokenRequest
    {
       public string Contexto { get; set; }
       public List<TokenField> Campos { get; set; }
       public int Telefono { get; set; }
       public string IdTransaccion { get; set; }
       public string TipoDoc { get; set; }
       public string NumeroDoc { get; set; }

    }

    public class ValidTokenRequest : ITokenRequest
    {
        public string Contexto { get; set; }
        public List<TokenField> Campos { get; set; }
        public string Token { get; set; }
        public int Telefono { get; set; }
        public string IdTransaccion { get; set; }
        public string TipoDoc { get; set; }
        public string NumeroDoc { get; set; }


    }

    public interface ITokenRequest
    {
       string Contexto { get; set; }
       List<TokenField> Campos { get; set; }
       int Telefono { get; set; }
       string IdTransaccion { get; set; }
       string TipoDoc { get; set; }
       string NumeroDoc { get; set; }


    }



    public class EnvioSMSRequest
    {
        public TefHeaderReq_ TefHeaderReq { get; set; }
        public NotifyAllSubscribersRequestData_ NotifyAllSubscribersRequestData { get; set; }

        public EnvioSMSRequest() {
            TefHeaderReq = new TefHeaderReq_();
            NotifyAllSubscribersRequestData = new NotifyAllSubscribersRequestData_();
        }

        public class TefHeaderReq_
        {
            public string userLogin { get; set; }
            public string serviceChannel { get; set; }
            public string application { get; set; }
            public string idMessage { get; set; }
            public string ipAddress { get; set; }
            public string functionalityCode { get; set; }
            public string transactionTimestamp { get; set; }
            public string serviceName { get; set; }
            public string version { get; set; }

        }
        public class NotifyAllSubscribersRequestData_
        {
            public string mediaType { get; set; }
          //  public List<notifyData_> notifyDataList { get; set; }
            public notifyDataList_ notifyDataList { get; set; }


            public NotifyAllSubscribersRequestData_()
            {
                notifyDataList = new notifyDataList_();
                
            }

        }


        public class notifyDataList_{

            public List<notifyData_> notifyData { get; set; }

            public notifyDataList_() {
                this.notifyData = new List<notifyData_>();
            }

        }

        public class templateParameter_
        {
            public string key { get; set; }
            public string value { get; set; }

        }

        public class templateId_
        {
            public string templateId { get; set; }

        }

        public class smsData_
        {

            public msisdnList_ msisdnList { get; set; }

        }

        public class msisdnList_ {

            public List<msisdn_> msisdn { get; set; }

        }

        public class msisdn_
        {
            public int number { get; set; }

        }

        public class notifyData_
        {

            public smsData_ smsData { get; set; }
            public templateId_ templateId { get; set; }
            //public List<templateParameter_> templateParameterList { get; set; }
            public templateParameterList_ templateParameterList { get; set; }

        }

        public class templateParameterList_
        {
            public List<templateParameter_> templateParameter { get; set; }
        }

    }


    public class EnvioEventNotificationRequest
    {

        public string eventType { get; set; }
        public string eventTime { get; set; }
        public string eventId { get; set; }
        public string eventSource { get; set; }
        public List<relatedEntity_> relatedEntity { get; set; }

        [JsonProperty("event")]
        public event_ Event { get; set; }

        public EnvioEventNotificationRequest()
        {
            relatedEntity = new List<relatedEntity_>();
            Event = new event_();

        }


        public class relatedEntity_
        {

            public string entityType { get; set; }
            public string id { get; set; }
            public string href { get; set; }

        }

        public class event_
        {
            [JsonProperty("@type")]
            public string @type { get; set; }

            [JsonProperty("@schemaLocation")]
            public List<schemaLocation_> @schemaLocation { get; set; }

            public event_()
            {
                this.@schemaLocation = new List<schemaLocation_>();
            }

        }

        public class schemaLocation_
        {

            public dinamicNotification_ dinamicNotification { get; set; }

            public schemaLocation_()
            {
                dinamicNotification = new dinamicNotification_();
            }

        }

        public class dinamicNotification_
        {

            public List<dinamicEntity_> dinamicEntity { get; set; }

        }

        public class dinamicEntity_
        {

            public string entityType { get; set; }
            public List<additionalData_> additionalData { get; set; }
        }

        public class additionalData_
        {

            public KeyValueType_ KeyValueType { get; set; }

        }

        public class KeyValueType_
        {

            public string Key { get; set; }
            public string Value { get; set; }

        }

    }

}
