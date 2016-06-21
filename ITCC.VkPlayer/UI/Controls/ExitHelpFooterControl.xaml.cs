using System.Windows;
using System.Windows.Controls;
using ITCC.VkPlayer.UI.Common;

namespace ITCC.VkPlayer.UI.Controls
{
    /// <summary>
    ///     Interaction logic for ExitHelpFooterControl.xaml
    /// </summary>
    public partial class ExitHelpFooterControl : UserControl
    {
        public ExitHelpFooterControl()
        {
            InitializeComponent();
        }

        private void ExitButton_OnClick(object sender, RoutedEventArgs e)
        {
            Helpers.DoWithConfirmation("Вы действительно хотите выйти?", Application.Current.Shutdown);
        }

        private void HelpButton_OnClick(object sender, RoutedEventArgs e)
        {
        }
    }
}