using Cake.Core.Diagnostics;
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
            var xmlContent = File.ReadAllText(filePath);

            // Find the Version element and increment it
            xmlContent = UpdateVersion(xmlContent);
            context.Log.Warning(xmlContent);
            // Write the updated content back to the file
            File.WriteAllText(filePath, xmlContent);
        }

        private string UpdateVersion(string xmlContent)
        {
            var doc = XDocument.Parse(xmlContent);

            var namespaceName = doc.Root.GetDefaultNamespace().NamespaceName;
            XNamespace ns = namespaceName;

            var versionNode = doc.Descendants("PropertyGroup")
                                 .Where(pg => pg.Attribute("Condition")?.Value == "'$(Configuration)' == 'Release'")
                                 .Elements("Version")
                                 .FirstOrDefault();

            if (versionNode != null && Version.TryParse(versionNode.Value, out var version))
            {
                var incrementedVersion = new Version(version.Major, version.Minor, version.Build + 1);
                versionNode.Value = incrementedVersion.ToString();
                return doc.ToString();
            }

            throw new InvalidOperationException("Version element not found or invalid in the XML.");
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
