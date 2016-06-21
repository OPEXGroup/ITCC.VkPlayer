using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using ITCC.Logging;
using ITCC.UI.Windows;
using ITCC.VkPlayer.Interfaces;
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
        public static readonly ApplicationContext Context = new ApplicationContext();
        private static ILongTaskRunner _currentRunner;
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

        public static void ClosingHandler(object sender, CancelEventArgs args)
        {
            Current.Windows.OfType<LogWindow>().SingleOrDefault()?.Close();
        }

        public static void GoToWindow<TWindow>(Window current, Action<TWindow> prepareAction = null, bool preserveSize = false)
            where TWindow : Window, ILongTaskRunner, new()
        {
            var window = new TWindow();
            window.Top = current.Top + (current.Height - window.Height) / 2;
            window.Left = current.Left + (current.Width - window.Width) / 2;
            prepareAction?.Invoke(window);
            if (preserveSize)
            {
                window.Width = current.ActualHeight;
                window.Width = current.ActualWidth;
            }
            window.Show();
            window.Closing += ClosingHandler;
            _currentRunner = window;
            current.Closing -= ClosingHandler;
            current.Close();
            LogMessage(LogLevel.Debug, $"Went to window {typeof(TWindow).Name}");
        }

        public static void LoadSingletonWindow<TWindow>(Window current)
            where TWindow : Window, new()
        {
            var singletonWindow = Current.Windows.OfType<TWindow>().SingleOrDefault();
            if (singletonWindow != null)
            {
                singletonWindow.Activate();
                return;
            }
            singletonWindow = new TWindow();
            singletonWindow.Top = current.Top + (current.Height - singletonWindow.Height) / 2;
            singletonWindow.Left = current.Left + (current.Width - singletonWindow.Width) / 2;
            singletonWindow.Show();
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
