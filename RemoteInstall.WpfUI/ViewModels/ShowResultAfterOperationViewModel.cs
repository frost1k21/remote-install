using System;
using Prism.Mvvm;
using System.Windows;
using Prism.Regions;
using RemoteInstall.ConfigFromFile.Models;
using Prism.Commands;
using RemoteInstall.WpfUI.Services;
using RemoteInstall.WpfUI.SupportModels;
using System.Collections.Concurrent;

namespace RemoteInstall.WpfUI.ViewModels
{
    public class ShowResultAfterOperationViewModel : BindableBase, INavigationAware
    {
        #region Private Field(s)

        private readonly IInstallOrEditService _installOrEditService;
        private readonly IRegionManager _regionManager;
        private readonly ConcurrentDictionary<string, string> _msiCodeResultes = new ConcurrentDictionary<string, string>()
        {
            ["0"] = "Успешное завершение",
            ["13"] = "Данные недействительны.",
            ["87"] = "Один из параметров недействителен.",
            ["120"] = "Это значение возвращается, когда настраиваемое действие пытается вызвать функцию, которую нельзя вызвать из настраиваемых действий. Функция возвращает значение ERROR_CALL_NOT_IMPLEMENTED. Доступно, начиная с Windows Installer версии 3.0.",
            ["1259"] = "Этот код ошибки возникает только при использовании установщика Windows версии 2.0 и Windows XP. Если установщик Windows определяет, что продукт может быть несовместим с текущей операционной системой, он отображает диалоговое окно, информирующее пользователя и спрашивающее, следует ли в любом случае попытаться установить. Этот код ошибки возвращается, если пользователь отказывается от попытки установки.",
            ["1601"] = "Не удалось получить доступ к службе установщика Windows. Обратитесь в службу поддержки, чтобы убедиться, что служба установщика Windows правильно зарегистрирована.",
            ["1602"] = "Пользователь отменяет установку.",
            ["1603"] = "Во время установки произошла фатальная ошибка.",
            ["1604"] = "Установка приостановлена, не завершена.",
            ["1605"] = "Это действие действительно только для установленных продуктов.",
            ["1606"] = "Идентификатор функции не зарегистрирован.",
            ["1607"] = "Идентификатор компонента не зарегистрирован.",
            ["1608"] = "Это неизвестное свойство.",
            ["1609"] = "Дескриптор находится в недопустимом состоянии.",
            ["1610"] = "Данные конфигурации для этого продукта повреждены. Обратитесь в службу поддержки.",
            ["1611"] = "Спецификатора компонента нет.",
            ["1612"] = "Источник установки для этого продукта недоступен. Убедитесь, что источник существует и вы можете получить к нему доступ.",
            ["1613"] = "Этот установочный пакет не может быть установлен службой установщика Windows. Вы должны установить пакет обновления Windows, который содержит более новую версию службы установщика Windows.",
            ["1614"] = "Продукт удален.",
            ["1615"] = "Синтаксис SQL-запроса недействителен или не поддерживается.",
            ["1616"] = "Поле записи не существует.",
            ["1618"] = "Другая установка уже выполняется. Завершите эту установку, прежде чем продолжить эту установку.",
            ["1619"] = "Не удалось открыть этот установочный пакет. Убедитесь, что пакет существует и доступен, или обратитесь к поставщику приложения, чтобы убедиться, что это действительный пакет установщика Windows.",
            ["1620"] = "Не удалось открыть этот установочный пакет. Обратитесь к поставщику приложения, чтобы убедиться, что это допустимый пакет установщика Windows.",
            ["1621"] = "Произошла ошибка при запуске пользовательского интерфейса службы установщика Windows. Обратитесь в службу поддержки.",
            ["1622"] = "При открытии файла журнала установки произошла ошибка. Убедитесь, что указанное расположение файла журнала существует и доступно для записи.",
            ["1623"] = "Этот язык этого установочного пакета не поддерживается вашей системой.",
            ["1624"] = "При применении преобразований произошла ошибка. Убедитесь, что указанные пути преобразования действительны.",
            ["1625"] = "Эта установка запрещена системной политикой. Обратитесь к системному администратору.",
            ["1626"] = "Функция не может быть выполнена.",
            ["1627"] = "Сбой функции во время выполнения.",
            ["1628"] = "Указана неверная или неизвестная таблица.",
            ["1629"] = "Предоставлены данные неправильного типа.",
            ["1630"] = "Данные этого типа не поддерживаются.",
            ["1631"] = "Не удалось запустить службу установщика Windows. Обратитесь в службу поддержки.",
            ["1632"] = "Папка Temp заполнена или недоступна. Убедитесь, что папка Temp существует и в нее можно писать.",
            ["1633"] = "Этот установочный пакет не поддерживается на этой платформе. Обратитесь к поставщику приложения.",
            ["1634"] = "Компонент не используется на этой машине.",
            ["1635"] = "Не удалось открыть этот пакет исправлений. Убедитесь, что пакет исправлений существует и доступен, или обратитесь к поставщику приложения, чтобы убедиться, что это действительный пакет исправлений установщика Windows.",
            ["1636"] = "Не удалось открыть этот пакет исправлений. Обратитесь к поставщику приложения, чтобы убедиться, что это действительный пакет исправлений для установщика Windows.",
            ["1637"] = "Этот пакет исправлений не может быть обработан службой установщика Windows. Вы должны установить пакет обновления Windows, который содержит более новую версию службы установщика Windows.",
            ["1638"] = "Уже установлена ​​другая версия этого продукта. Установка этой версии не может быть продолжена.",
            ["1639"] = "Недопустимый аргумент командной строки. Обратитесь к SDK установщика Windows для получения подробной справки по командной строке.",
            ["1640"] = "Установка из клиентского сеанса сервера терминалов не разрешена для текущего пользователя.",
            ["1641"] = "Установщик инициировал перезапуск. Это сообщение свидетельствует об успехе. Этот код ошибки недоступен в установщике Windows версии 1.0.",
            ["1642"] = "Установщик не может установить исправление обновления, поскольку обновляемая программа может отсутствовать или исправление обновления обновляет другую версию программы. Убедитесь, что программа, которую нужно обновить, существует на вашем компьютере и что у вас есть правильный патч для обновления. Этот код ошибки недоступен в установщике Windows версии 1.0.",
            ["1643"] = "Пакет исправлений не разрешен системной политикой. Этот код ошибки доступен с установщиком Windows версии 2.0.",
            ["1644"] = "Одна или несколько настроек не разрешены системной политикой. Этот код ошибки доступен с установщиком Windows версии 2.0.",
            ["1645"] = "Установщик Windows не разрешает установку через подключение к удаленному рабочему столу. Доступно, начиная с установщика Windows версии 2.0 для Windows Server 2003.",
            ["1646"] = "Пакет исправлений не является съемным пакетом исправлений. Доступно, начиная с установщика Windows версии 3.0.",
            ["1647"] = "Патч не применяется к данному продукту. Доступно, начиная с установщика Windows версии 3.0.",
            ["1648"] = "Не удалось найти допустимую последовательность для набора исправлений. Доступно, начиная с установщика Windows версии 3.0.",
            ["1649"] = "Удаление исправления запрещено политикой. Доступно, начиная с установщика Windows версии 3.0.",
            ["1650"] = "Данные исправления XML недействительны. Доступно, начиная с установщика Windows версии 3.0.",
            ["1651"] = "Пользователь с правами администратора не смог применить исправление для приложения, управляемого пользователем, или приложения для компьютера, которое находится в состоянии объявления. Доступно, начиная с установщика Windows версии 3.0.",
            ["3010"] = "Для завершения установки требуется перезагрузка.",
            ["2147549445"] = "Ошибка сбоя сервера RPC"
        };

