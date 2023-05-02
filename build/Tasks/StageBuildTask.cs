using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Publish;
using Cake.Frosting;
using System;
using System.IO;
using System.Linq;
using Cake.Common.IO;

namespace Build.Tasks;

[TaskName("Stage Build")]
[IsDependentOn(typeof(BuildTask))]
public sealed class StageBuildTask : FrostingTask<BuildContext>
{
    //private const string ROOT_BUILD_PATH = @"\\lm-srvapp01\IT Stuff\Deploy\builds\";
    private const string ROOT_BUILD_PATH = @"C:\\Lebermuth\IT Stuff\Deploy\builds\";


    public override void Run(BuildContext context)
    {
        var toDeploy = context.Projects.SingleOrDefault(p => p.Name == context.DeployProject);
        if (toDeploy == null)
            throw new Exception($"Unable to find the project {context.DeployProject}, be sure this value is configured correctly.");
        var settings = new DotNetPublishSettings
        {
            Framework = context.Framework,
            Configuration = context.BuildConfiguration,
            OutputDirectory = BuildContext.STAGING_FOLDER,
        };

        context.DotNetPublish(toDeploy.FullPath, settings);

        var projectPath = Path.Combine(ROOT_BUILD_PATH, context.GitRepositoryName);
        if (!context.DirectoryExists(projectPath))
        {
            throw new Exception($"Skipping install \"{context.GitRepositoryName}\" does not exist inside of \"{ROOT_BUILD_PATH}\"");
        }

        var dateFolderName = DateTime.Today.ToString("yyyy-MM-dd");
        var remoteStagingPath = Path.Combine(projectPath, dateFolderName);
        if (context.DirectoryExists(remoteStagingPath))
        {
            throw new Exception($"Skipping install \"{dateFolderName}\" already exists inside of \"{projectPath}\"");
        }

        context.EnsureDirectoryExists(remoteStagingPath);

        var buildFolderDestinationPath = Path.Combine(remoteStagingPath, "build");
        context.EnsureDirectoryExists(buildFolderDestinationPath);
        context.CopyFiles("*.bat", buildFolderDestinationPath);
        context.CopyDirectory("settings", Path.Combine(buildFolderDestinationPath, "settings"));
        context.CopyDirectory("bin", Path.Combine(buildFolderDestinationPath, "bin"));

        var deploymentFolderDestinationPath = Path.Combine(remoteStagingPath, "deployment");
        context.EnsureDirectoryExists(deploymentFolderDestinationPath);
        context.CopyDirectory("../deployment/src", Path.Combine(deploymentFolderDestinationPath, "src"));
    }
}
