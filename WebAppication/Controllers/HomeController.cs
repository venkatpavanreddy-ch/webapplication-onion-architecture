using log4net;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebAppication.Core.IService;
using WebAppication.Core.Models;
using WebAppication.Infrastructure.Common;
using WebAppication.Models;

namespace WebAppication.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEmailService _emailService;
        private readonly FolderTypesDTO _folderTypesDTO;
        private ILog _logger;

        public HomeController(IEmailService emailService, FolderTypesDTO folderTypesDTO)
        {
            _emailService = emailService;
            _folderTypesDTO = folderTypesDTO;
            _logger = ConfigureLogging.For<HomeController>("");
        }

        public IActionResult Index(string message)
        {
            string msg = string.IsNullOrEmpty(message) ? "" : message;
            ViewBag.Error = msg;
            _logger.Info("Home Index started");
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public string IsValidEmail(string email)
        {
            string result = string.Empty;
            if (string.IsNullOrEmpty(email))
                result = "please enter email.";
            try
            {
                result = _emailService.IsValidEmail(email).ToString();
            }
            catch (Exception ex)
            {
                result = "please enter valid email.";
                _logger.Error($"Error occurred while validating email, {ex}");
            }
            return result;
        }
    }
}