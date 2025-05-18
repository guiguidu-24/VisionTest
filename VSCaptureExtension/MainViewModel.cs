using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Drawing;
using System.Threading;
using System.Windows.Media.Imaging;


namespace VSExtension
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
        private bool isTextActivated = false;
        private string currentElementName = string.Empty;
        private string textToFind = string.Empty;
        private bool isImageActivated = false;

        public bool IsImageActivated
        {
            get { return isImageActivated; }
            set
            {
                if (isImageActivated == value) return;
                isImageActivated = value;
                OnPropertyChanged();
            }
        }

        public string CurrentElementName
        {
            get { return currentElementName; }
            set
            {
                if (currentElementName == value) return;
                currentElementName = value;
                OnPropertyChanged();
            }
        }

        public bool IsTextActivated
        {
            get { return isTextActivated; }
            set
            {
                if (isTextActivated == value) return;
                isTextActivated = value;
                OnPropertyChanged();
            }
        }

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
        public ICommand SaveCommand { get; }


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

            SaveCommand = new RelayCommand(() =>
            {
                //TODO: Check if the name is already used
                //TODO Repository.InsertOrUpdateScreenElement()

                string text = null;
                BitmapImage image = null;
                if (isTextActivated)
                {
                    text = textToFind;
                }
                if (isImageActivated)
                {
                    image = currentScreenshot;
                }
                if (!(isTextActivated || isImageActivated))
                {
                    return;
                }

                //repository.InsertOrUpdateScreenElement(currentElementName, text, image);

                ShowCaptureUI = false;
                ShowCaptureTool = false;
                CurrentScreenShot = null;
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    }
}
