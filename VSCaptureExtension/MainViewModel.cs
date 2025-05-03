using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;

namespace VSCaptureExtension
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private Window captureUI;
        private ScreenshotShape shape;
        private int captureDelay;
        private Rectangle screenShotZone;

        public Rectangle ScreenShotZone
        {
            get { return screenShotZone; }
            set { screenShotZone = value; OnPropertyChanged(); }
        }

        public ScreenshotShape Shape
        {
            get { return shape; }
            set { shape = value; OnPropertyChanged(); }
        }

        public int CaptureDelay
        {
            get { return captureDelay; }
            set { captureDelay = value; OnPropertyChanged(); }
        }


        public ICommand ClickAddCommand { get; }
        public ICommand ClickNewCommand { get; }


        public MainViewModel()
        {
            shape = ScreenshotShape.Rectangle;
            captureDelay = 0;

            ClickAddCommand = new RelayCommand(() =>
            {
                captureUI = new CaptureUI();
                captureUI.DataContext = this;
                captureUI.Show();
            });

            ClickNewCommand = new RelayCommand(() =>
            {
                captureUI.Hide();
                Thread.Sleep(captureDelay * 1000);
                var transParentWindow = new TransparentWindow();
                transParentWindow.DataContext = this;
                transParentWindow.Show();
            });
        }

        


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    }
}
