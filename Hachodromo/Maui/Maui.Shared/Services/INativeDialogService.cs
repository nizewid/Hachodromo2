using Hachodromo.Shared.Enums;

namespace Maui.Shared.Services
{
    public interface INativeDialogService
    {
        Task ShowAlertAsync(string title, string message, string okText = "Aceptar");
        Task ShowInfoAsync(string title, string message, string okText = "Aceptar");
        Task<bool> ShowConfirmMessageAsync(string title, string message, string yesText = "Sí", string noText = "No");

        Task ShowToastAsync(string message, ToastType type = ToastType.Info);
    }
}