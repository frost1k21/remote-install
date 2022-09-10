using System.Linq;
using System.Threading.Tasks;
using RemoteInstall.ConfigFromFile.Models;
using RemoteInstall.EditIbasesConfig;
using RemoteInstall.InstallViaSystemManagement;

namespace RemoteInstall.WpfUI.Services
{
    public class InstallOrEditService : IInstallOrEditService
    {
        private readonly SecretCred _secretCred;
        private readonly RemoteInstaller _remoteInstaller;
        private readonly OneCSettingsBases _oneCSettingsBases;

        public InstallOrEditService()
        {
            var configFromFile = new ConfigFromFile.ConfigForRemoteInstall("settings.xml");
            _secretCred = configFromFile.SecretCreds.First();
            _remoteInstaller = new RemoteInstaller();
            _oneCSettingsBases = new OneCSettingsBases();
        }
        public async Task<string> Install(string computerName, InstallSettings installSettings)
        {
            return await _remoteInstaller.InstallMsiAsync(computerName, installSettings, _secretCred, true);
        }

        public async Task<string> EditBase(string computerName, OneCBases oneCBases)
        {
            return await _oneCSettingsBases.UpdateIBasesWithResult(computerName, oneCBases);
        }
    }
}
