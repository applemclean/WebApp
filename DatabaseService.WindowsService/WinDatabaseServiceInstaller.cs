using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Web;

namespace DatabaseService
{
    /// <summary>
    /// Windows service installer.
    /// </summary>
    [RunInstaller(true)]
    public class WinDatabaseServiceInstaller : Installer
    {
        private readonly ServiceProcessInstaller process;
        private readonly ServiceInstaller service;

        public WinDatabaseServiceInstaller()
        {
            process = new ServiceProcessInstaller();
            process.Account = ServiceAccount.LocalSystem;
            service = new ServiceInstaller();
            service.ServiceName = WinDatabaseService.Name;
            Installers.Add(process);
            Installers.Add(service);
        }
    }
}