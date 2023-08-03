using Cake.Common.IO;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.NuGet.Push;
using Cake.Core.Diagnostics;
using Cake.Frosting;

namespace Codelisk.NugetPublish.Tasks.Library;


[TaskName("NugetDeploy")]
[IsDependentOn(typeof(BuildTask))]
//[IsDependeeOf(typeof(BasicTestsTask))]
public sealed class NugetDeployTask : FrostingTask<BuildContext>
{
    const string MainNuget = "https://api.nuget.org/v3/index.json";

    public override bool ShouldRun(BuildContext context)
    {
        var result = context.IsRunningInCI;
        if (result && String.IsNullOrWhiteSpace(context.NugetApiKey))
            throw new ArgumentException("NugetApiKey is missing");

        return result;
    }


    public override void Run(BuildContext context)
    {
        context.Log.Warning("NUGETAPIKEY" + context.NugetApiKey +"ENDE");
        var settings = new DotNetNuGetPushSettings
        {
            ApiKey = "oy2jmq4cpb4wtrtna2oqszv4m6ndnnhujhteddfyy7tusy",
            Source= MainNuget,
            SkipDuplicate = true,
            NoServiceEndpoint = true,
        };

        var packages = context.GetFiles("../../../src/**/*.nupkg");
        foreach (var package in packages)
        {
            //try
            //{
            context.Log.Warning("PACKAGE" + package.FullPath);
            context.DotNetNuGetPush(package.FullPath, settings);
            //}
            //catch (Exception ex)
            //{
            //    if (context.AllowNugetUploadFailures)
            //        context.Error($"Error Upload: {package.FullPath} - Exception: {ex}");
            //    else
            //        throw; // break build
            //}
        }
    }
}
