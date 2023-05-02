using log4net;
using System.Net.Mail;
using WebAppication.Core.IService;
using WebAppication.Infrastructure.Common;

namespace WebAppication.Infrastructure.Service
{
    public class EmailService : IEmailService
    {
        private ILog _logger;

        public EmailService()
        {
            _logger = ConfigureLogging.For<EmailService>("Connectionstring");
        }

        /// <summary>
        /// Check if the given email id valid format or not
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error occurred while validating email, {ex}");
                return false;
            }
        }

    }
}
