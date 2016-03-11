using DatabaseService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DatabaseService.Presenters
{
    /// <summary>
    /// Users list presenter.
    /// </summary>
    [DataContract]
    public struct UsersListPresenter
    {
        [DataMember]
        public int Count { get; private set; }

        [DataMember]
        public IList<UserPresenter> Users { get; private set; }

        public UsersListPresenter(int count, IList<User> users)
        {
            if(users == null)
            {
                throw new ArgumentNullException(nameof(users));
            }

            Count = count;
            Users = (from x in users select new UserPresenter(x)).ToList();
        }
    }
}