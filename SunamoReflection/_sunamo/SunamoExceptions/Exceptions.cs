namespace SunamoReflection._sunamo.SunamoExceptions;

internal sealed partial class Exceptions
{
    internal static string CheckBefore(string before)
    {
        return string.IsNullOrWhiteSpace(before) ? string.Empty : before + ": ";
    }

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

    internal static void TypeAndMethodName(string stackTraceLine, out string typeName, out string methodName)
    {
        var afterAt = stackTraceLine.Split("at ")[1].Trim();
        var fullMethodPath = afterAt.Split("(")[0];
        var segments = fullMethodPath.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        methodName = segments[^1];
        segments.RemoveAt(segments.Count - 1);
        typeName = string.Join(".", segments);
    }

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

    internal static string? NotImplementedMethod(string before)
    {
        return CheckBefore(before) + "Not implemented method.";
    }

    internal static string? IsNull(string before, string variableName, object? variable)
    {
        return variable == null ? CheckBefore(before) + variableName + " " + "is null" + "." : null;
    }

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
