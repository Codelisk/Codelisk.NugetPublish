using Cake.Common;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Frosting;
using Cake.Git;
using Codelisk.NugetPublish.Helper;
using System.Xml.Linq;

namespace Codelisk.NugetPublish.Tasks.Library
{
    [TaskName(nameof(NugetIncrementVersionTask))]
    public sealed class NugetIncrementVersionTask : FrostingTask<BuildContext>
    {
        public override bool ShouldRun(BuildContext context)
        {
            var result = context.IsRunningInCI;
            if (result && String.IsNullOrWhiteSpace(context.NugetApiKey))
                throw new ArgumentException("NugetApiKey is missing");

            return result;
        }


        public override void Run(BuildContext context)
        {
            
            var propsFilePath = "version.txt"; // Update with the correct file path

            // Increment the version and update the file
            IncrementAndSaveVersion(propsFilePath, context);
            CommitChanges(context);
        }

        // Increment the version and update the file
        private void IncrementAndSaveVersion(string filePath, BuildContext context)
        {
            context.StartProcess($"nbgv", new ProcessSettings
            {
                Arguments = new ProcessArgumentBuilder()
                    .Append($"prepare-release")
            });
        }
        private void CommitChanges(BuildContext context)
        {
            string name = "Pipeline";
            string email = "pipeline@commit.at";

            var commitMessage = "Updated version in Directory.Build.props"; // Customize your commit message
            //context.GitCheckout(filePath, new FilePath("CodeGen.git"));
            context.GitConfig(name, email);
            context.GitAdd();
            context.GitCommit(commitMessage);
            context.GitPushBranch();
        }
    }
}
