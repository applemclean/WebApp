using DatabaseService.Models;
using DatabaseService.Repositories;
using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using Helpers;
using DatabaseService.RequestValidation;
using DatabaseService.Presenters;
using DatabaseService.Contexts;
using System.Net;
using System.ServiceModel.Channels;
using System.Linq;
using System.ServiceModel.Activation;

namespace DatabaseService.Services
{
    /// <summary>
    /// Users endpoint implementation
    /// </summary>
    [ServiceContract]
    [ServiceBehavior(
      InstanceContextMode = InstanceContextMode.PerCall
    )]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class UserService : IDisposable
    {
        /// <summary>
        /// Optional override for the connection string value.
        /// </summary>
        public static string ConnectionString { get; set; } = null;

        public static IAntiForgeryValidator RequestValidator { get; set; } = null;

        private readonly UsersContext _usersContext;
        private readonly UserRepository _userRepository;

        private const int LIMIT_MAX = 100;

        private bool _isDisposed;

        /// <summary>
        /// Create User endpoint handler.
        /// </summary>
        public UserService()
        {
            if(ConnectionString == null)
            {
                _usersContext = new UsersContext();
            }
            else
            {
                _usersContext = new UsersContext(ConnectionString);
            }

            if(RequestValidator == null)
            {
                RequestValidator = new AntiForgeryValidator();
            }

            _userRepository = new UserRepository(_usersContext);
        }

        public void Dispose()
        {
            this.StandardDispose(ref _isDisposed, _usersContext.Dispose);
        }

        ~UserService()
        {
            Dispose();
            this.WarnAboutUsingDispose();
        }

        /// <summary>
        /// Read a list of users.
        /// </summary>
        /// <param name="offset">The offset of a list</param>
        /// <param name="limit">The limit</param>
        /// <returns></returns>
        [WebInvoke(Method = "GET", UriTemplate = "users?offset={offset}&limit={limit}",
                   ResponseFormat=WebMessageFormat.Json)]
        [OperationContract]
        public UsersListPresenter GetUsers(string offset, string limit)
        {
            this.GuardNotDisposed(_isDisposed);

            if(string.IsNullOrEmpty(offset) || string.IsNullOrEmpty(limit))
            {
                throw new WebFaultException(HttpStatusCode.NotFound);
            }

            var iOffset = Int32.Parse(offset);
            var iLimit = Int32.Parse(limit);

            if (iOffset < 0 || iLimit < 0 || iLimit > LIMIT_MAX)
            {
                throw new WebFaultException(HttpStatusCode.NotFound);
            }

            return new UsersListPresenter(
                _userRepository.GetCount(),
                _userRepository.ReadList(iOffset, iLimit)
            );
        }

        /// <summary>
        /// Read a user by its identifier.
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>User record</returns>
        [WebInvoke(Method = "GET", UriTemplate = "users/{id}",
                   ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        public UserPresenter GetUser(string id)
        {
            this.GuardNotDisposed(_isDisposed);

            ValidateCSRF();

            var iId = Int32.Parse(id);

            var user = _userRepository.Read(iId);

            if(user == default(User))
            {
                throw new WebFaultException(HttpStatusCode.NotFound);
            }

            return new UserPresenter(user);
        }

        /// <summary>
        /// Delete a user by its identifier.
        /// </summary>
        /// <param name="id">User ID</param>
        [WebInvoke(Method = "DELETE", UriTemplate = "users/{id}")]
        [OperationContract]
        public void DeleteUser(string id)
        {
            this.GuardNotDisposed(_isDisposed);

            ValidateCSRF();

            var iId = Int32.Parse(id);

            if(!_userRepository.Delete(iId))
            {
                throw new WebFaultException(HttpStatusCode.NotFound);
            }
        }

        /// <summary>
        /// Create a user record.
        /// </summary>
        /// <param name="userData">User data to be saved</param>
        /// <returns>Created user record</returns>
        [WebInvoke(Method = "POST", UriTemplate = "users", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public UserPresenter CreateUser(UserPresenter userData)
        {
            this.GuardNotDisposed(_isDisposed);

            ValidateCSRF();

            try
            {
                return new UserPresenter(_userRepository.Create(userData));
            }
            catch(ValidationFailedException failedValidation)
            {
                throw new WebFaultException<ServiceFault>(FormatServiceFault(failedValidation), HttpStatusCode.BadRequest);
            }
        }

        /// <summary>
        /// Update a record of a user.
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="userData">User data to be saved</param>
        [WebInvoke(Method = "PUT", UriTemplate = "users/{id}", RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public void UpdateUser(string id, UserPresenter userData)
        {
            this.GuardNotDisposed(_isDisposed);

            ValidateCSRF();

            var iId = Int32.Parse(id);

            try
            {
                _userRepository.Update(iId, userData);
            }
            catch (ValidationFailedException failedValidation)
            {
                throw new WebFaultException<ServiceFault>(FormatServiceFault(failedValidation), HttpStatusCode.BadRequest);
            }
        }

        /// <summary>
        /// Format service fault to be represented as <c>ServiceFault</c>
        /// </summary>
        /// <param name="failedValidation">Failed validation</param>
        /// <returns><c>ServiceFault</c></returns>
        private ServiceFault FormatServiceFault(ValidationFailedException failedValidation)
        {
            var validationErrors = from x in failedValidation.FailedValidations select x.ValidationErrors;

            var validationErrorsFlatten = validationErrors.SelectMany(x => x);

            var errors = from x in validationErrorsFlatten select x.ErrorMessage;

            var errorsCombined = String.Join(Environment.NewLine, errors);

            return new ServiceFault { Error = errorsCombined };
        }

        /// <summary>
        /// Validate request against CSRF vulnerability.
        /// </summary>
        private void ValidateCSRF()
        {
            var currentContext = OperationContext.Current;
            var request = (HttpRequestMessageProperty) currentContext.IncomingMessageProperties[HttpRequestMessageProperty.Name];

            if (!RequestValidator.Validate(request))
            {
                throw new WebFaultException(HttpStatusCode.Forbidden);
            }
        }
    }
}
