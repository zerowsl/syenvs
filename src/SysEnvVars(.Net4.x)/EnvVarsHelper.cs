using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

#if !NetCore
    using System.Security.AccessControl;
    using System.Security.Permissions;
#endif

static class EnvVarsHelper
{
    public static RegistryKey GetEnvRegSubKey(bool cu = false)
    {
        if (!cu)
        {
            return Registry.LocalMachine.OpenSubKey(
                    @"SYSTEM\CurrentControlSet\Control\Session Manager\Environment",
                    //RegistryKeyPermissionCheck.Default,
                    //RegistryRights.ReadKey | RegistryRights.WriteKey | RegistryRights.QueryValues | RegistryRights.SetValue
                    true
                );
        }
        else
        {
            return Registry.CurrentUser.OpenSubKey(@"Environment", true);
        }
    }

    public static bool IsSamePath(string p1, string p2)
        => PathUtilities.PathsEqual(Environment.ExpandEnvironmentVariables(p1), Environment.ExpandEnvironmentVariables(p2));

    public static bool IsSameCmdStr(string s1, string s2)
        => string.Equals(s1, s2, StringComparison.OrdinalIgnoreCase);

    public static string[] GetEnvKeyValues(RegistryKey subKey, string envKey = "Path")
        => GetEnvKeyValue(subKey, envKey)?.Split(new[] { PathUtilities.SeparatorChar }, StringSplitOptions.None);

    public static string GetEnvKeyValue(RegistryKey subKey, string envKey = "Path")
        => subKey.GetValue(envKey, null, RegistryValueOptions.DoNotExpandEnvironmentNames)?.ToString();

    public static void AddVars(RegistryKey sub, params ValueTuple<string, IEnumerable<string>>[] envs) 
    {
        IEnumerable<ValueTuple<string, string>> f()
        {
            foreach (var e in envs)
            {
#if NetCore
                var (name, vals) = e;
#else 
                var name = e.Item1;
                var vals = e.Item2;
#endif

                var values = new List<string>();

                var kvs = GetEnvKeyValues(sub, name);
                if (kvs != null) values.AddRange(kvs);

                foreach (var v in vals)
                {
                    if (!values.Any(s => IsSamePath(s, v)))
                        values.Add(v);
                }

                yield return ValueTuple.Create(name, string.Join(PathUtilities.SeparatorStr, values));
            }
        }

        Win32Native.SetEnvironmentVariable(sub, f());
    }

    public static void RemoveVars(RegistryKey sub, params ValueTuple<string, IEnumerable<string>>[] envs) //(string name, IEnumerable<string> values)[]
    {
        IEnumerable<ValueTuple<string, string>> f()
        {
            foreach (var e in envs)
            {
#if NetCore
                var (name, vals) = e;
#else 
                var name = e.Item1;
                var vals = e.Item2;
#endif

                List<string> values = null;

                var kvs = GetEnvKeyValues(sub, name);
                if (kvs == null || kvs.Length == 0) continue;

                if (vals != null)
                {
                    values = new List<string>(kvs);
                    foreach (var v in vals)
                        values.RemoveAll(s => IsSamePath(s, v));
                }

                yield return ValueTuple.Create(name, values == null ? null : string.Join(PathUtilities.SeparatorStr, values));
            }
        }

        Win32Native.SetEnvironmentVariable(sub, f());
    }

    public static void SetOrUpdateVars(RegistryKey sub, params ValueTuple<string, IEnumerable<string>>[] envs)  //(string name, IEnumerable<string> values)[]
    {
        IEnumerable<ValueTuple<string, string>> f()
        {
            foreach (var e in envs)
            {
#if NetCore
                var (name, vals) = e;
#else 
                var name = e.Item1;
                var vals = e.Item2;
#endif

                if (vals == null) continue;

                yield return ValueTuple.Create(name, string.Join(PathUtilities.SeparatorStr, vals));
            }
        }

        Win32Native.SetEnvironmentVariable(sub, f());
    }

    [SecurityCritical]
#if !NetCore
    [SuppressUnmanagedCodeSecurity]
#endif
    static class Win32Native
    {
        private const string str_SysVarRegistryKey = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Environment";
        private const string str_CurrentVarRegistryKey = @"HKEY_CURRENT_USER\Environment";

        const int HWND_BROADCAST = 0xffff;
        const int WM_SETTINGCHANGE = 0x001A;
        const string _env = "Environment";

        [Flags]
        public enum SendMessageTimeoutFlags : uint
        {
            SMTO_NORMAL = 0x0000,
            SMTO_BLOCK = 0x0001,
            SMTO_ABORTIFHUNG = 0x0002,
            SMTO_NOTIMEOUTIFNOTHUNG = 0x0008
        }

        [DllImport("user32.dll", BestFitMapping = false, CharSet = CharSet.Ansi, ExactSpelling = false, SetLastError = true)]
        private static extern IntPtr SendMessageTimeout(IntPtr hWnd, int Msg, IntPtr wParam, string lParam, SendMessageTimeoutFlags uTimeout, uint fuFlags, out IntPtr lpdwResult);

        public static void RefreshEnvironmentVariable()
        {
            SendMessageTimeout(new IntPtr(HWND_BROADCAST), WM_SETTINGCHANGE, IntPtr.Zero, _env, SendMessageTimeoutFlags.SMTO_ABORTIFHUNG, 200u, out var r);
        }

        [SecuritySafeCritical]
        public static void SetEnvironmentVariable(RegistryKey sub, IEnumerable<ValueTuple<string, string>> envVars) //IEnumerable<(string name, string value)>
        {
            var target = sub.ToString();
            if (target != str_SysVarRegistryKey && target != str_CurrentVarRegistryKey)
            {
                throw new InvalidOperationException("RegistryKey only work on system or current user Environments");
            }
            foreach (var e in envVars)
            {
#if NetCore
                var (name, value) = e;
#else 
                var name = e.Item1;
                var value = e.Item2;
#endif

                CheckEnvironmentVariableName(name);
#if !NetCore
                new EnvironmentPermission(PermissionState.Unrestricted).Demand();
#endif
                if (value == null || (value.Length > 0 && value[0] == '\0'))
                {
                    value = null;
                }
                if (target == str_CurrentVarRegistryKey)
                {
                    if (name.Length >= 255)
                    {
                        throw new ArgumentException("Argument is LongEnvVarValue");
                    }
                }
                if (sub != null)
                {
                    if (value == null)
                        sub.DeleteValue(name, false);
                    else
                        sub.SetValue(name, value, value.IndexOf('%') > -1 ? RegistryValueKind.ExpandString : RegistryValueKind.String);
                }
            }
            RefreshEnvironmentVariable();
        }

        private static void CheckEnvironmentVariableName(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("variable");
            }
            if (name.Length == 0)
            {
                throw new ArgumentException("variable Argument is zero length");
            }
            if (name[0] == '\0')
            {
                throw new ArgumentException(@"variable Argument first char is '\0'");
            }
            if (name.Length >= 32767)
            {
                throw new ArgumentException("Argument is LongEnvVarValue");
            }
            if (name.IndexOf('=') != -1)
            {
                throw new ArgumentException("Argument IllegalEnvVarName");
            }
        }
    }
}