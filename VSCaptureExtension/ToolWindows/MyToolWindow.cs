using Microsoft.VisualStudio.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace VSExtension
{
    public class MyToolWindow : BaseToolWindow<MyToolWindow>
    {
        public override string GetTitle(int toolWindowId) => "Capture Tool";

        public override Type PaneType => typeof(Pane);

        public override Task<FrameworkElement> CreateAsync(int toolWindowId, CancellationToken cancellationToken)
        {
            return Task.FromResult<FrameworkElement>(InitializeView()); //TODO faire la création de la vue
        }

        [Guid("3c791ec2-3093-492f-8a32-e7e2905f0afd")]
        internal class Pane : ToolkitToolWindowPane
        {
            public Pane()
            {
                BitmapImageMoniker = KnownMonikers.ToolWindow;
            }
        }

        private FrameworkElement InitializeView()
        {
            var toolWindow = new MyToolWindowControl();

            var viewModel = new MainViewModel();
            toolWindow.DataContext = viewModel;
            new WindowService(viewModel);

            return toolWindow;
        }
    }
}