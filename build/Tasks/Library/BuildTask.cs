using Cake.Common;
using Cake.Common.IO;
using Cake.Common.Tools.MSBuild;
using Cake.Frosting;

namespace Codelisk.NugetPublish.Tasks.Library;


[TaskName(nameof(BuildTask))]
[IsDependentOn(typeof(NugetIncrementVersionTask))]
public sealed class BuildTask : FrostingTask<BuildContext>
{
    // needs to be windows build for UWP
    public override bool ShouldRun(BuildContext context) => context.IsRunningOnWindows();


    public override void Run(BuildContext context)
    {
        context.CleanDirectories($"**/obj/");
        context.CleanDirectories($"**/bin/{context.MsBuildConfiguration}");

        //context.MSBuild("Shiny.sln", x => x
        context.MSBuild("Build.slnf", x => x
            .WithRestore()
            .WithTarget("Clean")
            .WithTarget("Build")
            .WithProperty("PublicRelease", "true")
            .WithProperty("CI", context.IsRunningInCI.ToString())
            .WithProperty("OS", context.OperatingSystemString)
            .SetConfiguration(context.MsBuildConfiguration)
        );
    }
}