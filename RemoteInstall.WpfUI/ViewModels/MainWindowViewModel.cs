using Prism.Mvvm;
using Prism.Regions;
using RemoteInstall.WpfUI.Views;

namespace RemoteInstall.WpfUI.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Программа для установки программ и настройки 1С баз версия 1.0.0";

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public MainWindowViewModel(IRegionManager regionManager)
        {
            regionManager.RegisterViewWithRegion("ContentRegion", typeof(IsntallOrEditBases));
        }
    }
}
