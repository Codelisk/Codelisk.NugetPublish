using Cake.Frosting;
using Codelisk.NugetPublish.Tasks.Library;

namespace Codelisk.NugetPublish.Tasks;


[TaskName("Default")]
[IsDependentOn(typeof(CopyArtifactsTask))]
[IsDependentOn(typeof(NugetDeployTask))]
[IsDependentOn(typeof(ReleaseAnnouncementTask))]
//[IsDependentOn(typeof(GitHubReleaseTask))]
public sealed class DefaultTarget : FrostingTask<BuildContext> { }
