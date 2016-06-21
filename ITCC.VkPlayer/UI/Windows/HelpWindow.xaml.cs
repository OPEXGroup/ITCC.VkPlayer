using System;
using System.Linq;
using System.Windows;
using ITCC.Logging;
using ITCC.UI.Windows;
using ITCC.VkPlayer.Logging;

namespace ITCC.VkPlayer.UI.Windows
{
    /// <summary>
    ///     Interaction logic for HelpWindow.xaml
    /// </summary>
    public partial class HelpWindow : Window
    {
        public HelpWindow()
        {
            InitializeComponent();
            Topmost = true;
            App.LogMessage(LogLevel.Debug, "Help window opened");
        }

        private void HelpWindow_OnClosed(object sender, EventArgs e)
        {
            App.LogMessage(LogLevel.Debug, "Help window closed");
        }

        private void OpenLogWindowButton_OnClick(object sender, RoutedEventArgs e)
        {
            var singletonWindow = Application.Current.Windows.OfType<LogWindow>().SingleOrDefault();
            if (singletonWindow != null)
            {
                singletonWindow.Activate();
                return;
            }
            singletonWindow = new LogWindow(LoggerManager.ObservableLogger);
            singletonWindow.Show();
        }

        private void CloseHelpButton_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}