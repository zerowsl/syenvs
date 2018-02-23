using System;

namespace SysEnvVars
{
    partial class Program
    {
        public static void Main(string[] @params)
        {
            var e = ArgParser.From(@params);

            if (e.IsDebug)
            {
                Console.WriteLine($"run params : {string.Join(" ", @params)} ");
                Console.ReadLine();
            }
            if (e.IsDisplayVersion)
            {
                exec_version(e);
                return;
            }
            if (e.IsCmdWithError)
            {
                exec_(e);
                return;
            }

            Exec exc = null;
            try
            {
                exc = get_do_exec(e);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            if (e.IsDisplayVersion) exec_version(e);
            if (e.IsCmdWithError) exec_(e);
            if (exc != null) exc(e);
            else exec_(e);
        }

        static Exec get_do_exec(Args e)
        {
            switch (e.Command?.ToLower())
            {
                case "help":
                    return get_exec_help(e);

                case "ls":
                case "list":
                    return get_exec_list(e);

                case "add":
                    return get_exec_add(e);

                case "set":
                    return get_exec_set(e);

                case "remove":
                case "rm":
                case "delete":
                case "del":
                    return get_exec_remove(e);

                default:
                    return null;
            }
        }

        static int exec_(Args e)
        {
            Console.WriteLine($"version: {Version}");
            Console.WriteLine($"no command can usable. using: '{CmdRunnerName} help' to check out");
            if (e.ProcessArgs?.Length <= 0) Console.ReadLine();
            return 0;
        }

        // (/|--|-)v|version
        static void exec_version(Args e)
        {
            Console.WriteLine($"{Version}");
        }
    }
}