using Cake.Core;
using System;
using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.FileHelpers;
using Cake.IIS;
using Cake.Json;
using WebAppication.Core.Common;

namespace Build;


public abstract class IisDeployment
{
    protected const string PROGRAM_FILES_SUB_FOLDER = "Nalashaa";

    public static IisDeployment Create(ICakeContext context, string environment, string deployProjectName)
    {
        var settingsFilePath = $"./Settings/{environment}.json";
        if (!context.FileExists(settingsFilePath))
            throw new System.Exception($"Invalid environment \"{environment}\": unable to find {environment}.json file in Settings folder");

        var settings = context.DeserializeJsonFromFile<DeploymentSettings>(settingsFilePath);

        if (settings.IsLocal)
            return new LocalIisDeployment(context, deployProjectName, settings, environment);
        else
            return new RemoteIisDeployment(context, deployProjectName, settings, environment);
    }

    protected ICakeContext _context;
    protected string _deployProjectName;
    protected DeploymentSettings _settings;
    protected string _environment;

    protected IisDeployment(ICakeContext context, string deployProjectName, DeploymentSettings settings, string environment)
    {
        _context = context;
        _deployProjectName = deployProjectName;
        _settings = settings;
        _environment = environment;
    }

    protected string PhysicalDirectory => $"C:/Program Files/{PROGRAM_FILES_SUB_FOLDER}/{_deployProjectName}";

    public void StageFiles()
    {
        _context.Information($"Deploying \u001b[36m{_environment}\u001b[0m version of {_deployProjectName} to \u001b[90m\"{DeploymentPath}\"\u001b[0m");
        _context.EnsureDirectoryExists(DeploymentPath);

        _context.CopyFiles(BuildContext.STAGING_FOLDER + "/**/*", DeploymentPath, true);

        UpdateSettings();
    }

    public void CreateAppPool()
    {
        var appPoolSettings = CreateApplicationPoolSettings();

        _context.CreatePool(appPoolSettings);
    }

    private ApplicationPoolSettings CreateApplicationPoolSettings()
    {
        var userName = _settings.UserName;
        var password = GetPasswordFromVault(userName);

        if (string.IsNullOrEmpty(password))
        {
            Console.WriteLine($"Please enter the password for {userName} ");
            password = Console.ReadLine();
        }

        return new ApplicationPoolSettings
        {
            Name = _deployProjectName,
            Username = "TLC\\" + userName,
            Password = password,
            LoadUserProfile = true,
        };
    }

    private string GetPasswordFromVault(string accountName)
    {
        var vaultPath = $"C:\\Nalashaa\\IT Stuff\\Deploy\\vault\\{_environment}\\{accountName}";
        try
        {
            var password = _context.FileReadText(vaultPath);

            return password;
        }
        catch (Exception ex)
        {
            return string.Empty;
        }
    }

    public void CreateWebsite()
    {
        var appPoolSettings = CreateApplicationPoolSettings();
        var authentication = new AuthenticationSettings
        {
            EnableWindowsAuthentication = true,
        };
        var websiteSetting = new WebsiteSettings
        {
            Name = _deployProjectName,
            PhysicalDirectory = PhysicalDirectory,
            Authentication = authentication,
            ApplicationPool = appPoolSettings,
        };
        _context.CreateWebsite(websiteSetting);
        var binding = IISBindings.Http.SetIpAddress("*");

        if (_settings.Port != 0)
            binding.SetPort(_settings.Port);

        binding.SetHostName(_settings.HostName);

        _context.AddBinding(_deployProjectName, binding);

        _context.RemoveBinding(_deployProjectName, websiteSetting.Binding);
    }

    private void UpdateSettings()
    {
        var jsonFilePath = $"{DeploymentPath}/appSettings.json";
        if (!_context.FileExists(jsonFilePath))
            throw new Exception($"Unable to find config file {jsonFilePath}");

        var parsedJObject = _context.ParseJsonFromFile(jsonFilePath);
        parsedJObject["Environment"] = _environment;
        parsedJObject["ServiceUserName"] = _settings.UserName;
        parsedJObject["DbUserName"] = _settings.DbUserName;
        parsedJObject["LeberDWSConnectionString"] = _settings.LeberDWSConnectionString;
        parsedJObject["ServiceStatusUrl"] = _settings.ServiceStatusUrl;

        _context.SerializeJsonToPrettyFile(jsonFilePath, parsedJObject);
        UpdateSecretsSettings();
    }

