﻿using Cake.Common;
using Cake.Common.IO;
using Cake.Common.Tools.MSBuild;
using Cake.Core.Diagnostics;
using Cake.Frosting;

namespace Codelisk.NugetPublish.Tasks.Library;


[TaskName("Build")]
public sealed class BuildTask : FrostingTask<BuildContext>
{
    // needs to be windows build for UWP
    public override bool ShouldRun(BuildContext context) => context.IsRunningOnWindows();


    public override void Run(BuildContext context)
    {
        context.CleanDirectories($"**/src/**/obj/");
        context.CleanDirectories($"**/src/**/bin/{context.MsBuildConfiguration}");

        //context.MSBuild("Shiny.sln", x => x
        context.MSBuild("../../../Build.slnf", x => x
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