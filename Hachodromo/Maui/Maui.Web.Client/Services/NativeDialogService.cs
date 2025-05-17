#if !ANDROID && !WINDOWS && !IOS && !MACCATALYST
using Hachodromo.Shared.Enums;
using Maui.Shared.Services;
using MudBlazor;

namespace Maui.Web.Client.Services
{

    public class NativeDialogService : INativeDialogService
    {
        private readonly IDialogService dialogService;
        private readonly ISnackbar snackbar;


        public NativeDialogService(IDialogService dialogService, ISnackbar snackbar)
        {
            this.dialogService = dialogService;
            this.snackbar = snackbar;
        }

        public async Task ShowAlertAsync(string title, string message, string okText = "Aceptar")
        {
            await dialogService.ShowMessageBox(title, message, okText);
        }

        public async Task<bool> ShowConfirmMessageAsync(string title, string message, string yesText = "Sí", string noText = "No")
        {
            var result = await dialogService.ShowMessageBox(title, message, yesText, cancelText: noText);
            return result == true;
        }

        public async Task ShowInfoAsync(string title, string message, string okText = "Aceptar")
        {
            await dialogService.ShowMessageBox(title, message, okText);
        }

        public Task ShowToastAsync(string message, ToastType type = ToastType.Info)
        {
            var severity = type switch
            {
                ToastType.Success => Severity.Success,
                ToastType.Warning => Severity.Warning,
                ToastType.Error => Severity.Error,
                _ => Severity.Info
            };

            snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopEnd;
            snackbar.Add(message, severity);

            return Task.CompletedTask;
        }
    }
}
#endif
