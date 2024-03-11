using UnityEngine;

namespace MoonriseGames.CloudsAhoyConnect.Logging
{
    /// <summary>Logging interface used for all network related logs. The log fidelity can be adjusted by setting the <see cref="LogLevel" />.</summary>
    public static class NetworkLogger
    {
        private const string COLOR_DEBUG = "#ffffff";
        private const string COLOR_INFO = "#66ffff";
        private const string COLOR_WARN = "#ffff99";
        private const string COLOR_ERROR = "#ff9999";

        /// <summary>The log level the logger is currently set to. To silence all logs set the log level to <see cref="LogLevels.None" />.</summary>
        public static LogLevels LogLevel { get; set; } = LogLevels.Debug;

        private static bool IsDevelopmentBuild => Application.isEditor || UnityEngine.Debug.isDebugBuild;

        /// <summary>Prints a log message to the console.</summary>
        /// <param name="message">The message or object to be logged.</param>
        /// <param name="logLevel">The log level at which the message is logged.</param>
        public static void Log(object message, LogLevels logLevel) => PrintMessage(logLevel, message);

        /// <summary>Prints a log message at the <see cref="LogLevels.Debug" /> to the console.</summary>
        /// <param name="message">The message or object to be logged.</param>
        public static void Debug(object message) => PrintMessage(LogLevels.Debug, message);

        /// <summary>Prints a log message at the <see cref="LogLevels.Info" /> to the console.</summary>
        /// <param name="message">The message or object to be logged.</param>
        public static void Info(object message) => PrintMessage(LogLevels.Info, message);

        /// <summary>Prints a log message at the <see cref="LogLevels.Warn" /> to the console.</summary>
        /// <param name="message">The message or object to be logged.</param>
        public static void Warn(object message) => PrintMessage(LogLevels.Warn, message);

        /// <summary>Prints a log message at the <see cref="LogLevels.Error" /> to the console.</summary>
        /// <param name="message">The message or object to be logged.</param>
        public static void Error(object message) => PrintMessage(LogLevels.Error, message);

        private static void PrintMessage(LogLevels logLevel, object message)
        {
            if (!IsDevelopmentBuild || !ShouldPrintMessage(logLevel))
                return;
            var log = "Clouds Ahoy Connect: " + (message ?? "null");
            UnityEngine.Debug.Log(ColorizedLogMessage(log, logLevel));
        }

        private static bool ShouldPrintMessage(LogLevels messageLogLevel) => (int)LogLevel <= (int)messageLogLevel;

        private static string ColorizedLogMessage(string message, LogLevels logLevel) => $"<color={LogLevelColorCode(logLevel)}>{message}</color>";

        private static string LogLevelColorCode(LogLevels logLevel) =>
            logLevel switch
            {
                LogLevels.Debug => COLOR_DEBUG,
                LogLevels.Info => COLOR_INFO,
                LogLevels.Warn => COLOR_WARN,
                LogLevels.Error => COLOR_ERROR,
                _ => string.Empty
            };
    }
}
