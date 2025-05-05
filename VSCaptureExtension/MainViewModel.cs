using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Drawing;


namespace VSCaptureExtension
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ScreenshotShape shape = ScreenshotShape.Rectangle;
        private int captureDelay = 0;
        private Rectangle screenShotZone;
        private bool showCaptureUI = false;
        private bool captureToolStarted = false;

        public bool CaptureToolStarted
        {
            get { return captureToolStarted; }
            set
            {
                if (showCaptureUI == value) return;
                captureToolStarted = value;
                OnPropertyChanged();
            }
        }

        public bool ShowCaptureUI
        {
            get { return showCaptureUI; }
            set
            {
                if (showCaptureUI == value) return;
                showCaptureUI = value;
                OnPropertyChanged();
            }
        }

        public Rectangle ScreenShotZone
        {
            get { return screenShotZone; }
            set 
            { 
                screenShotZone = value;
                //Take a screenshot of the selected area + show it in VS to validate the selection
                OnPropertyChanged();
            }
        }

        public ScreenshotShape Shape
        {
            get { return shape; }
            set 
            { 
                if(shape == value) return;  
                shape = value; 
                OnPropertyChanged(); 
            }
        }

        public int CaptureDelay
        {
            get { return captureDelay; }
            set 
            {
                if (captureDelay == value) return;
                captureDelay = value; 
                OnPropertyChanged(); 
            }
        }


        public ICommand ClickAddCommand { get; }
        public ICommand ClickNewCommand { get; }


        public MainViewModel()
        {
            shape = ScreenshotShape.Rectangle;
            captureDelay = 0;

            ClickAddCommand = new RelayCommand(() =>
            {
                ShowCaptureUI = true;
                //captureUI = new CaptureUI();
                //captureUI.DataContext = this;
                //captureUI.Show();
            });

            ClickNewCommand = new RelayCommand(() =>
            {
                //captureUI.Hide();
                //Thread.Sleep(captureDelay * 1000);
                //var transParentWindow = new TransparentWindow();
                //transParentWindow.DataContext = this;
                //transParentWindow.Show();

                ShowCaptureUI = false;

            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    }
}
