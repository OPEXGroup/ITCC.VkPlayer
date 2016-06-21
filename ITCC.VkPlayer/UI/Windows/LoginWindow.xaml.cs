using System.Windows;
using ITCC.VkPlayer.Enums;
using ITCC.VkPlayer.Interfaces;
using ITCC.VkPlayer.UI.Common;
using ITCC.VkPlayer.Utils;

namespace ITCC.VkPlayer.UI.Windows
{
    /// <summary>
    ///     Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window, ILongTaskRunner
    {
        public LoginWindow()
        {
            InitializeComponent();
            CenterWindowOnScreen();

            UsernameTextBox.Text = Configuration.Username;
            PasswordPasswordBox.Password = Configuration.Password;
            Closing += App.ClosingHandler;
        }

        public void BeginOperation(string message)
        {
            InProgressControl.Start(message);
            MainGrid.IsEnabled = false;
        }

        public void EndOperation()
        {
            InProgressControl.Stop();
            MainGrid.IsEnabled = true;
        }

        public void CancelOperations()
        {
            App.Context.ApiRunner.CancelLogin();
            EndOperation();
        }

        private void CenterWindowOnScreen()
        {
            var screenWidth = SystemParameters.PrimaryScreenWidth;
            var screenHeight = SystemParameters.PrimaryScreenHeight;
            var windowWidth = Width;
            var windowHeight = Height;
            Left = screenWidth / 2 - windowWidth / 2;
            Top = screenHeight / 2 - windowHeight / 2;
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            BeginOperation("Выполняется вход");
            var result = await App.Context.ApiRunner.AuthorizeAsync(UsernameTextBox.Text, PasswordPasswordBox.Password);
            if (result != SimpleOperationStatus.Ok)
                Helpers.ShowWarning("Неудачная попытка входа");
            EndOperation();
        }
    }
}