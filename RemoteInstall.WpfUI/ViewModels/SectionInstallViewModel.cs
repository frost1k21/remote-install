using System.Collections.Generic;
using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using RemoteInstall.ConfigFromFile.Models;

namespace RemoteInstall.WpfUI.ViewModels
{
    public class SectionInstallViewModel : BindableBase
    {
        #region Private Field(s)

        private readonly IRegionManager _regionManager;

        #endregion //Private Field(s)

        #region Prism Property(ies)

        private string _computerName;
        public string ComputerName
        {
            get => _computerName;
            set
            {
                SetProperty(ref _computerName, value);
                CanNavigate();
            }
        }

        private ReadOnlyObservableCollection<InstallSettings> _readonyInstallSettings;
        public ReadOnlyObservableCollection<InstallSettings> ReadonyInstallSettings
        {
            get => _readonyInstallSettings;
            set => SetProperty(ref _readonyInstallSettings, value);
        }

        private InstallSettings _currentInstallSettings;
        public InstallSettings CurrentInstallSettings
        {
            get => _currentInstallSettings;
            set
            {
                SetProperty(ref _currentInstallSettings, value);
                CanNavigate();
            }
        }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        #endregion //Prism Property(ies)

        #region Constructor(s)

        public SectionInstallViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            var configFromFile = new ConfigFromFile.ConfigForRemoteInstall("settings.xml");
            var installSettings = configFromFile.InstallSettingses;
            ReadonyInstallSettings = new ReadOnlyObservableCollection<InstallSettings>(new ObservableCollection<InstallSettings>(installSettings));

            NavigateCommand = new DelegateCommand<string>(Navigate).ObservesCanExecute(() => IsEnabled);
            NavigateAwayCommand = new DelegateCommand<string>(Navigate);
        }

        #endregion //Constructor(s)

        #region DelegateCommand(s)

        public DelegateCommand<string> NavigateCommand { get; private set; }
        public DelegateCommand<string> NavigateAwayCommand { get; private set; }

        #endregion DelegateCommand(s)

        #region Private Method(s)

        private void Navigate(string navigationPath)
        {
            if (!string.IsNullOrEmpty(navigationPath))
            {
                if (navigationPath == "ShowResultAfterOperation")
                {
                    var parameters = new NavigationParameters
                    {
                        {"computerName", ComputerName }, {"install", "install"}, {"currentInstallSettings", CurrentInstallSettings}
                    };
                    _regionManager.RequestNavigate("ContentRegion", navigationPath, parameters);
                }
                else
                {
                    _regionManager.RequestNavigate("ContentRegion", navigationPath);
                }
            }
        }

        private void CanNavigate()
        {
            if (!string.IsNullOrWhiteSpace(ComputerName) && CurrentInstallSettings != null)
                IsEnabled = true;
            else
                IsEnabled = false;
        }

        #endregion //Private Method(s)
    }
}
