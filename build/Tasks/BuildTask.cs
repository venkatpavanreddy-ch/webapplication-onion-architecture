using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Build;
using Cake.Frosting;

namespace Build.Tasks;

[TaskName("Build")]
[IsDependentOn(typeof(RestoreTask))]
public sealed class BuildTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var settings = new DotNetBuildSettings
        {
            Framework = context.Framework,
            Configuration = context.BuildConfiguration,
        };

        foreach (var project in context.Projects)
            context.DotNetBuild(project.FullPath, settings);
    }
}
