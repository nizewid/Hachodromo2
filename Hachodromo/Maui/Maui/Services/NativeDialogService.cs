#if ANDROID || IOS || MACCATALYST
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
#endif
using System;
using System.Linq;
using System.Threading.Tasks;
using Hachodromo.Shared.Enums;           // Para ToastType
using Maui.Shared.Services;             // Para INativeDialogService
using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;

namespace Maui.Services
{
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
#if ANDROID || IOS || MACCATALYST
            // Solo en plataformas móviles/maca: usamos CommunityToolkit.Maui.Toast
            var toast = Toast.Make(message, ToastDuration.Short, 14);
            return MainThread.InvokeOnMainThreadAsync(() => toast.Show());
#else
            // En Windows (y cualquier otra plataforma), usamos un DisplayAlert sencillo
            return MainThread.InvokeOnMainThreadAsync(() =>
                GetPage().DisplayAlert("", message, "OK"));
#endif
        }

        public Task ShowInfoAsync(string title, string message, string okText = "Aceptar") =>
            // Simplemente reutilizamos ShowAlertAsync para mostrar información
            ShowAlertAsync(title, message, okText);

        private static Page GetPage() =>
            Shell.Current
            ?? Application.Current?.Windows.FirstOrDefault()?.Page
            ?? throw new InvalidOperationException("No se encontró una página activa.");
    }
}
