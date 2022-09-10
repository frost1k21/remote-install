using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using RemoteInstall.ConfigFromFile.Models;

namespace RemoteInstall.WpfUI.ViewModels
{
    public class SectionMultipleInstallViewModel : BindableBase
    {
        #region Private Field(s)

        private readonly IRegionManager _regionManager;

        #endregion //Private Field(s)

        #region Prism Property(ies)

        private string _computerName;
        public string ComputerName
        {
            get => _computerName;
            set => SetProperty(ref _computerName, value);
        }

        private ObservableCollection<string> _computerNamesCollection = new ObservableCollection<string>();
        public ObservableCollection<string> ComputerNamesCollection
        {
            get => _computerNamesCollection;
            set
            {
                SetProperty(ref _computerNamesCollection, value);
                CanNavigate();
            }
        }

        private Visibility _listBoxVisibility = Visibility.Collapsed;
        public Visibility ListBoxVisibility
        {
            get => _listBoxVisibility;
            set => SetProperty(ref _listBoxVisibility, value);
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

        private string _textToParse;
        public string TextToParse
        {
            get { return _textToParse; }
            set { SetProperty(ref _textToParse, value); }
        }

        #endregion //Prism Property(ies)

        #region Public DelegateCommand(s)

        public DelegateCommand<string> NavigateCommand { get; private set; }
        public DelegateCommand<string> NavigateAwayCommand { get; private set; }
        public DelegateCommand AddToComputerNamesCollectionCommand { get; private set; }
        public DelegateCommand<string> DeleteFromCollectionCommand { get; private set; }
        public DelegateCommand DeleteAllFromCollectionCommand { get; private set; }
        public DelegateCommand AddToComputerNamesFromStringCollectionCommand { get; private set; }

        #endregion //Public DelegateCommand(s)

        #region Constructor(s)

        public SectionMultipleInstallViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            var configFromFile = new ConfigFromFile.ConfigForRemoteInstall("settings.xml");
            var installSettings = configFromFile.InstallSettingses;
            ReadonyInstallSettings = new ReadOnlyObservableCollection<InstallSettings>(new ObservableCollection<InstallSettings>(installSettings));

            NavigateCommand = new DelegateCommand<string>(Navigate).ObservesCanExecute(() => IsEnabled);
            NavigateAwayCommand = new DelegateCommand<string>(Navigate);
            AddToComputerNamesCollectionCommand = new DelegateCommand(AddToComputerNamesCollection);
            AddToComputerNamesFromStringCollectionCommand = new DelegateCommand(AddComputerNamesFromString);
            DeleteFromCollectionCommand = new DelegateCommand<string>(DeleteFromCollection);
            DeleteAllFromCollectionCommand = new DelegateCommand(DeleteAllFromCollection);
        }

        #endregion //Constructor(s)

        #region Private Method(s)

        private void Navigate(string navigationPath)
        {
            if (!string.IsNullOrEmpty(navigationPath))
            {
                if (navigationPath == "ShowMultipleResultAfterOperation")
                {
                    var parameters = new NavigationParameters
                    {
                        {"computerNames", ComputerNamesCollection}, {"install", "install"}, {"currentInstallSettings", CurrentInstallSettings }
                    };
                    _regionManager.RequestNavigate("ContentRegion", navigationPath, parameters);
                }
                else
                {
                    _regionManager.RequestNavigate("ContentRegion", navigationPath);
                }
            }
        }

        private void AddToComputerNamesCollection()
        {
            if (string.IsNullOrWhiteSpace(ComputerName)) return;
            if (!ComputerNamesCollection.Contains(ComputerName))
            {
                ComputerNamesCollection.Insert(0, ComputerName);
                ListBoxVisibility = Visibility.Visible;
            }
            ComputerName = "";
            CanNavigate();
        }

        private void AddComputerNamesFromString()
        {
            if (string.IsNullOrWhiteSpace(TextToParse))
            {
                TextToParse = string.Empty;
                return;
            } 
            var text = TextToParse.ToLower().Trim();
            var regex = new Regex(@"(?i)ws[\d]{3,4}");
            var result = regex.Matches(text);
            if (result.Count == 0) 
            {
                TextToParse = string.Empty;
                return;
            }
            var computerNames = GetComputerNamesFromRegexResult(result);
            foreach (var computer in computerNames)
            {
                if (!ComputerNamesCollection.Contains(computer))
                {
                    ComputerNamesCollection.Insert(0, computer);
                }
            }
            ListBoxVisibility = Visibility.Visible;
            TextToParse = string.Empty;
            CanNavigate();
        }

        private IEnumerable<string> GetComputerNamesFromRegexResult(MatchCollection matchCollection)
        {
            foreach (var item in matchCollection)
            {
                yield return item.ToString().Trim();
            }
        }

        private void DeleteFromCollection(string name)
        {
            ComputerNamesCollection.Remove(name);
            if (ComputerNamesCollection.Count == 0)
                ListBoxVisibility = Visibility.Collapsed;
            CanNavigate();
        }

        private void DeleteAllFromCollection()
        {
            ComputerNamesCollection = new ObservableCollection<string>();
            ListBoxVisibility = Visibility.Collapsed;
            CanNavigate();
        }

        private void CanNavigate()
        {
            if (CurrentInstallSettings != null && ComputerNamesCollection?.Count > 0)
                IsEnabled = true;
            else
                IsEnabled = false;
        }

        #endregion //Private Method(s)
    }
}
