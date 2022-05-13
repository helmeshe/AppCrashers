using System.ComponentModel;
using System.Windows;

namespace AppCrasher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Hooker.WindowTitle = WindowTitle.Text;
            _hooker.Hook();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            _hooker.Unhook();
        }

        private Hooker _hooker = new Hooker();
    }
}
