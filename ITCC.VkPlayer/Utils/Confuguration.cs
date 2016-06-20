using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using ITCC.Logging;
using ITCC.VkPlayer.Enums;

namespace ITCC.VkPlayer.Utils
{
    internal static class Configuration
    {
        public static void ReadAppConfig()
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;

                LogLevel = (LogLevel)Enum.Parse(typeof(LogLevel), appSettings["LogLevel"]);
                LoggerMode = (LoggerMode)Enum.Parse(typeof(LoggerMode), appSettings["LoggerMode"]);
                MessageLevel = (LogLevel)Enum.Parse(typeof(LogLevel), appSettings["MessageLevel"]);
                BannedLoggerScopes = new List<string>(ConfigurationManager.AppSettings["BannedLoggerScopes"].Split(';'));
                LogSavingPeriod = Convert.ToDouble(ConfigurationManager.AppSettings["LogSavingPeriod"]);
                MaxWindowEntries = Convert.ToInt32(ConfigurationManager.AppSettings["MaxWindowEntries"]);

                LogDirectory = Environment.CurrentDirectory + "\\Log";

                if (!Directory.Exists(LogDirectory))
                {
                    Directory.CreateDirectory(LogDirectory);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error reading application configuration", ex);
            }
        }

        #region log

        public static LogLevel LogLevel { get; private set; }

        public static LoggerMode LoggerMode { get; private set; }

        public static LogLevel MessageLevel { get; private set; }

        public static List<string> BannedLoggerScopes { get; private set; }

        public static string LogDirectory { get; private set; }

        public static double LogSavingPeriod { get; private set; }

        public static int MaxWindowEntries { get; private set; }

        #endregion

        #region utils

        #endregion
    }
}