        #endregion //PrPrivate Field(s)

        #region Prism Property(ies)

        private OneCBases _oneCBase;
        public OneCBases OneCBase
        {
            get => _oneCBase;
            set => SetProperty(ref _oneCBase, value);
        }

        private InstallSettings _installSettings;
        public InstallSettings InstallSettings
        {
            get => _installSettings;
            set => SetProperty(ref _installSettings, value);
        }

        private StatusInstallOrEdit _statusInstallOrEdit;
        public StatusInstallOrEdit StatusInstallOrEdit
        {
            get => _statusInstallOrEdit;
            set => SetProperty(ref _statusInstallOrEdit, value);
        }

        #endregion //Prism Property(ies)

        #region Constructor(s)

        public ShowResultAfterOperationViewModel(IRegionManager regionManager, IInstallOrEditService installOrEditService)
        {
            _regionManager = regionManager;
            _installOrEditService = installOrEditService;

            NavigateCommand = new DelegateCommand<string>(Navigate);
        }

        #endregion //Constructor(s)

        #region DelegateCommand(s)

        public DelegateCommand<string> NavigateCommand { get; private set; }

        #endregion //DelegateCommand(s)

        #region Implementation of INavigationAware

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            StatusInstallOrEdit = new StatusInstallOrEdit();
            if (navigationContext.Parameters["computerName"] is string computerName)
                StatusInstallOrEdit.ComputerName = computerName;
            if (navigationContext.Parameters["currentInstallSettings"] is InstallSettings settings)
                InstallSettings = settings;
            if (navigationContext.Parameters["currentOneCBase"] is OneCBases bases)
                OneCBase = bases;
            if (navigationContext.Parameters["install"] is string)
                Install();
            if (navigationContext.Parameters["editBases"] is string)
                EditOneCBases();
        }

