using Microsoft.VisualStudio.RpcContracts.Commands;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace VSCaptureExtension
{
    public partial class TransparentWindow : Window
    {
        private Point startPoint;
        private bool isDrawing = false;

        public TransparentWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Left = 0;
            this.Top = 0;
            this.Width = SystemParameters.PrimaryScreenWidth;
            this.Height = SystemParameters.PrimaryScreenHeight;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(MainCanvas);
            Canvas.SetLeft(SelectionRect, startPoint.X);
            Canvas.SetTop(SelectionRect, startPoint.Y);
            SelectionRect.Width = 0;
            SelectionRect.Height = 0;
            SelectionRect.Visibility = Visibility.Visible;
            isDrawing = true;
            CaptureMouse();
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDrawing) return;

            Point pos = e.GetPosition(MainCanvas);

            double x = Math.Min(pos.X, startPoint.X);
            double y = Math.Min(pos.Y, startPoint.Y);
            double width = Math.Abs(pos.X - startPoint.X);
            double height = Math.Abs(pos.Y - startPoint.Y);

            Canvas.SetLeft(SelectionRect, x);
            Canvas.SetTop(SelectionRect, y);
            SelectionRect.Width = width;
            SelectionRect.Height = height;

            // Update the ViewModel property with the new coordinates
            DataContext.ScreenShotZone = new Rectangle
            {
                X = x,
                Y = y,
                Width = width,
                Height = height
            };
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!isDrawing) return;
            isDrawing = false;
            ReleaseMouseCapture();
        }
    }
}
