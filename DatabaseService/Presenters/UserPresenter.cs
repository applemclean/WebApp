using DatabaseService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DatabaseService.Presenters
{
    /// <summary>
    /// Single user.
    /// </summary>
    [DataContract]
    public struct UserPresenter
    {
        [DataMember]
        public int? Id { get; private set; }

        [DataMember]
        public string Name { get; private set; }

        [DataMember]
        public string EMail { get; private set; }

        [DataMember]
        public string Surname { get; private set; }

        public UserPresenter(User user)
        {
            if(user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            Id = user.Id;
            EMail = user.EMail;
            Name = user.Name;
            Surname = user.Surname;
        }
    }
}