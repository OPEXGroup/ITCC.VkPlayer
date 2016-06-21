using System.Windows;
using System.Windows.Controls;
using ITCC.VkPlayer.Interfaces;

namespace ITCC.VkPlayer.UI.Controls
{
    /// <summary>
    ///     Interaction logic for InProgressControl.xaml
    /// </summary>
    public partial class InProgressControl : UserControl
    {
        private string _busyText = "Операция выполняется";
        public ILongTaskRunner TaskRunner;

        public InProgressControl()
        {
            InitializeComponent();
            Panel.SetZIndex(this, 999);
            InProgressIndicator.DataContext = this;
        }

        public string BusyText
        {
            get { return _busyText; }
            set
            {
                _busyText = value;
                App.RunOnUiThread(() => BusyTextBlock.Text = _busyText);
            }
        }

        private void CancelTaskButton_OnClick(object sender, RoutedEventArgs e)
        {
            TaskRunner?.CancelOperations();
        }

        public void Start(string message)
        {
            BusyText = message;
            InProgressIndicator.IsBusy = true;
        }

        public void Stop()
        {
            InProgressIndicator.IsBusy = false;
        }

        private void InProgressControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            TaskRunner = Window.GetWindow(this) as ILongTaskRunner;
        }
    }
}