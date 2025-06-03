
namespace VisionTest.VSExtension.Services
{
    using System;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Windows.Media.Imaging;
    using EnvDTE;
    using EnvDTE80;
    using Microsoft.VisualStudio.Shell;
    using VisionTest.VSExtension.Utils;

    public static class ProjectService
    {
        //private static readonly DTE2 _dte;

        public static DTE2 Dte {private get; set; }


        //public void SaveImageToProjectDirectory(BitmapImage image, string relativePath) //TODO: do it async
        //{
        //    ThreadHelper.ThrowIfNotOnUIThread(nameof(SaveImageToProjectDirectory));
        //
        //    Project project = GetActiveProject();
        //    if (project == null)
        //        throw new InvalidOperationException("No active project found.");
        //
        //    string projectDir = Path.GetDirectoryName(project.FullName);
        //    if (string.IsNullOrEmpty(projectDir))
        //        throw new InvalidOperationException("Unable to determine project directory.");
        //
        //    string fullPath = Path.Combine(projectDir, relativePath);
        //    Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!); // Ensure folder exists
        //
        //    if (File.Exists(fullPath))
        //    {
        //        throw new ArgumentException($"File already exists: {relativePath}");
        //    }
        //
        //    image.ConvertToBitmap().Save(fullPath, ImageFormat.Png);
        //
        //}

        private static Project GetActiveProject()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                Array activeSolutionProjects = (Array)Dte.ActiveSolutionProjects;
                if (activeSolutionProjects.Length > 0)
                {
                    return activeSolutionProjects.GetValue(0) as Project;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        public static string GetActiveProjectDirectory()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            Project project = GetActiveProject();
            if (project == null)
                throw new InvalidOperationException("No active project found.");

            return Path.GetDirectoryName(project.FullName);
        }
    }

}
