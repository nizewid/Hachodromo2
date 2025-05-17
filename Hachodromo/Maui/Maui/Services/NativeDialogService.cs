#if ANDROID || WINDOWS || IOS || MACCATALYST
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Hachodromo.Shared.Enums;
using Maui.Shared.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;

namespace Maui.Services { 

public class NativeDialogService : INativeDialogService
{
    public Task ShowAlertAsync(string title, string message, string okText = "Aceptar") =>
        MainThread.InvokeOnMainThreadAsync(() =>
            GetPage().DisplayAlert(title, message, okText));

    public Task<bool> ShowConfirmMessageAsync(string title, string message, string yesText = "Sí", string noText = "No") =>
        MainThread.InvokeOnMainThreadAsync(() =>
            GetPage().DisplayAlert(title, message, yesText, noText));

        public Task ShowToastAsync(string message, ToastType type = ToastType.Info)
        {
            var toast = Toast.Make(message, ToastDuration.Short, 14); // duración y fuente opcional
            return MainThread.InvokeOnMainThreadAsync(() => toast.Show());
        }

        private static Page GetPage() => Shell.Current ??
        Application.Current?.Windows.FirstOrDefault()?.Page ??
        throw new InvalidOperationException("No se encontró una página activa.");

    public Task ShowInfoAsync(string title, string message, string okText = "Aceptar")
    {
        return ShowAlertAsync(title, message, okText); // mismo backend, diferente intención
    }
}
}
#endif
