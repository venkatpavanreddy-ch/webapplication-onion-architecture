using Cake.Frosting;

namespace Build.Tasks;

[IsDependentOn(typeof(RunTestsTask))]
[IsDependentOn(typeof(StageBuildTask))]
[IsDependentOn(typeof(DeployTask))]
public sealed class Default : FrostingTask
{
}
