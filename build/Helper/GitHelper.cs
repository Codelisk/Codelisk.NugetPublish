using Cake.Common;
using Cake.Core;
using Cake.Core.IO;

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
        public static void GitConfig(this BuildContext context, string username, string email)
        {
            var command = new ProcessArgumentBuilder()
                .Append("config")
                .Append("--global")
            .Append("user.name")
                .Append($"\"{username}\"");

            var command2 = new ProcessArgumentBuilder()
                .Append("config")
                .Append("--global")
            .Append("user.email")
                .Append($"\"{email}\"");

            var result = context.StartProcess("git", new ProcessSettings { Arguments = command.Render() });
            context.StartProcess("git", new ProcessSettings { Arguments = command2.Render() });
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
                .Append($"\"{message}\"");


            var result = context.StartProcess("git", new ProcessSettings { Arguments = command.Render() });
        }
        public static void GitPushBranch(this BuildContext context)
        {
            var command = new ProcessArgumentBuilder()
                .Append("push")
                .Append("-u")
                .Append("master");

            var result = context.StartProcess("git", new ProcessSettings { Arguments = command.Render() });
        }
    }
}
