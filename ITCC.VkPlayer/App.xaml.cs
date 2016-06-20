using System;
using System.Windows;
using ITCC.Logging;
using ITCC.UI.Windows;
using ITCC.VkPlayer.Logging;
using ITCC.VkPlayer.UI.Common;
using ITCC.VkPlayer.Utils;

namespace ITCC.VkPlayer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            DispatcherUnhandledException += (sender, args) =>
            {
                LoggerManager.DoNotShowMessageBoxes();
                var errorMessage = "Работа приложения остановлена в связи с необработанной ошибкой.";
                var fileName = LoggerManager.LogfileName();
                var optionalWindowMessage = LoggerManager.ObservableLogger != null ? "в окне лога приложения и " : "";
                if (fileName != null && Configuration.LogLevel != LogLevel.None)
                    errorMessage += $"\nДетали ошибки описаны {optionalWindowMessage}в файле {fileName}";
                Helpers.ShowWarning(errorMessage);
                LogException(LogLevel.Critical, args.Exception);
                LoggerManager.ResumeMessageBoxes();
#if DEBUG
                args.Handled = true;
#else
                Current.Shutdown();
#endif
            };

            try
            {
                Configuration.ReadAppConfig();
                LoggerManager.StartLoggers();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.InnerException?.Message,
                    "Startup error", MessageBoxButton.OK, MessageBoxImage.Error);
                Current.Shutdown();
            }

#if DEBUG
            var logWindow = new LogWindow(LoggerManager.ObservableLogger);
            logWindow.Show();
#endif

            LogMessage(LogLevel.Info, "Приложение готово");
        }


        public static void RunOnUiThread(Action action)
        {
            Current.Dispatcher.Invoke(action);
        }

        private void App_OnExit(object sender, ExitEventArgs e)
        {
            LoggerManager.FinalizeLoggers();
        }

        #region log

        public static void LogMessage(LogLevel level, string message)
        {
            Logger.LogEntry("APPLICATION", level, message);
        }

        public static void LogException(LogLevel level, Exception exception)
        {
            Logger.LogException("APPLICATION", level, exception);
        }

        #endregion
    }
}
