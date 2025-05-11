#if ANDROID || WINDOWS || IOS || MACCATALYST
using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;
using System.Threading.Tasks;

namespace Maui.Shared.Services;

public class NativeDialogService : INativeDialogService
{
    public Task ShowAlertAsync(string title, string message, string okText) =>
        MainThread.InvokeOnMainThreadAsync(() =>
            GetPage().DisplayAlert(title, message, okText));

    public Task<bool> ShowConfirmMessageAsync(string title, string message,
                                              string yesText = "Sí", string noText = "No") =>
        MainThread.InvokeOnMainThreadAsync(() =>
            GetPage().DisplayAlert(title, message, yesText, noText));

    static Page GetPage() => Shell.Current ??
        Application.Current!.Windows.FirstOrDefault()?.Page ?? throw new InvalidOperationException("No active page found.");
}
#endif
