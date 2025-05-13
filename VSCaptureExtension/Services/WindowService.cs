using System.Text.RegularExpressions;
using System.Windows;

namespace VSCaptureExtension
{
    public class WindowService //TODO : IWindowService make two window services one for each window
    {
        private readonly MainViewModel _dataContext;
        private Window window = null;
        private Window transParentWindow = null;

        public WindowService(MainViewModel dataContext)
        {
            _dataContext = dataContext;
            _dataContext.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(MainViewModel.ShowCaptureUI))
                {
                    if (_dataContext.ShowCaptureUI)
                    {
                        ShowWindow();
                    }
                    else
                    {
                        HideWindow();
                    }
                }
            };

            _dataContext.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(MainViewModel.ShowCaptureTool))
                {
                    if (_dataContext.ShowCaptureTool)
                    {
                        ShowCaptureTool();
                    }
                    else
                    {
                        HideCaptureTool();
                    }
                }
            };
        }

        /// <summary>
        /// Shows the CaptureUI window.
        /// </summary>
        public void ShowWindow()
        {
            if (window != null)
            {
                if (!window.IsVisible)
                    window.Show();

                if (window.WindowState == WindowState.Minimized)
                    window.WindowState = WindowState.Normal;

                window.Activate();
                //window.Topmost = true;  // Bring to front
                //window.Topmost = false; // Allow others to overlay
                window.Focus();
                return;
            }
            window = new CaptureUI();
            window.Closed += (s, e) =>
            {
                window = null;
                _dataContext.ShowCaptureUI = false;
            };

            window.DataContext = _dataContext; // Corrected variable name to _dataContext
            window.Show();
        }

        /// <summary>
        /// Hides the CaptureUI window.
        /// </summary>
        public void HideWindow()
        {
            //if (window == null) return;
            window?.Close();
        }

        public void ShowCaptureTool()
        {
            transParentWindow = new TransparentWindow();
            transParentWindow.DataContext = _dataContext;
            transParentWindow.Show();
        }

        public void HideCaptureTool()
        {
           transParentWindow?.Close();
        }
    }

}
