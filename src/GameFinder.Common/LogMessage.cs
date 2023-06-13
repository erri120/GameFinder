using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace GameFinder.Common;

/// <summary>
/// Defines available message levels corresponding to LogLevels from NLog or Microsoft.Extensions.Logging.
/// </summary>
/// <remarks>
/// Log levels ordered by severity:<br/>
/// - <see cref="MessageLevel.Trace"/> (Ordinal = 0) : Most verbose level. Used for development and seldom enabled in production.<br/>
/// - <see cref="MessageLevel.Debug"/> (Ordinal = 1) : Debugging the application behavior from internal events of interest.<br/>
/// - <see cref="MessageLevel.Info"/>  (Ordinal = 2) : Information that highlights progress or application lifetime events.<br/>
/// - <see cref="MessageLevel.Warn"/>  (Ordinal = 3) : Warnings about validation issues or temporary failures that can be recovered.<br/>
/// - <see cref="MessageLevel.Error"/> (Ordinal = 4) : Errors where functionality has failed or <see cref="System.Exception"/> have been caught.<br/>
/// - <see cref="MessageLevel.Fatal"/> (Ordinal = 5) : Most critical level. Application is about to abort.<br/>
/// - <see cref="MessageLevel.None"/>  (Ordinal = 6) : Off log level<br/>
/// </remarks>
public enum MessageLevel
{
    /// <summary>
    /// Most verbose level. Used for development and seldom enabled in production.
    /// </summary>
    Trace,
    /// <summary>
    /// Debugging the application behavior from internal events of interest. 
    /// </summary>
    Debug,
    /// <summary>
    /// Information that highlights progress or application lifetime events.
    /// </summary>
    Info,
    /// <summary>
    /// Warnings about validation issues or temporary failures that can be recovered.
    /// </summary>
    Warn,
    /// <summary>
    /// Errors where functionality has failed or <see cref="System.Exception"/> have been caught.
    /// </summary>
    Error,
    /// <summary>
    /// Most critical level. Application is about to abort.
    /// </summary>
    Fatal,
    /// <summary>
    /// Off log level (Ordinal = 6)
    /// </summary>
    None
}

/// <summary>
/// Represents a generic logging message (defaults to <see cref="MessageLevel.Error"/>).
/// </summary>
[PublicAPI]
[DebuggerDisplay("{Message}")]
public readonly struct LogMessage
{
    /// <summary>
    /// The message.
    /// </summary>
    public readonly string Message;

    /// <summary>
    /// The log level (Debug, Info, Warn, Error, etc.).
    /// </summary>
    public readonly MessageLevel Level;

    /// <summary>
    /// Constructor taking a message.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="msgLevel"></param>
    public LogMessage(string message, MessageLevel msgLevel = MessageLevel.Error)
    {
        Message = message;
        Level = msgLevel;
    }

    /// <summary>
    /// Constructor taking an exception.
    /// </summary>
    /// <param name="e"></param>
    /// <param name="msgLevel"></param>
    public LogMessage(Exception e, MessageLevel msgLevel = MessageLevel.Error)
    {
        Message = e.ToString();
        Level = msgLevel;
    }

    /// <summary>
    /// Constructor taking an exception and a message.
    /// </summary>
    /// <param name="e"></param>
    /// <param name="message"></param>
    /// <param name="msgLevel"></param>
    public LogMessage(Exception e, string message, MessageLevel msgLevel = MessageLevel.Error)
    {
        Message = $"{message}:\n{e}";
        Level = msgLevel;
    }

    /// <inheritdoc/>
    public override string ToString() => Message;

    /// <summary>
    /// Converts <see cref="LogMessage"/> to a <see cref="string"/>.
    /// </summary>
    public static explicit operator string(LogMessage message) => message.Message;

    /// <summary>
    /// Creates a new <see cref="LogMessage"/> from a <see cref="string"/>.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static implicit operator LogMessage(string message) => new(message);

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj switch
        {
            null => false,
            string s => string.Equals(Message, s, StringComparison.InvariantCulture),
            LogMessage logMessage => string.Equals(Message, logMessage.Message, StringComparison.InvariantCulture),
            _ => false
        };
    }

    /// <inheritdoc/>
    public override int GetHashCode() => Message.GetHashCode(StringComparison.InvariantCulture);
}
