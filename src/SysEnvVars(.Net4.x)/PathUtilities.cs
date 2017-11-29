using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

static class PlatformInformation
{
    public static bool IsWindows => Path.DirectorySeparatorChar == '\\';
    public static bool IsUnix => Path.DirectorySeparatorChar == '/';
}

static class PathUtilities
{
    internal static readonly char DirectorySeparatorChar = PlatformInformation.IsUnix ? '/' : '\\';
    internal static readonly char AltDirectorySeparatorChar = '/';
    //internal static readonly string ParentRelativeDirectory = "..";
    //internal static readonly string ThisDirectory = ".";
    //internal static readonly string DirectorySeparatorStr = new string(DirectorySeparatorChar, 1);
    internal static readonly char SeparatorChar = Path.PathSeparator;
    internal static readonly string SeparatorStr = new string(Path.PathSeparator, 1);
    //internal const char VolumeSeparatorChar = ':';

    public static bool IsDirectorySeparator(char c) => c == DirectorySeparatorChar || c == AltDirectorySeparatorChar;

    public static string TrimTrailingSeparators(string s)
    {
        var lastSeparator = s.Length;
        while (lastSeparator > 0 && IsDirectorySeparator(s[lastSeparator - 1]))
        {
            lastSeparator = lastSeparator - 1;
        }
        if (lastSeparator != s.Length)
        {
            s = s.Substring(0, lastSeparator);
        }
        return s;
    }

    private static string RemoveTrailingDirectorySeparator(string path)
    {
        if (path.Length > 0 && IsDirectorySeparator(path[path.Length - 1]))
        {
            return path.Substring(0, path.Length - 1);
        }
        else
        {
            return path;
        }
    }

    public static bool PathsEqual(string path1, string path2)
    {
        if (path1 == null && path2 == null)
            return true;
        if (path1 == null || path2 == null)
            return false;

        path1 = TrimTrailingSeparators(path1);
        path2 = TrimTrailingSeparators(path2);
        var length = Math.Max(path1.Length, path2.Length);

        if (path1.Length < length || path2.Length < length)
            return false;

        for (var i = 0; i < length; i++)
        {
            if (!PathCharEqual(path1[i], path2[i]))
                return false;
        }

        return true;
    }

    private static bool PathCharEqual(char x, char y)
    {
        if (IsDirectorySeparator(x) && IsDirectorySeparator(y))
            return true;

        return PlatformInformation.IsUnix
            ? x == y
            : char.ToUpperInvariant(x) == char.ToUpperInvariant(y);
    }
}