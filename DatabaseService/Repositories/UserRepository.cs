using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DatabaseService.Models;
using Helpers;
using DatabaseService.Contexts;
using System.Data.Entity.Infrastructure;
using DatabaseService.Presenters;

namespace DatabaseService.Repositories
{
    /// <summary>
    /// Repository for the purpose of operating on a context having users
    /// </summary>
    internal class UserRepository
    {
        private readonly UsersContext _usersContext;

        /// <summary>
        /// Create a User repository.
        /// </summary>
        /// <param name="context">The context</param>
        public UserRepository(UsersContext context)
        {
            if(context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            _usersContext = context;
        }

        /// <summary>
        /// Read a user record.
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>The user being read</returns>
        public User Read(int id)
        {
            return _usersContext.Users.SingleOrDefault(x => x.Id == id);
        }

        /// <summary>
        /// Delete a user record.
        /// </summary>
        /// <param name="id">User ID</param>
        public bool Delete(int id)
        {
            var userToBeRemoved = Read(id);
            if (userToBeRemoved != default(User))
            {
                _usersContext.Users.Remove(userToBeRemoved);

                if(_usersContext.SaveChanges() != 1)
                {
                    return false;
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the number of users in a database.
        /// </summary>
        /// <returns>The number of users in a database.</returns>
        public int GetCount()
        {
            return _usersContext.Users.Count();
        }

        /// <summary>
        /// Read a list of user records.
        /// </summary>
        /// <param name="offset">Offset</param>
        /// <param name="limit">The limit of users</param>
        /// <returns></returns>
        public IList<User> ReadList(int offset, int limit)
        {
            return _usersContext.Users.OrderBy(x => x.Id)
                                .Skip(offset).Take(limit).ToList();
        }

        /// <summary>
        /// Create a user record.
        /// </summary>
        /// <param name="userData">User data</param>
        /// <returns>New user record</returns>
        public User Create(UserPresenter userData)
        {
            var newUser = _usersContext.Users.Add(new User
            {
                EMail = userData.EMail,
                Name = userData.Name,
                Surname = userData.Surname
            });

            var validationErrros = _usersContext.GetValidationErrors();

            if(validationErrros.Count() > 0)
            {
                throw new ValidationFailedException(_usersContext.GetValidationErrors());
            }

            _usersContext.SaveChanges();

            return newUser;
        }

        /// <summary>
        /// Update a user.
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="userData">User data to be updated</param>
        public void Update(int id, UserPresenter userData)
        {
            var userToBeUpdated = _usersContext.Users.Single(x => x.Id == id);
            userToBeUpdated.EMail = userData.EMail;
            userToBeUpdated.Name = userData.Name;
            userToBeUpdated.Surname = userData.Surname;

            var validationErrros = _usersContext.GetValidationErrors();

            if (validationErrros.Count() > 0)
            {
                throw new ValidationFailedException(_usersContext.GetValidationErrors());
            }

            _usersContext.SaveChanges();
        }
    }
}