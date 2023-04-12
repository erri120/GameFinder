using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace GameFinder.Common;

/// <summary>
/// Utilities.
/// </summary>
public static class Utils
{
    /// <summary>
    /// Sanitizes the given path.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="PlatformNotSupportedException"></exception>
    public static string SanitizeInputPath(string input)
    {
        var sb = new StringBuilder(input.Trim());

        char currentDirectorySeparator;
        char otherDirectorySeparator;
        if (OperatingSystem.IsLinux())
        {
            currentDirectorySeparator = '/';
            otherDirectorySeparator = '\\';
        }
        else if (OperatingSystem.IsWindows())
        {
            currentDirectorySeparator = '\\';
            otherDirectorySeparator = '/';
        }
        else
        {
            throw new PlatformNotSupportedException();
        }

        sb.Replace(otherDirectorySeparator, currentDirectorySeparator);
        sb.Replace(
            $"{currentDirectorySeparator}{currentDirectorySeparator}",
            $"{currentDirectorySeparator}"
        );

        int i;
        for (i = sb.Length - 1; i > 0; i--)
        {
            var c = sb[i];
            if (c != currentDirectorySeparator) break;
        }

        var rootLength = GetRootLength(sb, currentDirectorySeparator);
        if (rootLength == 0 || rootLength != i + 1) return sb.ToString(0, i + 1);
        return sb.ToString(0, rootLength);
    }

    private static int GetRootLength(StringBuilder sb, char directorySeparator)
    {
        if (OperatingSystem.IsLinux())
            return sb.Length >= 1 && sb[0] == directorySeparator ? 1 : 0;

        if (!OperatingSystem.IsWindows())
            throw new PlatformNotSupportedException();

        if (sb.Length < 3 || sb[1] != ':' || !IsValidDriveChar(sb[0])) return 0;
        return sb[2] == directorySeparator ? 3 : 0;
    }

    /// <summary>
    /// Returns true if the given character is a valid drive letter.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsValidDriveChar(char value)
    {
        // Licensed to the .NET Foundation under one or more agreements.
        // The .NET Foundation licenses this file to you under the MIT license.
        // https://github.com/dotnet/runtime/blob/main/LICENSE.TXT
        // source: https://github.com/dotnet/runtime/blob/d9f453924f7c3cca9f02d920a57e1477293f216e/src/libraries/Common/src/System/IO/PathInternal.Windows.cs#L69-L75
        return (uint)((value | 0x20) - 'a') <= 'z' - 'a';
    }
}
