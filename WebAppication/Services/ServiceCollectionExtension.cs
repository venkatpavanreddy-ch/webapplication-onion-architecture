using log4net;
using Newtonsoft.Json;
using WebAppication.Core.Common;
using WebAppication.Core.IService;
using WebAppication.Core.Models;
using WebAppication.Infrastructure.Common;
using WebAppication.Infrastructure.Service;

namespace WebAppication.Services
{
    public static class ServiceCollectionExtension
    {
        static ILog _logger = LogManager.GetLogger(typeof(ServiceCollectionExtension));

        public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            try
            {
                services.AddOptions();

                services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
                if (!File.Exists(configuration.GetValue<string>("SecretsPath").ToString()))
                    CreateSecrets(configuration);
                RegisterConfigurations(services, configuration, webHostEnvironment);

                RegisterBusinessComponents(services);
            }
            catch (Exception ex)
            {
                _logger.Error($"Exception details: {ex}");
            }
            return services;
        }

        private static void RegisterConfigurations(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            var secrets = GetSecrets(configuration);
            if (secrets == null)
            {
                _logger.Error("Unable to read the secrets from excrypted file.");
                throw new NullReferenceException("Unable to read the secrets from excrypted file.");
            }

            FolderTypesDTO folderTypes = new FolderTypesDTO()
            {
                PublicPath = configuration.GetValue<string>("PublicFolderPath"),
                RootFolderPath = configuration.GetValue<string>("RootFolderPath")
            };
            services.AddSingleton(folderTypes);


        }

        private static void RegisterBusinessComponents(IServiceCollection services)
        {
            services.AddTransient<IEmailService, EmailService>();
        }

        private static void CreateSecrets(IConfiguration configuration)
        {
            var key = PasswordRepository.ReadCredential(configuration.GetValue<string>("KeyName"));
            var iv = PasswordRepository.ReadCredential(configuration.GetValue<string>("IVName"));
            CleanDirectory(configuration.GetValue<string>("SecretsFolder").ToString());

            FileManager.EncryptFile(key.Password, iv.Password, $"{configuration.GetValue<string>("RootFolderPath")}\\Local_ItemPortalSecrets.json", configuration.GetValue<string>("SecretsPath").ToString());
        }

        private static void CleanDirectory(string path)
        {
            var sourceDirectory = new DirectoryInfo(path);

            if (sourceDirectory.Exists)
                sourceDirectory.Delete(true);

            sourceDirectory.Create();
        }

        private static dynamic? GetSecrets(IConfiguration configuration)
        {
            try
            {
                var key = PasswordRepository.ReadCredential(configuration.GetValue<string>("KeyName"));
                var iv = PasswordRepository.ReadCredential(configuration.GetValue<string>("IVName"));
                if (key == null || iv == null)
                {
                    _logger.Error("Key or iv is not found.");
                }
                if (string.IsNullOrEmpty(key?.Password) || string.IsNullOrEmpty(iv?.Password))
                {
                    _logger.Error("Key or iv value is empty.");
                }
                string keyValue = key != null && key.Password != null ? key.Password : "";
                string ivValue = iv != null && iv.Password != null ? iv.Password : "";
                var secrets = FileManager.GetSecrets(keyValue, ivValue, configuration.GetValue<string>("SecretsPath"));
                return JsonConvert.DeserializeObject<dynamic>(secrets);
            }
            catch (Exception ex)
            {
                _logger.Error($"Exception occured while reading secrets from encrypted file {ex}");
            }
            return null;
        }
    }
}
