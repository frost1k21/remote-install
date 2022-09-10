using System;
using System.IO;
using System.Management;
using System.Net.NetworkInformation;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using RemoteInstall.ConfigFromFile.Models;

namespace RemoteInstall.EditIbasesConfig
{
    public class OneCSettingsBases
    {
        #region Private Field(s)

        private string userName;
        private string appDataPath;
        private string osNameField;
        private bool isLoggedIn;

        #endregion

        #region Private Method(s)

        private async Task<bool> CheckWsConnection(string wsName)
        {
            try
            {
                var ping = new Ping();
                var pingReplay = await ping.SendPingAsync(wsName, 1000);
                return (pingReplay.Status == IPStatus.Success) ? true : false;
            }
            catch
            {
                return false;
            }

        }

        private void RemoteRegistryService(string wsName)
        {
            ManagementObject serviceObject = new ManagementObject($"\\\\{wsName}\\root\\cimv2:Win32_Service.Name='RemoteRegistry'");
            serviceObject.Get();
            if (serviceObject["StartMode"].ToString().ToLower() == "disabled")
            {
                var inParams = serviceObject.GetMethodParameters("ChangeStartMode");
                inParams["StartMode"] = "Automatic";
                serviceObject.InvokeMethod("ChangeStartMode", inParams, null);
                serviceObject.Get();
            }

            if (serviceObject["State"].ToString().ToLower() != "running")
            {
                serviceObject.InvokeMethod("StartService", null);
                serviceObject.Get();
            }
        }

        private void GetCurrentUser(string wsName)
        {
            var user = "";
            ManagementScope scope = new ManagementScope($"\\\\{wsName}\\root\\cimv2");
            scope.Connect();

            ManagementClass currentUser = new ManagementClass(scope, new ManagementPath("Win32_ComputerSystem"), null);
            foreach (ManagementBaseObject instance in currentUser.GetInstances())
            {
                user = instance["UserName"]?.ToString();
            }

            if (string.IsNullOrEmpty(user))
            {
                isLoggedIn = false;
                GetLastLogonUserName(wsName, scope);
            }
            else
            {
                isLoggedIn = true;
                userName = user;
            }
        }

        private void GetLastLogonUserName(string wsName, ManagementScope managementScope)
        {
            string osName = "";
            string user = "";
            managementScope.Connect();
            ManagementClass osNameClass = new ManagementClass(managementScope, new ManagementPath("Win32_OperatingSystem"), null);
            foreach (ManagementBaseObject instance in osNameClass.GetInstances())
            {
                osName = instance["Caption"]?.ToString();
                osNameField = osName;
            }

            if (osName == "Microsoft Windows XP Professional")
            {
                var regEntry = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, wsName)
                    .OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon");
                foreach (string valueName in regEntry.GetValueNames())
                {
                    if (valueName.ToLower() == "defaultusername")
                        user = $"CHEAZ\\{regEntry.GetValue(valueName).ToString()}";
                }
            }
            else
            {
                var regEntry = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, wsName, RegistryView.Registry64)
                    .OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Authentication\LogonUI");
                foreach (string valueName in regEntry.GetValueNames())
                {
                    if (valueName.ToLower() == "lastloggedonuser")
                        user = regEntry.GetValue(valueName).ToString();
                }
            }
            userName = user;
        }

        private string NameToSid(string name)
        {
            NTAccount f = new NTAccount(name);
            SecurityIdentifier s = (SecurityIdentifier)f.Translate(typeof(SecurityIdentifier));
            return s.ToString();
        }


        private void GetAppDataPathWithRemoteRegestry(string wsName)
        {
            var pathFolder = "";
            if (isLoggedIn)
            {
                var x = NameToSid(userName);
                var test = RegistryKey.OpenRemoteBaseKey(RegistryHive.Users, wsName)
                    .OpenSubKey(x + @"\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders");

                foreach (string valueName in test.GetValueNames())
                {
                    if (valueName.ToLower() == "appdata")
                        pathFolder = test.GetValue(valueName).ToString();
                }

                appDataPath = pathFolder;
            }
            else
            {
                var lastLogUser = userName.Split('\\');
                if (osNameField == "Microsoft Windows XP Professional")
                {
                    pathFolder = $@"C:\Documents and Settings\{lastLogUser[1]}\Application Data";
                }
                else
                {
                    pathFolder = $@"C:\Users\{lastLogUser[1]}\AppData\Roaming";
                }

                appDataPath = pathFolder;
            }
            
        }

        private void CopyBasesSettings(string wsName, OneCBases oneCBases)
        {
            var path = appDataPath.Replace(':','$');
            var fullPath = $"\\\\{wsName}\\{path}\\1C\\1CEStart";
            var filePath = $"{fullPath}\\ibases.v8i";
            if (!File.Exists(filePath))
            {
                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                }
                using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    sw.WriteLine($"[{oneCBases.NameForUser}]");
                    sw.WriteLine($"Connect=Srvr=\"{oneCBases.ServerName}\";Ref=\"{oneCBases.DbName}\";");
                }
            }
            using (StreamWriter sw = new StreamWriter(filePath, true, Encoding.UTF8))
            {
                sw.WriteLine($"[{oneCBases.NameForUser}]");
                sw.WriteLine($"Connect=Srvr=\"{oneCBases.ServerName}\";Ref=\"{oneCBases.DbName}\";");
            }
        }

        #endregion

        #region Public Property(ies)

        public string UserName => userName;
        public string AppDataPath => appDataPath;

        #endregion

        #region Public Method(s)

        public void UpdateIBases(string wsName, OneCBases oneCBases)
        {
            RemoteRegistryService(wsName);
            GetCurrentUser(wsName);
            GetAppDataPathWithRemoteRegestry(wsName);
            CopyBasesSettings(wsName, oneCBases);
        }

        public async Task<string> UpdateIBasesWithResult(string wsName, OneCBases oneCBases)
        {
            var result = await CheckWsConnection(wsName);
            if (!result)
                return $"Компьютер {wsName.ToUpper()} не в сети";
            try
            {
                #region RemoteRegistryService
                try
                {
                    RemoteRegistryService(wsName);

                }
                catch
                {
                    return $"Не удалось запустить службу удаленного реестра на компьютере {wsName.ToUpper()}";
                }
                #endregion //RemoteRegistryService

                #region GetCurrentUser
                try
                {
                    GetCurrentUser(wsName);
                    if (userName.ToLower().StartsWith("ws"))
                        return $"Локальный пользователь на компьютере {wsName.ToUpper()}";
                }
                catch
                {
                    return $"Ошибка при определении пользователя на компьютере {wsName.ToUpper()}";
                }
                #endregion //GetCurrentUser

                #region GetAppDataPathWithRemoteRegestry

                try
                {
                    GetAppDataPathWithRemoteRegestry(wsName);
                }
                catch
                {
                    return $"Ошибка определения пути AppData на компьютере {wsName.ToUpper()}";
                }


                #endregion //GetAppDataPathWithRemoteRegestry

                #region CopyBasesSettings

                try
                {
                    CopyBasesSettings(wsName, oneCBases);
                }
                catch
                {
                    return $"Ошибка при настройки баз на компьютере {wsName.ToUpper()}";
                }

                #endregion //CopyBasesSettings

                return $"Настройка баз на компьютере {wsName.ToUpper()} выполнена успешно";
            }
            catch
            {
                return $"Непредвиденная ошибка при настройке баз на компьютере {wsName.ToUpper()}";
            }
            
        }

        #endregion
    }
}
