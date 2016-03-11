using DatabaseService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using System.Web;
using System.Diagnostics;
using System.ComponentModel;

namespace DatabaseService
{
    /// <summary>
    /// DatabaseService as windows service.
    /// </summary>
    public class WinDatabaseService : ServiceBase
    {
        internal static string Name { get; } = nameof(WinDatabaseService); 

        private ServiceHost _serviceHost;

        /// <summary>
        /// Create database service.
        /// </summary>
        public WinDatabaseService()
        {
            ServiceName = Name;
        }

        /// <summary>
        /// Service entry point.
        /// </summary>
        public static void Main()
        {
//#if DEBUG
//            var host = new ServiceHost(typeof(UserService));
//
//            host.Open();
//
//            Console.ReadKey();
//#else
            ServiceBase.Run(new WinDatabaseService());
//#endif
        }

        /// <summary>
        /// Starting the service.
        /// </summary>
        /// <param name="args">Service arguments</param>
        protected override void OnStart(string[] args)
        {
            base.OnStart(args);

            _serviceHost?.Close();

            _serviceHost = new ServiceHost(typeof(UserService));

            _serviceHost.Open();
        }

        /// <summary>
        /// Stopping the service.
        /// </summary>
        protected override void OnStop()
        {
            base.OnStop();

            if (_serviceHost != null)
            {
                _serviceHost.Close();
                _serviceHost = null;
            }
        }
    }
}