using System.Collections.Generic;
using System.Linq;
using Cake.Common;
using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Core;
using Cake.Frosting;

namespace Build;

public class BuildContext : FrostingContext
{
    public const string STAGING_FOLDER = "../deployment/src";

    public List<ProjectInformation> Projects;

    public string Target { get; set; }
    public string BuildConfiguration { get; set; }
    public string Framework { get; set; }
    public string BuildEnvironment { get; set; }
    public string DeployProject { get; set; }
    public string SolutionFile { get; set; }
    public string GitRepositoryName { get; set; }

    public bool SkipTests = true;

    public BuildContext(ICakeContext context)
        : base(context)
    {
        Target = context.Argument("target", "Default");
        BuildConfiguration = context.Argument("configuration", "Debug");
        Framework = context.Argument("framework", "net6.0");
        BuildEnvironment = context.Argument("environment", "dev");
        DeployProject = context.Argument("deployProject", "WebAppication");
        SolutionFile = context.Argument("solutionFile", "WebAppication.sln");
        GitRepositoryName = context.Argument("gitRepositoryName", "WebAppication");

        context.Information($"Running tasks for {DeployProject}...");

        Projects = context.GetFiles("../**/*.csproj")
            .Select(fp => new ProjectInformation(BuildConfiguration, Framework)
            {
                Name = fp.GetFilenameWithoutExtension().ToString(),
                FullPath = fp.GetDirectory().FullPath,
            })
            .Where(p => p.Name != "Build")
            .ToList();
    }
}
