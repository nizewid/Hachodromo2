using Hachodromo.Shared.Responses;

namespace Hachodromo.API.Helpers
{
    public interface IMailHelper
    {
        Response SendMail(string toName, string toEmail, string subject, string body);
    }
}
