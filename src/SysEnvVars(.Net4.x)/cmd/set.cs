using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SysEnvVars
{
    partial class Program
    {
        // set [o] <env-name> [o] <val_1 ... val_n>
        static Exec get_exec_set(Args e)
        {
            string env_name = null;
            var values = new List<string>();

            var _err_parse = false;

            for (var i = 0; i < e.XArgs.Count; i++)
            {
                var arg = e.XArgs[i];
                if (_err_parse) break;
                if (ArgParser.TryGetOption(arg, out var o))
                {
                    if (values.Count > 0) o = null;
                    if (e.CheckOption(o)) continue;
                    switch (o)
                    {
                        default:
                            _err_parse = true;
                            continue;
                    }
                }
                else if (env_name == null)
                {
                    env_name = arg.Trim('\"');
                }
                else
                {
                    //values.Add(arg.Trim('\"'));
                    foreach (var a in arg.Trim('\"').Split(new[] { PathUtilities.SeparatorChar }))
                        values.Add(a);
                }
            }

            if (_err_parse) return exec_;

            return _ => 
            {
                if (string.IsNullOrEmpty(env_name))
                {
                    Console.WriteLine("env name must be not null or empty");
                    return 1;
                }
                using (var envs = EnvVarsHelper.GetEnvRegSubKey(e.IsCu))
                {
#if NetCore
                    EnvVarsHelper.SetOrUpdateVars(envs, (env_name, values));
#else 
                    EnvVarsHelper.SetOrUpdateVars(envs, ValueTuple.Create<string, IEnumerable<string>>(env_name, values));
#endif
                }

                return 0;
            };
        }
    }
}