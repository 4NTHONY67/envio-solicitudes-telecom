using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovistarToken.Models
{
    public class TokenResponse
    {
        public string Token { get; set; }
    }

    public class ValidTokenResponse
    {
        public string ResultCode { get; set; }
    }

    public class ErrorResponse
    {
        public string ResultCode { get; set; }
        public string ResultDesc { get; set; }
    }



    public class EnvioSMSResponse
    {
        public TefHeaderRes_ TefHeaderRes { get; set; }
        public notifyAllSubscribersResponseData_ notifyAllSubscribersResponseData { get; set; }

        public class TefHeaderRes_
        {
            public string idMessage { get; set; }
            public string serviceName { get; set; }
            public string responseDateTime { get; set; }

        }
        public class notifyAllSubscribersResponseData_
        {
            public status_ status { get; set; }

        }
        public class status_
        {
            public int code { get; set; }
            public string description { get; set; }
        }
    }


}
