namespace SunamoReflection._sunamo.SunamoExceptions;

/// <summary>
/// Provides exception message formatting and stack trace analysis utilities.
/// </summary>
internal sealed partial class Exceptions
{
    /// <summary>
    /// Returns the before prefix formatted for exception messages.
    /// </summary>
    /// <param name="before">The prefix text, or null/empty for no prefix.</param>
    /// <returns>Formatted prefix string ending with ": " or empty string.</returns>
    internal static string CheckBefore(string before)
    {
        return string.IsNullOrWhiteSpace(before) ? string.Empty : before + ": ";
    }

    /// <summary>
    /// Extracts a readable text from an exception, optionally including inner exceptions.
    /// </summary>
    /// <param name="exception">The exception to extract text from.</param>
    /// <param name="isIncludingInner">Whether to include inner exception messages.</param>
    /// <returns>Formatted exception text.</returns>
    internal static string TextOfExceptions(Exception exception, bool isIncludingInner = true)
    {
        if (exception == null) return string.Empty;
        StringBuilder stringBuilder = new();
        stringBuilder.Append("Exception:");
        stringBuilder.AppendLine(exception.Message);
        if (isIncludingInner)
            while (exception.InnerException != null)
            {
                exception = exception.InnerException;
                stringBuilder.AppendLine(exception.Message);
            }
        var result = stringBuilder.ToString();
        return result;
    }

    /// <summary>
    /// Analyzes the current stack trace to determine the place where an exception originated.
    /// </summary>
    /// <param name="isFillAlsoFirstTwo">Whether to also extract type and method name from the first non-ThrowEx frame.</param>
    /// <returns>Tuple of (typeName, methodName, fullStackTrace).</returns>
    internal static Tuple<string, string, string> PlaceOfException(bool isFillAlsoFirstTwo = true)
    {
        StackTrace stackTrace = new();
        var stackTraceText = stackTrace.ToString();
        var lines = stackTraceText.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
        lines.RemoveAt(0);
        var index = 0;
        string typeName = string.Empty;
        string methodName = string.Empty;
        for (; index < lines.Count; index++)
        {
            var line = lines[index];
            if (isFillAlsoFirstTwo)
                if (!line.StartsWith("   at ThrowEx"))
                {
                    TypeAndMethodName(line, out typeName, out methodName);
                    isFillAlsoFirstTwo = false;
                }
            if (line.StartsWith("at System."))
            {
                lines.Add(string.Empty);
                lines.Add(string.Empty);
                break;
            }
        }
        return new Tuple<string, string, string>(typeName, methodName, string.Join(Environment.NewLine, lines));
    }

    /// <summary>
    /// Extracts the type name and method name from a stack trace line.
    /// </summary>
    /// <param name="stackTraceLine">A single line from a stack trace.</param>
    /// <param name="typeName">The extracted type name.</param>
    /// <param name="methodName">The extracted method name.</param>
    internal static void TypeAndMethodName(string stackTraceLine, out string typeName, out string methodName)
    {
        var afterAt = stackTraceLine.Split("at ")[1].Trim();
        var fullMethodPath = afterAt.Split("(")[0];
        var segments = fullMethodPath.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        methodName = segments[^1];
        segments.RemoveAt(segments.Count - 1);
        typeName = string.Join(".", segments);
    }

    /// <summary>
    /// Gets the name of the calling method at the specified stack depth.
    /// </summary>
    /// <param name="depth">The stack frame depth to inspect.</param>
    /// <returns>The name of the calling method.</returns>
    internal static string CallingMethod(int depth = 1)
    {
        StackTrace stackTrace = new();
        var methodBase = stackTrace.GetFrame(depth)?.GetMethod();
        if (methodBase == null)
        {
            return "Method name cannot be get";
        }
        var methodName = methodBase.Name;
        return methodName;
    }

    /// <summary>
    /// Returns a formatted "not implemented method" error message.
    /// </summary>
    /// <param name="before">The prefix for the error message.</param>
    /// <returns>Formatted error message.</returns>
    internal static string? NotImplementedMethod(string before)
    {
        return CheckBefore(before) + "Not implemented method.";
    }

    /// <summary>
    /// Returns a formatted "is null" error message if the variable is null.
    /// </summary>
    /// <param name="before">The prefix for the error message.</param>
    /// <param name="variableName">The name of the variable being checked.</param>
    /// <param name="variable">The variable to check for null.</param>
    /// <returns>Formatted error message if null, otherwise null.</returns>
    internal static string? IsNull(string before, string variableName, object? variable)
    {
        return variable == null ? CheckBefore(before) + variableName + " " + "is null" + "." : null;
    }

    /// <summary>
    /// Returns a formatted "not implemented case" error message.
    /// </summary>
    /// <param name="before">The prefix for the error message.</param>
    /// <param name="notImplementedName">The name or type of the unimplemented case.</param>
    /// <returns>Formatted error message.</returns>
    internal static string? NotImplementedCase(string before, object notImplementedName)
    {
        var forClause = string.Empty;
        if (notImplementedName != null)
        {
            forClause = " for ";
            if (notImplementedName.GetType() == typeof(Type))
                forClause += ((Type)notImplementedName).FullName;
            else
                forClause += notImplementedName.ToString();
        }
        return CheckBefore(before) + "Not implemented case" + forClause + " . internal program error. Please contact developer" +
        ".";
    }
}
