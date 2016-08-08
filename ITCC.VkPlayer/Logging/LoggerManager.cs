using System;
using ITCC.Logging;
using ITCC.Logging.Loggers;
using ITCC.UI.Loggers;
using ITCC.VkPlayer.Enums;
using ITCC.VkPlayer.Utils;

namespace ITCC.VkPlayer.Logging
{
    internal static class LoggerManager
    {
        #region public

        public static void StartLoggers()
        {
            Logger.Level = Configuration.LogLevel;
            Logger.AddBannedScopeRange(Configuration.BannedLoggerScopes);

            if (Configuration.LoggerMode.HasFlag(LoggerMode.File))
            {
                var logFileName = Configuration.LogDirectory + @"\" + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") +
                                  ".txt";
                _fileLogger = new BufferedFileLogger(logFileName, false, Configuration.LogSavingPeriod);
                Logger.RegisterReceiver(_fileLogger);
            }

            if (Configuration.LoggerMode.HasFlag(LoggerMode.Message))
            {
                _messageLogger = new MessageLogger { Level = Configuration.MessageLevel };
                Logger.RegisterReceiver(_messageLogger);
            }

            if (Configuration.LoggerMode.HasFlag(LoggerMode.Window))
            {
                ObservableLogger = new ObservableLogger(Configuration.MaxWindowEntries, App.RunOnUiThread) { Level = Logger.Level };
                Logger.RegisterReceiver(ObservableLogger);
            }
        }

        public static void FinalizeLoggers()
        {
            App.LogMessage(LogLevel.Info, "Приложение завершено");
            _fileLogger?.Flush().Wait();
        }

        public static void DoNotShowMessageBoxes()
        {
            Logger.UnregisterReceiver(_messageLogger);
        }

        public static void ResumeMessageBoxes()
        {
            Logger.RegisterReceiver(_messageLogger);
        }

        public static string LogfileName()
        {
            return _fileLogger?.Filename;
        }

        #endregion

        #region private

        private static BufferedFileLogger _fileLogger;

        private static MessageLogger _messageLogger;

        public static ObservableLogger ObservableLogger;

        #endregion
    }
}