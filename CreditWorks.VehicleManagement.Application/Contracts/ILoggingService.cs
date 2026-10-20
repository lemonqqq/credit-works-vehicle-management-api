namespace CreditWorks.VehicleManagement.Application.Contracts
{
    /// <summary>
    ///     Defines structured logging for a specified application class.
    /// </summary>
    /// <typeparam name="TClass">The class associated with the log entries.</typeparam>
    public interface ILoggingService<TClass> where TClass : class
    {
        /// <summary>
        ///     Writes a debug message.
        /// </summary>
        /// <param name="messageTemplate">The structured message template.</param>
        /// <param name="args">Values for the template placeholders.</param>
        void LogDebug(string messageTemplate, params object?[] args);

        /// <summary>
        ///     Writes a warning associated with an exception.
        /// </summary>
        /// <param name="ex">The exception to log.</param>
        /// <param name="messageTemplate">The structured message template.</param>
        /// <param name="args">Values for the template placeholders.</param>
        void LogWarning(Exception ex, string messageTemplate, params object?[] args);
        
        /// <summary>
        ///     Writes an informational message.
        /// </summary>
        /// <param name="messageTemplate">The structured message template.</param>
        /// <param name="args">Values for the template placeholders.</param>
        void LogInfo(string messageTemplate, params object?[] args);
        
        /// <summary>
        ///     Writes an error associated with an exception.
        /// </summary>
        /// <param name="ex">The exception to log.</param>
        /// <param name="messageTemplate">The structured message template.</param>
        /// <param name="args">Values for the template placeholders.</param>
        void LogError(Exception ex, string messageTemplate, params object?[] args);
    }
}
