using System.Windows;
using System.Windows.Controls;

namespace VSCaptureExtension
{
    public partial class MyToolWindowControl : UserControl
    {
        public MyToolWindowControl()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}