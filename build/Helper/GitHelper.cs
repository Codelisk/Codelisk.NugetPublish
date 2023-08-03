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
        public static void GitPushBranch(this BuildContext context)
        {
            var command = new ProcessArgumentBuilder()
                .Append("git")
                .Append("push")
                .Append("-u")
                .Append("https://github.com/Codelisk/CodeGen")
                .Append("master");

            // Execute the command
            var processSettings = new ProcessSettings
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            var result = context.StartProcess("git", new ProcessSettings { Arguments = command.Render() });
        }
    }
}
