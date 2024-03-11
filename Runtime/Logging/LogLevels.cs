namespace MoonriseGames.CloudsAhoyConnect.Logging
{
    /// <summary>The different log levels at which log messages can be printed.</summary>
    public enum LogLevels
    {
        /// <summary>The message contains information relevant for debugging.</summary>
        Debug = 0,

        /// <summary>The message contains general information about the application state.</summary>
        Info = 1,

        /// <summary>The message contains warnings that should be addressed and might lead to errors.</summary>
        Warn = 2,

        /// <summary>The message contains errors that indicate an illegal application state which should not occur under normal operation.</summary>
        Error = 3,

        /// <summary>The message should be ignored and not printed to the log.</summary>
        None = 4
    }
}
