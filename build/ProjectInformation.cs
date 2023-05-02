namespace Build;

public class ProjectInformation
{
    public ProjectInformation(string configuration, string framework)
    {
        _configuration = configuration;
        _framework = framework;
    }

    private readonly string _configuration;
    private readonly string _framework;

    public string Name { get; set; }
    public string FullPath { get; set; }

    public bool IsTestProject => Name.EndsWith("Tests");

    public string BinPath => $"{FullPath}/bin";
    public string ObjPath => $"{FullPath}/obj";

    public string BuildPath => $"{FullPath}/bin/{_configuration}/{_framework}";

    public string DllPath => $"{BuildPath}/{Name}.dll";
}
