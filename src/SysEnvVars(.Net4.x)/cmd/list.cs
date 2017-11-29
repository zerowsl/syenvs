using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SysEnvVars
{
    partial class Program
    {
        // list|ls [-inline] [-cu] <path1 path2 ...>
        // list|ls <path1 path2 ...> [-inline] [-cu] 
        static Exec get_exec_list(Args e)
        {
            var _inline = false;
            var paths = new List<string>();

            var _err_parse = false;
            var _pi = -1;

            for (var i = 0; i < e.XArgs.Count; i++)
            {
                var arg = e.XArgs[i];
                if (_err_parse) break;
                if (ArgParser.TryGetOption(arg, out var o))
                {
                    if (e.CheckOption(o)) continue;
                    switch (o)
                    {
                        case "inline":
                            _inline = true;
                            continue;
                        default:
                            _err_parse = true;
                            continue;
                    }
                }
                else if (_pi > -1 && _pi + 1 < i)
                {
                    _err_parse = true;
                    break;
                }
                else
                {
                    _pi = i;
                    paths.Add(arg.Trim('\"'));
                }
            }

            if (_err_parse) return exec_;

            return _ => 
            {
                using (var envs = EnvVarsHelper.GetEnvRegSubKey(e.IsCu))
                {
                    void _list_env_var(string ck)
                    {
                        if (_inline)
                        {
                            Console.WriteLine($"{ck}:\n{"   "}{(EnvVarsHelper.GetEnvKeyValue(envs, ck) ?? "[null]")}\n");
                        }
                        else
                        {
                            Console.WriteLine($"{ck}:");
                            var ckvs = EnvVarsHelper.GetEnvKeyValues(envs, ck);
                            if (ckvs == null) Console.WriteLine($" [null]\n");
                            else
                            {
                                foreach (var cv in ckvs)
                                    Console.WriteLine($"  {cv}");
                                Console.WriteLine();
                            }
                        }
                    }

                    if (paths.Count == 0)
                    {
                        foreach (var env in envs.GetValueNames())
                            _list_env_var(env);
                    }
                    else
                    {
                        foreach (var key in paths)
                            _list_env_var(key);
                    }
                }

                return 0;
            };
        }
    }
}