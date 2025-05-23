using EnvDTE;
using EnvDTE80;
using System.Runtime.InteropServices;
using System.Threading;

namespace VisionTest.VSExtension.Services
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("My Extension", "Saves images to current project", "1.0")]
    [Guid(PackageGuidString)]
    public sealed class ExtensionPackage : AsyncPackage
    {
        public const string PackageGuidString = "d81a0a36-56a2-4e52-888e-d4bc7d048e2a";

        public DTE2 Dte { get; private set; }

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await base.InitializeAsync(cancellationToken, progress);

            // Switch to main thread to access DTE
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            Dte = (DTE2)await GetServiceAsync(typeof(DTE));

        }
    }
}
