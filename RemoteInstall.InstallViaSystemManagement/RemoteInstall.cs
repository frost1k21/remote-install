using System;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using RemoteInstall.ConfigFromFile.Models;

namespace RemoteInstall.InstallViaSystemManagement
{
    public class RemoteInstaller
    {
        public async Task<string> InstallMsiAsync(string wsName, InstallSettings installSettings, SecretCred secretCred, bool tryAgain)
        {
            var isInNetwork = await CheckWsConnection(wsName);

            if (!isInNetwork)
                return $"Компьютер {wsName.ToUpper()} не в сети";

            try
            {
                ConnectionOptions options = new ConnectionOptions();
                ManagementScope scope = new ManagementScope($"\\\\{wsName}\\root\\cimv2", options);
                scope.Connect();

                await WindowsInstallerService(wsName);

                ManagementClass netUseCredClass = new ManagementClass(scope, new ManagementPath("Win32_Process"), null);
                ManagementClass installMsiClass = new ManagementClass(scope, new ManagementPath("Win32_Product"), null);

                var netUseParams = netUseCredClass.GetMethodParameters("Create");
                var commandLine = $"net use \"{installSettings.ProductPath}\" \"{secretCred.Password}\" /user:{secretCred.Login}";
                netUseParams["CommandLine"] = commandLine;
                var netUseOutParams = await Task.Run(() => netUseCredClass.InvokeMethod("Create", netUseParams, null));
                await Task.Delay(1000);
                await Task.Yield();
                var installParams = installMsiClass.GetMethodParameters("Install");
                installParams["PackageLocation"] = installSettings.ProductPath;
                installParams["Options"] = installSettings.Arguments;
                installParams["AllUsers"] = true;

                var outParams = await Task.Run(() => installMsiClass.InvokeMethod("Install", installParams, null));
                var resultFromOperation = outParams["returnValue"].ToString();
                if ((resultFromOperation == "1603" || resultFromOperation == "1619") && tryAgain)
                {
                    resultFromOperation = await InstallMsiAsync(wsName, installSettings, secretCred, false);
                }

                return resultFromOperation;
            }
            catch
            {
                await Task.Delay(1000);
                return $"Непредвиденная ошибка при установки {installSettings.ProductName} на компьютер {wsName.ToUpper()}";
            }
            
        }

        private Task WindowsInstallerService(string wsName)
        {
            ManagementObject serviceObject = new ManagementObject($"\\\\{wsName}\\root\\cimv2:Win32_Service.Name='msiserver'");
            serviceObject.Get();
            if (serviceObject["StartMode"].ToString().ToLower() == "disabled")
            {
                var inParams = serviceObject.GetMethodParameters("ChangeStartMode");
                inParams["StartMode"] = "Manual";
                serviceObject.InvokeMethod("ChangeStartMode", inParams, null);
                serviceObject.Get();
            }

            if (serviceObject["State"].ToString().ToLower() != "running")
            {
                serviceObject.InvokeMethod("StartService", null);
                serviceObject.Get();
            }

            return Task.FromResult(true);
        }

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
    }
}
