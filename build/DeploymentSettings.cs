namespace Build;

public class DeploymentSettings
{
    public string ServerName { get; set; }
    public string UserName { get; set; }
    public string ServiceUserName { get; set; }
    public string Email { get; set; }
    public string EIPKeyName { get; set; }
    public string EIPIVName { get; set; }
    public string DbUserName { get; set; }
    public string LeberDWSConnectionString { get; set; }
    public string ServiceStatusUrl { get; set; }

    public int Port { get; set; }
    public string HostName { get; set; }

    public bool IsLocal => ServerName == "localhost";
}
