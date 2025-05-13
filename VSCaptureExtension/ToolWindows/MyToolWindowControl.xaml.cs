using System.Windows;
using System.Windows.Controls;

namespace VSCaptureExtension
{
    public partial class MyToolWindowControl : UserControl
    {
        public MyToolWindowControl()
        {
            InitializeComponent();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.Text == "Text to find...")
            {
                textBox.Text = string.Empty;
            }
        }
    }
}