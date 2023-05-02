using System.Linq;
using Cake.Common.Diagnostics;
using Cake.Common.Tools.NUnit;
using Cake.Frosting;

namespace Build.Tasks;

[TaskName("Run Tests")]
[IsDependentOn(typeof(BuildTask))]
public sealed class RunTestsTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        if (context.Projects.All(p => !p.IsTestProject))
        {
            context.WriteWarning("No test projects found");
            return;
        }

        if (context.SkipTests)
        {
            context.WriteWarning("Skipping tests as SkipTests is set to true");
            return;
        }

        foreach (var project in context.Projects.Where(p => p.IsTestProject))
        {
            context.Information($"Running tests for \u001b[90m{project.FullPath}\u001b[0m");

            var resultsFile = $"{project.BuildPath}/NUnitResults.xml";
            context.Information($"Results file \u001b[90m{resultsFile}\u001b[0m");
            var nUnitSettings = new NUnit3Settings
            {
                Results = new[] { new NUnit3Result { FileName = resultsFile } },
                WorkingDirectory = project.BuildPath,
            };
            context.Information($"Dll path: \u001b[90m\"{project.DllPath}\"\u001b[0m");
            // TODO: this throws an exception when it fails the first project, make this process all records
            context.NUnit3(project.DllPath, nUnitSettings);
            context.Information($"Finished running tests for \u001b[90m{project.FullPath}\u001b[0m");
        }
    }
}
