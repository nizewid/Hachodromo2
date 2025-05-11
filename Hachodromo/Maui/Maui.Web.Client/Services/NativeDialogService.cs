#if !ANDROID && !WINDOWS && !IOS && !MACCATALYST
using Maui.Shared.Services;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Maui.Web.Client.Services;

public class NativeDialogService : INativeDialogService
{
    private readonly IJSRuntime js;
    public NativeDialogService(IJSRuntime js) => this.js = js;

    public Task ShowAlertAsync(string title, string message, string okText) =>
        js.InvokeVoidAsync("alert", $"{title}\n\n{message}").AsTask();

    public async Task<bool> ShowConfirmMessageAsync(string title, string message,
                                                    string yesText = "Sí", string noText = "No")
        => await js.InvokeAsync<bool>("confirm", $"{title}\n\n{message}");
}
#endif
