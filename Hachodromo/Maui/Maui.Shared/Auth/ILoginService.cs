namespace Maui.Shared.Auth
{
    public interface ILoginService
    {
        Task LoginAsync(string token);
        Task LogoutAsync();
    }
}
