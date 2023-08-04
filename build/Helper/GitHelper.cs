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
        public static void GitAdd(this BuildContext context)
        {
            var command = new ProcessArgumentBuilder()
                .Append("add")
                .Append(".");

            var result = context.StartProcess("git", new ProcessSettings { Arguments = command.Render() });
        }
        public static void GitCommit(this BuildContext context, string message)
        {
            var command = new ProcessArgumentBuilder()
                .Append("commit")
                .Append("-m")
                .Append(message);


            var result = context.StartProcess("git", new ProcessSettings { Arguments = command.Render() });
        }
        public static void GitPushBranch(this BuildContext context)
        {
            var command = new ProcessArgumentBuilder()
                .Append("push")
                .Append("-u")
                .Append("https://github.com/Codelisk/CodeGen")
                .Append("master");

            var result = context.StartProcess("git", new ProcessSettings { Arguments = command.Render() });
        }
    }
}
