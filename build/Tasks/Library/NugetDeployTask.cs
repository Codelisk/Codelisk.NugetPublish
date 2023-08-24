using Cake.Common;
using Cake.Common.IO;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Frosting;

namespace Codelisk.NugetPublish.Tasks.Library;


[TaskName(nameof(NugetDeployTask))]
[IsDependentOn(typeof(BuildTask))]
//[IsDependeeOf(typeof(BasicTestsTask))]
public sealed class NugetDeployTask : FrostingTask<BuildContext>
{
    private const string MainNuget = "https://api.nuget.org/v3/index.json";

    public override bool ShouldRun(BuildContext context)
    {
        var result = context.IsRunningInCI;
        if (result && String.IsNullOrWhiteSpace(context.NugetApiKey))
            throw new ArgumentException("NugetApiKey is missing");

        return result;
    }


    public override void Run(BuildContext context)
    {
        var packages = context.GetFiles("**/*.nupkg");
        foreach (var package in packages)
        {
            context.Log.Information("Pushing NuGet package {0}", package.FullPath);
            // Build the command
            var command = new ProcessArgumentBuilder()
                .Append("dotnet")
                .Append("nuget")
                .Append("push")
                .Append(package.FullPath)              // Add the package file path
                .Append("--source")                   // Specify the NuGet source
                .Append(MainNuget)                    // Add the source URL
                .Append("--api-key")
                .AppendSecret(context.NugetApiKey)// Add the API key as a secret
                .Append("--skip-duplicate");

            // Execute the command
            var processSettings = new ProcessSettings
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            var result = context.StartProcess("dotnet", new ProcessSettings { Arguments = command.Render() });
        }
    }
}
