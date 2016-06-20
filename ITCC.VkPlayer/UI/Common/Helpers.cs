using System;
using System.Windows;
using ITCC.Logging;

namespace ITCC.VkPlayer.UI.Common
{
    internal static class Helpers
    {
        public static MessageBoxResult Ask(string question)
        {
            return MessageBox.Show(question, "Клиент", MessageBoxButton.YesNo, MessageBoxImage.Question,
                MessageBoxResult.No);
        }

        public static bool AskBinary(string question) => Ask(question) == MessageBoxResult.Yes;

        public static void DoWithConfirmation(string question, Action action)
        {
            if (Ask(question) == MessageBoxResult.Yes)
                action.Invoke();
        }

        public static void ShowNotImplemented()
        {
            Logger.LogEntry("INTERFACE", LogLevel.Debug, $"User got `Not implemented` message");
            MessageBox.Show("Данный раздел находится в разработке :(", "Клиент", MessageBoxButton.OK,
                MessageBoxImage.Question);
        }

        public static void ShowInfo(string message)
        {
            Logger.LogEntry("INTERFACE", LogLevel.Debug, $"User got info `{message}`");
            MessageBox.Show(message, "Клиент", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void ShowWarning(string message)
        {
            Logger.LogEntry("INTERFACE", LogLevel.Debug, $"User got warning `{message}`");
            MessageBox.Show(message, "Клиент", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}