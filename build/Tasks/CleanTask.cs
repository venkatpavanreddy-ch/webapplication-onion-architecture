using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Frosting;

namespace Build.Tasks;

[TaskName("Clean")]
public sealed class CleanTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        if (context.DirectoryExists(BuildContext.STAGING_FOLDER))
            context.CleanDirectory(BuildContext.STAGING_FOLDER);

        foreach (var project in context.Projects)
        {
            context.Information($"Cleaning {project.Name} bin and obj folders");
            context.CleanDirectory($"{project.BinPath}");
            context.CleanDirectory($"{project.ObjPath}");
        }
    }
}
