using System.Threading.Tasks;

public interface IProjectDirectoryService
{
    Task<string> GetActiveProjectDirectoryAsync();
}
