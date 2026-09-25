namespace SunamoReflection._sunamo.SunamoExceptions;

/// <summary>
/// Provides methods that throw exceptions with detailed context about the calling code location.
/// </summary>
internal partial class ThrowEx
{
    /// <summary>
    /// Throws if the specified variable is null.
    /// </summary>
    /// <param name="variableName">The name of the variable.</param>
    /// <param name="variable">The variable to check.</param>
    /// <returns>True if the variable was null and an exception was thrown.</returns>
    internal static bool IsNull(string variableName, object? variable = null)
    { return ThrowIsNotNull(Exceptions.IsNull(FullNameOfExecutedCode(), variableName, variable)); }

    /// <summary>
    /// Throws for an unimplemented switch/case value.
    /// </summary>
    /// <param name="notImplementedName">The unimplemented case value.</param>
    /// <returns>True if an exception was thrown.</returns>
    internal static bool NotImplementedCase(object notImplementedName)
    { return ThrowIsNotNull(Exceptions.NotImplementedCase, notImplementedName); }

    /// <summary>
    /// Throws for a method that is not yet implemented.
    /// </summary>
    /// <returns>True if an exception was thrown.</returns>
    internal static bool NotImplementedMethod() { return ThrowIsNotNull(Exceptions.NotImplementedMethod); }

    /// <summary>
    /// Gets the full name of the currently executed code location from the stack trace.
    /// </summary>
    /// <returns>Full name in format "Namespace.Type.Method".</returns>
    internal static string FullNameOfExecutedCode()
    {
        Tuple<string, string, string> placeOfException = Exceptions.PlaceOfException();
        string fullName = FullNameOfExecutedCode(placeOfException.Item1, placeOfException.Item2, true);
        return fullName;
    }

    /// <summary>
    /// Resolves the full name of executed code from a type and method name.
    /// </summary>
    /// <param name="type">The type (can be Type, MethodBase, or string).</param>
    /// <param name="methodName">The method name.</param>
    /// <param name="isFromThrowEx">Whether the call originates from ThrowEx (adjusts stack depth).</param>
    /// <returns>Full name in format "TypeFullName.MethodName".</returns>
    static string FullNameOfExecutedCode(object type, string methodName, bool isFromThrowEx = false)
    {
        if (methodName == null)
        {
            int depth = 2;
            if (isFromThrowEx)
            {
                depth++;
            }

            methodName = Exceptions.CallingMethod(depth);
        }
        string typeFullName;
        if (type is Type typeValue)
        {
            typeFullName = typeValue.FullName ?? "Type cannot be get via type is Type type2";
        }
        else if (type is MethodBase method)
        {
            typeFullName = method.ReflectedType?.FullName ?? "Type cannot be get via type is MethodBase method";
            methodName = method.Name;
        }
        else if (type is string)
        {
            typeFullName = type.ToString() ?? "Type cannot be get via type is string";
        }
        else
        {
            Type objectType = type.GetType();
            typeFullName = objectType.FullName ?? "Type cannot be get via type.GetType()";
        }
        return string.Concat(typeFullName, ".", methodName);
    }

    /// <summary>
    /// Throws an exception if the exception message is not null.
    /// </summary>
    /// <param name="exception">The exception message to evaluate.</param>
    /// <param name="isReallyThrowing">Whether to actually throw or just return true.</param>
    /// <returns>True if the exception message was not null.</returns>
    internal static bool ThrowIsNotNull(string? exception, bool isReallyThrowing = true)
    {
        if (exception != null)
        {
            Debugger.Break();
            if (isReallyThrowing)
            {
                throw new Exception(exception);
            }
            return true;
        }
        return false;
    }

    /// <summary>
    /// Evaluates a function with a before-context and argument, then throws if result is not null.
    /// </summary>
    /// <typeparam name="TArgument">The type of the argument.</typeparam>
    /// <param name="exceptionFactory">Function that produces an exception message.</param>
    /// <param name="argument">The argument to pass to the factory.</param>
    /// <returns>True if an exception was thrown.</returns>
    internal static bool ThrowIsNotNull<TArgument>(Func<string, TArgument, string?> exceptionFactory, TArgument argument)
    {
        string? exception = exceptionFactory(FullNameOfExecutedCode(), argument);
        return ThrowIsNotNull(exception);
    }

    /// <summary>
    /// Evaluates a function with a before-context, then throws if result is not null.
    /// </summary>
    /// <param name="exceptionFactory">Function that produces an exception message.</param>
    /// <returns>True if an exception was thrown.</returns>
    internal static bool ThrowIsNotNull(Func<string, string?> exceptionFactory)
    {
        string? exception = exceptionFactory(FullNameOfExecutedCode());
        return ThrowIsNotNull(exception);
    }
}
