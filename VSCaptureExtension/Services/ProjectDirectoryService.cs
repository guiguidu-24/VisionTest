using EnvDTE;
using EnvDTE80;
using System.IO;
using System.Threading.Tasks;

public class ProjectDirectoryService : IProjectDirectoryService
{
    private readonly AsyncPackage _package;

    public ProjectDirectoryService(AsyncPackage package)
    {
        _package = package;
    }

    public async Task<string?> GetActiveProjectDirectoryAsync()
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

        var dte = await _package.GetServiceAsync(typeof(DTE)) as DTE2;
        if (dte == null) return null;

        Array activeProjects = (Array)dte.ActiveSolutionProjects;
        if (activeProjects.Length == 0) return null;

        EnvDTE.Project project = activeProjects.GetValue(0) as EnvDTE.Project;
        if (project == null || string.IsNullOrEmpty(project.FullName)) return null;

        return Path.GetDirectoryName(project.FullName);
    }
}
