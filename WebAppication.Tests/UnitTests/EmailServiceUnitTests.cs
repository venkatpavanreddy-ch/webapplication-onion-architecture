using NUnit.Framework;
using WebAppication.Infrastructure.Service;

namespace WebAppication.Tests.UnitTests
{
    [TestFixture]
    public class EmailServiceUnitTests
    {
        private readonly EmailService _emailService;

        public EmailServiceUnitTests()
        {
            _emailService = new EmailService();
        }

        [TestCase("simple@example.com")]
        [TestCase("example-indeed@strange-example.com")]
        [TestCase("user-@example.org")]
        public void Validate_Email(string email)
        {
            var res = _emailService.IsValidEmail(email);
            Assert.That(res);
        }

        [TestCase("Abc.example.com")]
        [TestCase("A@b@c@example.com")]
        [TestCase("just\"not\"right@example.com")]
        public void validate_Email_fails(string email)
        {
            var res = _emailService.IsValidEmail(email);
            Assert.That(!res);
        }

    }
}
