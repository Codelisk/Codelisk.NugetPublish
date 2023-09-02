using Cake.Common;
using Cake.Common.Build;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Frosting;
using Cake.Git;
using Cake.GitVersioning;

namespace Codelisk.NugetPublish;


public class BuildContext : FrostingContext
{
    public BuildContext(ICakeContext context) : base(context)
    {
        context.StartProcess($"nbgv", new ProcessSettings
        {
            Arguments = new ProcessArgumentBuilder()
                .Append($"prepare-release")
        });

#if DEBUG
        //walk backwards until git directory found -that's root
        if (false && !context.GitIsValidRepository(context.Environment.WorkingDirectory))
        {
            var dir = new DirectoryPath(".");
            dir = new DirectoryPath(Directory.GetParent(dir.FullPath).FullName);
            dir = new DirectoryPath(Directory.GetParent(dir.FullPath).FullName);
            while (!context.GitIsValidRepository(dir))
                dir = new DirectoryPath(Directory.GetParent(dir.FullPath).FullName);

            context.Environment.WorkingDirectory = dir;
        }
#endif
        var test = this.GitVersioningGetVersion();
        ReleaseVersion = this.GitVersioningGetVersion().NuGetPackageVersion;
        Log.Information("NUGET PACKAGE VERSION: " + ReleaseVersion);
    }


    public string ReleaseVersion { get; }
    //public bool UseXamarinPreview => this.HasArgumentOrEnvironment("UseXamarinPreview");
    public string GitHubSecretToken => ArgumentOrEnvironment<string>("GITHUB_TOKEN");
    public string OperatingSystemString => Environment.Platform.Family == PlatformFamily.Windows ? "WINDOWS_NT" : "MAC";
    public string MsBuildConfiguration => ArgumentOrEnvironment("configuration", Constants.DefaultBuildConfiguration);
    public string NugetApiKey => ArgumentOrEnvironment<string>("NugetApiKey");
    public bool AllowNugetUploadFailures => ArgumentOrEnvironment("AllowNugetUploadFailures", false);
    public GitBranch Branch { get; }

    public T ArgumentOrEnvironment<T>(string name, T defaultValue = default)
        => this.HasArgument(name) ? this.Argument<T>(name) : this.EnvironmentVariable<T>(name, defaultValue);

    public bool HasArgumentOrEnvironment(string name)
        => this.HasArgument(name) || this.HasEnvironmentVariable(name);

    public string ArtifactDirectory
    {
        get
        {
            if (IsRunningInCI)
                return this.GitHubActions().Environment.Workflow.Workspace + "/artifacts";

            return System.IO.Path.Combine(Environment.WorkingDirectory.FullPath, "artifacts");
        }
    }

    public bool IsReleaseBranch => Branch.FriendlyName.ToLower().StartsWith("v");


    public bool IsPullRequest =>
        IsRunningInCI &&
        this.GitHubActions().Environment.PullRequest.IsPullRequest;

    public bool IsRunningInCI
        => this.GitHubActions()?.IsRunningOnGitHubActions ?? false;


    public bool IsNugetDeployBranch
    {
        get
        {
            if (IsPullRequest)
                return false;

            var bn = Branch.FriendlyName.ToLower();
            return bn.Equals("main") || bn.Equals("master") || bn.Equals("preview") || bn.StartsWith("v");
        }
    }


    public bool IsDocsDeployBranch => IsNugetDeployBranch && !IsPullRequest;
}