using System.Threading.Tasks;

namespace src.Interfaces
{
    public interface IFtpClientRepositoryAsync
    {
        Task<string> UploadFileAsync(string fileName, string data);
        Task<string> AppendFileAsync(string fileName, string data);
        Task<string> DeleteFileAsync(string fileName);
        Task<string> DownloadFileAsync(string fileName);
        Task<string> CreateDirectoryAsync(string directoryName);
        Task<string> ListDirectoryAsync(string directoryName);
        Task<string> ListDirectoryDetailsAsync(string directoryName);
        Task<string> DeleteDirectoryAsync(string directoryName);
        Task<string> RenameDirectoryAsync(string sourceDirectoryName, string targetDirectoryName);
    }
}