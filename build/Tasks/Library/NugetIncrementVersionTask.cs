using Cake.Common.IO;
using Cake.Common;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Frosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codelisk.NugetPublish.Tasks.Library
{
    [TaskName(nameof(NugetIncrementVersionTask))]
    [IsDependentOn(typeof(BuildTask))]
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
            var propsFilePath = "../../../Directory.Build.props"; // Update with the correct file path

            // Increment the version and update the file
            IncrementAndSaveVersion(propsFilePath);
        }
        // Increment the version and update the file
        void IncrementAndSaveVersion(string filePath)
        {
            var xmlContent = File.ReadAllText(filePath);

            // Find the Version element and increment it
            xmlContent = UpdateVersion(xmlContent);

            // Write the updated content back to the file
            File.WriteAllText(filePath, xmlContent);
        }

        // Update the Version element in the XML content
        private string UpdateVersion(string xmlContent)
        {
            var xml = new System.Xml.XmlDocument();
            xml.LoadXml(xmlContent);

            var versionNode = xml.SelectSingleNode("//PropertyGroup[Condition=\"'$(Configuration)' == 'Release'\"]/Version");
            if (versionNode != null && Version.TryParse(versionNode.InnerText, out var version))
            {
                var incrementedVersion = new Version(version.Major, version.Minor, version.Build + 1, version.Revision);
                versionNode.InnerText = incrementedVersion.ToString();
                return xml.OuterXml;
            }

            throw new InvalidOperationException("Version element not found or invalid in the XML.");
        }
    }
}
