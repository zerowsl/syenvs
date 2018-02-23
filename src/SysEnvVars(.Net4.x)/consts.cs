using System;

namespace SysEnvVars
{
#if !NetCore
    using System.IO;
#endif

    //delegate Exec DoParse(Args e);
    delegate int Exec(Args e);

    partial class Program
    {
        public const string Version = "0.4.0.0";

#if !NetCore
        public static readonly string CmdRunnerName;
#else
        public const string CmdRunnerName = "syenvs";
#endif

#if !NetCore
        static Program()
        {
            CmdRunnerName = Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs() is string[] ss && ss.Length > 0 ? ss[0] : typeof(Program).Assembly.ManifestModule.FullyQualifiedName);
        }
#endif
    }
}