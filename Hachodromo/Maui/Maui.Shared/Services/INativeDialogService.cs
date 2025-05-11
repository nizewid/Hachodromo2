namespace Maui.Shared.Services
{ 
    public interface INativeDialogService
    {
        Task ShowAlertAsync(string title, string message, string okText);

        /// <summary>
        /// Devuelve true si el usuario acepta (Yes/OK) o false si cancela.
        /// </summary>
        Task<bool> ShowConfirmMessageAsync(string title, string message,
                                           string yesText = "Sí",
                                           string noText = "No");
    }
}