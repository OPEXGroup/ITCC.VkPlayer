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

                AppId = Convert.ToUInt64(appSettings["AppId"]);
                Username = appSettings["Username"];
                Password = appSettings["Password"];

                LogLevel = (LogLevel)Enum.Parse(typeof(LogLevel), appSettings["LogLevel"]);
                LoggerMode = (LoggerMode)Enum.Parse(typeof(LoggerMode), appSettings["LoggerMode"]);
                MessageLevel = (LogLevel)Enum.Parse(typeof(LogLevel), appSettings["MessageLevel"]);
                BannedLoggerScopes = new List<string>(ConfigurationManager.AppSettings["BannedLoggerScopes"].Split(';'));
                LogSavingPeriod = Convert.ToDouble(ConfigurationManager.AppSettings["LogSavingPeriod"]);
                MaxWindowEntries = Convert.ToInt32(ConfigurationManager.AppSettings["MaxWindowEntries"]);

                MusicCacheFolder = appSettings["MusicCacheFolder"];
                KeepCache = Convert.ToBoolean(appSettings["KeepCache"]);

                LogDirectory = Environment.CurrentDirectory + "\\Log";

                if (!Directory.Exists(LogDirectory))
                {
                    Directory.CreateDirectory(LogDirectory);
                }
                if (!Directory.Exists(MusicCacheFolder))
                {
                    Directory.CreateDirectory(MusicCacheFolder);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error reading application configuration", ex);
            }
        }

        #region authorizarion
        public static ulong AppId { get; private set; }

        public static string Username { get; private set; }

        public static string Password { get; private set; }
        #endregion

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
        public static string MusicCacheFolder { get; private set; }

        public static bool KeepCache { get; private set; }
        #endregion
    }
}