        #endregion //Implementation of INavigationAware

        #region Private Method(s)

        private async void Install()
        {
            StatusInstallOrEdit.StatusMessage = $"Идет установка {InstallSettings.ProductName} на компьютер {StatusInstallOrEdit.ComputerName.ToUpper()}";
            ChangeVisibility(VisibilityState.InAction, StatusInstallOrEdit);

            var result = await _installOrEditService.Install(StatusInstallOrEdit.ComputerName, InstallSettings);

            if (_msiCodeResultes.TryGetValue(result, out string codeMessage))
            {
                if (result == "0" || result == "3010")
                {
                    StatusInstallOrEdit.StatusMessage = $"Статус установки {InstallSettings.ProductName} на компьютер {StatusInstallOrEdit.ComputerName.ToUpper()}: {codeMessage}";
                    ChangeVisibility(VisibilityState.InComplete, StatusInstallOrEdit);
                }
                else
                {
                    StatusInstallOrEdit.StatusMessage = $"Статус установки {InstallSettings.ProductName} на компьютер {StatusInstallOrEdit.ComputerName.ToUpper()}: Ошибка {result} - {codeMessage}";
                    ChangeVisibility(VisibilityState.InError, StatusInstallOrEdit);
                }
            } 
            else
            {
                StatusInstallOrEdit.StatusMessage = result;
                ChangeVisibility(VisibilityState.InError, StatusInstallOrEdit);
            }
        }

        private async void EditOneCBases()
        {
            StatusInstallOrEdit.StatusMessage = $"Идет настройка {OneCBase.NameForUser} на компьютере {StatusInstallOrEdit.ComputerName}";
            ChangeVisibility(VisibilityState.InAction, StatusInstallOrEdit);
            var result = await _installOrEditService.EditBase(StatusInstallOrEdit.ComputerName, OneCBase);
            if (result.ToLowerInvariant().StartsWith("настройка баз на компьютере"))
            {
                StatusInstallOrEdit.StatusMessage = result;
                ChangeVisibility(VisibilityState.InComplete, StatusInstallOrEdit);
            }
            else
            {
                StatusInstallOrEdit.StatusMessage = result;
                ChangeVisibility(VisibilityState.InError, StatusInstallOrEdit);
            }
        }

        private void Navigate(string navigationPath)
        {
            if (!string.IsNullOrEmpty(navigationPath))
                _regionManager.RequestNavigate("ContentRegion", navigationPath);
        }

        private void ChangeVisibility(VisibilityState state, StatusInstallOrEdit item)
        {
            switch (state)
            {
                case VisibilityState.InAction:
                    item.InProcess = Visibility.Visible;
                    item.IsComplete = Visibility.Collapsed;
                    item.InError = Visibility.Collapsed;
                    break;
                case VisibilityState.InComplete:
                    item.InProcess = Visibility.Collapsed;
                    item.IsComplete = Visibility.Visible;
                    item.InError = Visibility.Collapsed;
                    break;
                case VisibilityState.InError:
                    item.InProcess = Visibility.Collapsed;
                    item.IsComplete = Visibility.Collapsed;
                    item.InError = Visibility.Visible;
                    break;
            }
        }

        #endregion //Private Method(s)
    }
}
