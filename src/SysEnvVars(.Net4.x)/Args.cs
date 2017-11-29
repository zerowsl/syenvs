using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SysEnvVars
{
    class ArgParser
    {
        public const string OptionPrev = "/|--|-";

        public static bool TryGetOption(string arg, out string o)
        {
            o = Regex.Replace(arg.ToLower(), $@"^({OptionPrev})", string.Empty);
            return !string.Equals(arg, o, StringComparison.OrdinalIgnoreCase);
        }

        public static string GetNextArgItem(IEnumerator<string> argsEnumerator)
        {
            if (argsEnumerator == null || !argsEnumerator.MoveNext()) return null;
            return argsEnumerator.Current;
        }

        public static Args From(IEnumerable<string> args)
        {
            var a = new Args { ProcessArgs = args.ToArray(), XArgs = new List<string>() };
            var cmd = a.Command;
            foreach (var arg in a.ProcessArgs)
            {
                var b = TryGetOption(arg, out var oa);
                if (b)
                {
                    switch (oa)
                    {
                        case "v":
                        case "version":
                            a.IsDisplayVersion = true;
                            continue;
                        case "debug":
                            a.IsDebug = true;
                            continue;
                    }
                }
                if (cmd == null)
                {   
                    if (!b) cmd = a.Command = arg;
                    else
                    {
                        if (a.CheckOption(oa)) continue;
                        if (a.IsDisplayVersion) break;
                        a.IsCmdWithError = true;
                        break;
                    }
                }
                else
                {
                    a.XArgs.Add(arg);
                }
            }
            return a;
        }
    }

    class Args
    {
        public string[] ProcessArgs { get; set; }
        public IList<string> XArgs { get; set; }

        public string Command { get; set; }
        public bool IsCmdWithError { get; set; }

        public bool IsDisplayVersion { get; set; }
        public bool IsDebug { get; set; }
        public bool IsCu { get; set; }

        public bool CheckOption(string o)
        {
            switch (o)
            {
                case "cu":
                    IsCu = true;
                    return true;
                case "debug":
                    IsDebug = true;
                    return true;
            }
            return false;
        }
    }
}