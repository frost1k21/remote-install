using RemoteInstall.WpfUI.Views;
using Prism.Ioc;
using System.Windows;
using RemoteInstall.WpfUI.Services;

namespace RemoteInstall.WpfUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            var splashScreenWindow = new SplashScreenWindow();
            splashScreenWindow.Show();

            var mainShell =  Container.Resolve<MainWindow>();
            mainShell.Dispatcher.InvokeAsync(() =>
            {
                mainShell.Show();
                splashScreenWindow.Close();
            });

            return mainShell;
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            #region Register for navigation

            containerRegistry.RegisterForNavigation<IsntallOrEditBases>();
            containerRegistry.RegisterForNavigation<SectionInstall>();
            containerRegistry.RegisterForNavigation<SectionEditOneCBases>();
            containerRegistry.RegisterForNavigation<ShowResultAfterOperation>();
            containerRegistry.RegisterForNavigation<SectionMultipleInstall>();
            containerRegistry.RegisterForNavigation<SectionMultipleEditOneCBases>();
            containerRegistry.RegisterForNavigation<ShowMultipleResultAfterOperation>();

            #endregion //Register for navigation

            #region Register types

            containerRegistry.Register<IInstallOrEditService, InstallOrEditService>();

            #endregion //Register types

        }
    }
}
