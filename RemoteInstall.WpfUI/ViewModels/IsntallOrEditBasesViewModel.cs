using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace RemoteInstall.WpfUI.ViewModels
{
    public class IsntallOrEditBasesViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;

        public DelegateCommand<string> NavigateCommand { get; private set; }

        public IsntallOrEditBasesViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            NavigateCommand = new DelegateCommand<string>(Navigate);
        }

        private void Navigate(string navigationPath)
        {
            if (!string.IsNullOrEmpty(navigationPath))
                _regionManager.RequestNavigate("ContentRegion", navigationPath);
        }
    }
}
