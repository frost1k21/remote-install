using Prism.Mvvm;
using System.Windows;

namespace RemoteInstall.WpfUI.SupportModels
{
    public class StatusInstallOrEdit : BindableBase
    {
        #region Prism Property(ies)

        private string _computerName;
        public string ComputerName
        {
            get => _computerName;
            set => SetProperty(ref _computerName, value);
        }

        private Visibility _inProcess = Visibility.Visible;
        public Visibility InProcess
        {
            get => _inProcess;
            set => SetProperty(ref _inProcess, value);
        }

        private Visibility _inError = Visibility.Collapsed;
        public Visibility InError
        {
            get => _inError;
            set => SetProperty(ref _inError, value);
        }

        private Visibility _isComplete = Visibility.Collapsed;
        public Visibility IsComplete
        {
            get => _isComplete;
            set => SetProperty(ref _isComplete, value);
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        #endregion //Prism Property(ies)

        #region Constructor(s)

        public StatusInstallOrEdit()
        {

        }

        #endregion //Constructor(s)
    }
}
