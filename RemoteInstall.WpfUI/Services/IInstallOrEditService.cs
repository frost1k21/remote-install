using System.Threading.Tasks;
using RemoteInstall.ConfigFromFile.Models;

namespace RemoteInstall.WpfUI.Services
{
    public interface IInstallOrEditService
    {
        Task<string> Install(string computerName, InstallSettings installSettings);
        Task<string> EditBase(string computerName, OneCBases oneCBases);
    }
}
