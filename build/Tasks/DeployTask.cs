using Cake.Frosting;

namespace Build.Tasks;

[TaskName("Deploy")]
public sealed class DeployTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        // TODO: allow deploying to staging website
        var iisDeployment = IisDeployment.Create(context, context.BuildEnvironment, context.DeployProject);

        iisDeployment.RemoveWebsite();
        iisDeployment.RemoveAppPool();

        iisDeployment.StageFiles();

        iisDeployment.CreateAppPool();
        iisDeployment.CreateWebsite();
    }
}
