using DatabaseService.Presenters;
using DatabaseService.RequestValidation;
using DatabaseService.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace DatabaseService.Tests.Services
{
    /// <summary>
    /// Due to sequrity limitations, http://go.microsoft.com/fwlink/?LinkId=70353
    /// Port 8090 should be open for WCF service to be started using user privileges
    /// 
    /// Example:
    /// netsh http add urlacl url=http://+:8090/ user=YOUR_USERNAME
    /// </summary>
    [TestClass]
    public class UserServiceTest
    {
        private HttpClient _client;

        private const string _serverAddress = "http://localhost:8090/";

        private WebServiceHost _testHost;

        private JavaScriptSerializer _serializer;

        private class UserResponse
        {
            public int Id = 0;
            public string Name = null;
        }

        private class FakeCSRFValidator : IAntiForgeryValidator
        {
            public bool Validate(HttpRequestMessageProperty request)
            {
                return true;
            }
        }

        [TestInitialize]
        public void Setup()
        {
            var address = new Uri(_serverAddress);

            UserService.ConnectionString = @"Data Source=localhost\SQLEXPRESS;Database=WebApiTest;Trusted_Connection=True;MultipleActiveResultSets=True";

            UserService.RequestValidator = new FakeCSRFValidator();

            using (var c = new DbContext(UserService.ConnectionString))
            {
                c.Database.ExecuteSqlCommand("DELETE FROM dbo.Users");
            }

            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler { CookieContainer = cookieContainer };

            cookieContainer.Add(
                new Cookie(AntiForgeryValidator.TokenName, "CSRF", "/")
                {
                    Port = $"\"{address.Port}\"",
                    Domain = address.Host
                }
            );

            _client = new HttpClient(handler, true);
            _client.DefaultRequestHeaders.Add(AntiForgeryValidator.TokenName, "CSRF");

            _serializer = new JavaScriptSerializer();

            _testHost = new WebServiceHost(typeof(UserService), address);

            ServiceDebugBehavior sdb = _testHost.Description.Behaviors.Find<ServiceDebugBehavior>();
            sdb.IncludeExceptionDetailInFaults = true;

            _testHost.Open();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _testHost.Close();
            _client.Dispose();
        }

        [TestMethod]
        public async Task CanGetListOfUsers()
        {
            var offset = 0;
            var limit = 100;
            var response =
                await _client.GetAsync($"{_serverAddress}users?offset={offset}&limit={limit}");
            Assert.IsTrue(response.IsSuccessStatusCode);
        }

        [TestMethod]
        public async Task CanUpdateOneUser()
        {
            var newUser = await CreateUser("One", "mail3@example.com");

            var userContent = new StringContent("{ \"Name\": \"Two\", \"Surname\": \"y\", \"EMail\": \"mail@example.com\" }", Encoding.UTF8, "application/json");
            var response = await _client.PutAsync($"{_serverAddress}users/{newUser.Id}", userContent);
            Assert.IsTrue(response.IsSuccessStatusCode);

            var sameUser = await GetUser(newUser.Id);

            Assert.AreEqual("Two", sameUser.Name);
        }

        [TestMethod]
        public async Task CanDeleteOneUser()
        {
            var newUser = await CreateUser("One", "mail2@example.com");

            var response = await _client.DeleteAsync($"{_serverAddress}users/{newUser.Id}");
            Assert.IsTrue(response.IsSuccessStatusCode);

            var secondResponse = await _client.DeleteAsync($"{_serverAddress}users/{newUser.Id}");
            Assert.AreEqual(HttpStatusCode.NotFound, secondResponse.StatusCode);
        }

        [TestMethod]
        public async Task CanCreateUser()
        {
            var user = await CreateUser("Test", "mail@example.com");
            var userGet = await GetUser(user.Id);
            Assert.AreEqual("Test", userGet.Name);
        }

        /// <summary>
        /// Make api call and create one user.
        /// </summary>
        /// <param name="userName">User name</param>
        /// <returns>Create response</returns>
        private async Task<UserResponse> CreateUser(string userName, string eMail)
        {
            var userContent = new StringContent($"{{ \"Name\": \"{userName}\", \"Surname\": \"y\", \"EMail\": \"{eMail}\" }}", Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"{_serverAddress}users", userContent);

            Assert.IsTrue(response.IsSuccessStatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();

            return _serializer.Deserialize<UserResponse>(responseBody);
        }

        /// <summary>
        /// Make api call and get one user.
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns></returns>
        private async Task<UserResponse> GetUser(int id)
        {
            var response = await _client.GetAsync($"{_serverAddress}users/{id}");

            Assert.IsTrue(response.IsSuccessStatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();
            return _serializer.Deserialize<UserResponse>(responseBody);
        }
    }
}
