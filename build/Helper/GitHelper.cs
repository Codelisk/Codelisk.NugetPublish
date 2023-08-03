using Cake.Core.IO;
using Cake.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cake.Common;
using Cake.Git;
using Cake.Core.Diagnostics;

namespace Codelisk.NugetPublish.Helper
{
    public static class GitHelper
    {
        public static void GitSubmoduleForEach(this BuildContext context, string gitCommand)
        {
            context.StartProcess($"git", new ProcessSettings
            {
                Arguments = new ProcessArgumentBuilder()
                .Append($"submodule foreach '{gitCommand}'")
            });
        }
        public static void GitPushBranch(this BuildContext context, string branchName)
        {
            context.Log.Warning(context.Environment.WorkingDirectory.FullPath);
            context.StartProcess($"git", $"push origin -u {branchName}");
        }
    }
}
