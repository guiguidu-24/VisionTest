using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Drawing;
using System.Threading;
using System.Windows.Media.Imaging;


namespace VSCaptureExtension
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ScreenshotShape shape = ScreenshotShape.Rectangle;
        private int captureDelay = 0;
        private Rectangle screenShotZone;
        private bool showCaptureUI = false;
        private bool showCaptureTool = false;
        private BitmapImage currentScreenshot = null;
        private string textFound = string.Empty;
        private PreviewApiService previewApiService = new PreviewApiService();


        public string TextFound
        {
            get { return textFound; }
            set
            {
                if (textFound == value) return;
                textFound = value;
                OnPropertyChanged();
            }
        }

        public BitmapImage CurrentScreenShot
        {
            get { return currentScreenshot; }
            set
            {
                if (currentScreenshot == value) return;
                currentScreenshot = value;
                OnPropertyChanged();

                if (currentScreenshot != null)
                {
                    TextFound = previewApiService.GetText(currentScreenshot);
                }
            }
        }

        public bool ShowCaptureTool
        {
            get { return showCaptureTool; }
            set
            {
                if (showCaptureTool == value) return;
                showCaptureTool = value;
                OnPropertyChanged();
            }
        }

        public bool ShowCaptureUI
        {
            get { return showCaptureUI; }
            set
            {
                showCaptureUI = value;
                OnPropertyChanged();
            }
        }

        public Rectangle ScreenShotZone
        {
            get { return screenShotZone; }
            set 
            {
                ShowCaptureTool = false;
                ShowCaptureUI = true;
                screenShotZone = value;
                OnPropertyChanged();
                Thread.Sleep(100);
                CurrentScreenShot = Screen.Shoot(screenShotZone);
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
        public ICommand ValidateCommand { get; }


        public MainViewModel()
        {
            shape = ScreenshotShape.Rectangle;
            captureDelay = 0;

            ClickAddCommand = new RelayCommand(() =>
            {
                ShowCaptureUI = true;
            });

            ClickNewCommand = new RelayCommand(() =>
            {
                ShowCaptureUI = false;
                Thread.Sleep(captureDelay * 1000);
                ShowCaptureTool = true;
            });

            ValidateCommand = new RelayCommand(() =>
            {
                ShowCaptureUI = false;
                ShowCaptureTool = false;
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    }
}
