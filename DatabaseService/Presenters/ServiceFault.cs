using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DatabaseService.Presenters
{
    [DataContract]
    public class ServiceFault
    {
        [DataMember]
        public string Error { get; set; }
    }
}