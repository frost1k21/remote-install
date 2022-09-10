using Prism.Mvvm;
using Prism.Regions;
using System.Collections.ObjectModel;
using Prism.Commands;
using RemoteInstall.ConfigFromFile.Models;

namespace RemoteInstall.WpfUI.ViewModels
{
    public class SectionEditOneCBasesViewModel : BindableBase
    {
        #region Private Field(s)

        private readonly IRegionManager _regionManager;

        #endregion Private Field(s)

        #region Prism Property(ies)

        private bool _isEnabled;
        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

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

        private ReadOnlyObservableCollection<OneCBases> _readOnlyOneCBases;
        public ReadOnlyObservableCollection<OneCBases> ReadOnlyOneCBases
        {
            get => _readOnlyOneCBases;
            set => SetProperty(ref _readOnlyOneCBases, value);
        }

        private OneCBases _currentOnCBase;
        public OneCBases CurrentOneCBase
        {
            get => _currentOnCBase;
            set
            {
                SetProperty(ref _currentOnCBase, value);
                CanNavigate();
            }
        }

        #endregion //Prism Property(ies)

        #region DelegateCommand(s)

        public DelegateCommand<string> NavigateCommand { get; private set; }
        public DelegateCommand<string> NavigateAwayCommand { get; private set; }

        #endregion

        #region Constructor(s)

        public SectionEditOneCBasesViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            var configFromFile = new ConfigFromFile.ConfigForRemoteInstall("settings.xml");
            var oneCSettingsBases = configFromFile.OneCBaseses;
            ReadOnlyOneCBases = new ReadOnlyObservableCollection<OneCBases>(new ObservableCollection<OneCBases>(oneCSettingsBases));

            NavigateCommand = new DelegateCommand<string>(Navigate).ObservesCanExecute(() => IsEnabled);
            NavigateAwayCommand = new DelegateCommand<string>(Navigate);
        }

        #endregion //Constructor(s)

        #region Private Method(s)

        private void Navigate(string navigationPath)
        {
            if (!string.IsNullOrEmpty(navigationPath))
            {
                if (navigationPath == "ShowResultAfterOperation")
                {
                    var parameters = new NavigationParameters
                    {
                        {"computerName", ComputerName}, {"currentOneCBase", CurrentOneCBase}, {"editBases", "editBases"}
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
            if (!string.IsNullOrWhiteSpace(ComputerName) && CurrentOneCBase != null)
                IsEnabled = true;
            else
                IsEnabled = false;
        }

        #endregion //Private Method(s)
    }
}
