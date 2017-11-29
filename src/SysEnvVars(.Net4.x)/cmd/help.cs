using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SysEnvVars
{
    partial class Program
    {
        // help [cmd]
        static Exec get_exec_help(Args e)
        {
            string cmd = null;

            if (e.XArgs.Count > 1)
            {
                throw new Exception("parse args and options error");
            }
            if (e.XArgs.Count == 1)
            {
                cmd = e.XArgs[0].ToLower();
            }

            return _ => 
            {
                Console.WriteLine($"version: {Version}");
                switch (cmd)
                {
                    case "list":
                    case "ls":
                        Console.WriteLine($@"    {CmdRunnerName} list|ls [options] <path1 path2 ...> [options]");
                        Console.WriteLine();
                        Console.WriteLine("options:");
                        Console.WriteLine(@"    -cu         current user env vars");
                        Console.WriteLine(@"    -inline     output raw vars values,if not,will spit with ';'");
                        Console.WriteLine(@"    -debug      log for vs debug");
                        break;

                    case "add":
                        Console.WriteLine($@"    {CmdRunnerName} add [options] <env-name> [options] <val_1 ... val_n>");
                        Console.WriteLine();
                        Console.WriteLine("options:");
                        Console.WriteLine(@"    -cu         current user env vars");
                        Console.WriteLine(@"    -debug      log for vs debug");
                        Console.WriteLine();
                        Console.WriteLine("example:");
                        Console.WriteLine($@"    {CmdRunnerName} add ""Path"" -cu ""%%windir%%\path0\\"" ""path1"" ""path2"" ");
                        Console.WriteLine($@"    {CmdRunnerName} add ""Path"" -cu ""%%windir%%\path0;path1;"" ""path2"" ");
                        break;

                    case "remove":
                    case "rm":
                    case "delete":
                    case "del":
                        Console.WriteLine($@"    {CmdRunnerName} remove|rm|del|delete [options] <env-name> [options] <val_1 ... val_n>");
                        Console.WriteLine();
                        Console.WriteLine("options:");
                        Console.WriteLine(@"    -cu         current user env vars");
                        Console.WriteLine(@"    -debug      log for vs debug");
                        Console.WriteLine();
                        Console.WriteLine("example:");
                        Console.WriteLine($@"    {CmdRunnerName} remove ""Path"" -cu ""%%windir%%\path0\\"" ""path1"" ""path2"" ");
                        Console.WriteLine($@"    {CmdRunnerName} del ""Path"" -cu ""%%windir%%\path0;path1;"" ""path2"" ");
                        break;

                    default:
                        Console.WriteLine($"using:{CmdRunnerName} <command> [args] [options]");
                        Console.WriteLine();
                        Console.WriteLine("available:");
                        Console.WriteLine($@"    {CmdRunnerName} -v");
                        Console.WriteLine($@"    {CmdRunnerName} help [command]");
                        Console.WriteLine($@"    {CmdRunnerName} list|ls [options] <path1 path2 ...> [options]");
                        Console.WriteLine($@"    {CmdRunnerName} add [options] <env-name> [options] <val_1 ... val_n>");
                        Console.WriteLine($@"    {CmdRunnerName} remove|rm|del|delete [options] <env-name> [options] <val_1 ... val_n>");
                        break;
                }
                Console.WriteLine();

                return 0;
            };
        }
    }
}