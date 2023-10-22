using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace GameFinder.Common;

/// <summary>
/// Represents an aggregated exception.
/// </summary>
[PublicAPI]
public sealed class ExceptionCollectorAggregatedException : Exception
{
    private ExceptionCollectorAggregatedException(string msg) : base(msg) { }

    internal static ExceptionCollectorAggregatedException Create(List<Exception> exceptions)
    {
        var sb = new StringBuilder();
        foreach (var exception in exceptions)
        {
            sb.Append(exception);
            sb.AppendLine();
        }

        return new ExceptionCollectorAggregatedException(sb.ToString());
    }
}
