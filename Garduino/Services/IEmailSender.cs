using System.Threading.Tasks;

namespace Garduino.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