    private void UpdateSecretsSettings()
    {
        var jsonFilePath = $"{DeploymentPath}/wwwroot/Local_ItemPortalSecrets.json";

        if (!_context.FileExists(jsonFilePath))
            throw new Exception($"Unable to find config file {jsonFilePath}");

        var parsedJObject = _context.ParseJsonFromFile(jsonFilePath);
        var dbPassword = GetPasswordFromVault(_settings.DbUserName);
        if (string.IsNullOrEmpty(dbPassword))
        {
            Console.WriteLine($"Please enter the password for db user {_settings.DbUserName} ");
            dbPassword = Console.ReadLine();
        }
        var connectionString = _settings.LeberDWSConnectionString.Replace("{UserName}", _settings.DbUserName).Replace("{Password}", dbPassword);
        parsedJObject["ConnectionStrings"]["LeberDWS"] = connectionString;
        var emailPassword = GetPasswordFromVault(_settings.Email);
        if (string.IsNullOrEmpty(emailPassword))
        {
            Console.WriteLine($"Please enter the password for email {_settings.Email} ");
            emailPassword = Console.ReadLine();
        }
        var serviceUserPassword = GetPasswordFromVault($"TLC\\{_settings.ServiceUserName}");
        if (string.IsNullOrEmpty(emailPassword))
        {
            Console.WriteLine($"Please enter the password for service user {_settings.ServiceUserName} ");
            serviceUserPassword = Console.ReadLine();
        }
        parsedJObject["SmtpClientSettings"]["Password"] = emailPassword;
        parsedJObject["ClientCredential"]["Password"] = serviceUserPassword;
        parsedJObject["SmtpClientSettings"]["Email"] = _settings.Email;
        _context.SerializeJsonToPrettyFile(jsonFilePath, parsedJObject);
        CheckAndCreateKeyAndIV();
    }

    public void CheckAndCreateKeyAndIV()
    {
        var key = PasswordRepository.ReadCredential(_settings.EIPKeyName);
        var iv = PasswordRepository.ReadCredential(_settings.EIPIVName);
        if (key == null || iv == null || string.IsNullOrEmpty(key?.Password) || string.IsNullOrEmpty(iv?.Password))
        {
            Console.WriteLine($"Please enter the value for the key {_settings.EIPKeyName}");
            PasswordRepository.WriteCredential(_settings.EIPKeyName, _settings.EIPKeyName, Console.Read().ToString());

            Console.WriteLine($"Please enter the value for the iv {_settings.EIPIVName}");
            PasswordRepository.WriteCredential(_settings.EIPIVName, _settings.EIPIVName, Console.Read().ToString());
        }
    }

    public abstract void RemoveWebsite();
    public abstract void RemoveAppPool();
    public abstract string DeploymentPath { get; }
}


public class LocalIisDeployment : IisDeployment
{
    public LocalIisDeployment(ICakeContext context, string deployProjectName, DeploymentSettings settings, string environment)
        : base(context, deployProjectName, settings, environment)
    {
    }

    public override void RemoveWebsite()
    {
        if (_context.SiteExists(_deployProjectName))
        {
            _context.StopSite(_deployProjectName);
            _context.DeleteSite(_deployProjectName);
        }
    }

    public override void RemoveAppPool()
    {
        if (_context.PoolExists(_deployProjectName))
        {
            _context.StopPool(_deployProjectName);
            _context.DeletePool(_deployProjectName);
        }
    }

    public override string DeploymentPath => PhysicalDirectory;
}

public class RemoteIisDeployment : IisDeployment
{
    public RemoteIisDeployment(ICakeContext context, string deployProjectName, DeploymentSettings settings, string environment)
        : base(context, deployProjectName, settings, environment)
    {
    }

    public override void RemoveWebsite()
    {
        if (_context.SiteExists(_settings.ServerName, _deployProjectName))
        {
            _context.StopSite(_settings.ServerName, _deployProjectName);
            _context.DeleteSite(_settings.ServerName, _deployProjectName);
        }
    }

    public override void RemoveAppPool()
    {
        if (_context.PoolExists(_settings.ServerName, _deployProjectName))
        {
            _context.StopPool(_settings.ServerName, _deployProjectName);
            _context.DeletePool(_settings.ServerName, _deployProjectName);
        }
    }

    public override string DeploymentPath => $"\\\\{_settings.ServerName}\\C$\\Program Files\\{PROGRAM_FILES_SUB_FOLDER}\\{_deployProjectName}";
}
