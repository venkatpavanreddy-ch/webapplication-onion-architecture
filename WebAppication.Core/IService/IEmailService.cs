using WebAppication.Core.Models;

namespace WebAppication.Core.IService
{
    public interface IEmailService
    {
        public bool IsValidEmail(string email);
    }
}