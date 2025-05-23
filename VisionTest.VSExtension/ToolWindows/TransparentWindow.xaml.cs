using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace VisionTest.VSExtension
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
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!isDrawing) return;
            isDrawing = false;
            ReleaseMouseCapture();

            System.Windows.Point endPoint = e.GetPosition(MainCanvas);

            double x = Math.Min(startPoint.X, endPoint.X);
            double y = Math.Min(startPoint.Y, endPoint.Y);
            double width = Math.Abs(endPoint.X - startPoint.X);
            double height = Math.Abs(endPoint.Y - startPoint.Y);

            // Convert to int rectangle
            var rect = new System.Drawing.Rectangle(
                (int)(x + this.Left),
                (int)(y + this.Top),
                (int)width,
                (int)height
            );

            // Assign to ViewModel
            if (DataContext is MainViewModel vm)
            {
                vm.ScreenShotZone = rect;
            }
        }
    }
}
