using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using src.Interfaces;

namespace src.Services
{
    public class FtpClientRepositoryAsync : IFtpClientRepositoryAsync
    {
        private static FtpWebRequest ftp;
        public string BaseUri { get; } = @"";
        public string UserName { get; } = "";
        public string Password { get; } = "";

        private void Initialize(string path = null)
        {
            ftp = (FtpWebRequest)WebRequest.Create(string.Concat(BaseUri, path));
            ftp.Credentials = new NetworkCredential(UserName, Password);
            ftp.UseBinary = true;
        }
        public FtpClientRepositoryAsync()
        {
            Initialize();
        }
        public async Task<string> AppendFileAsync(string fileName, string data)
        {
            Initialize(fileName);
            ftp.Method = WebRequestMethods.Ftp.AppendFile;
            var stream = await WriteStreamAsync(data);
            using var response = (FtpWebResponse)await ftp.GetResponseAsync();
            return await Task.FromResult($"File Appended, status {response.StatusDescription}");
        }

        public async Task<string> CreateDirectoryAsync(string directoryName)
        {
            Initialize(directoryName);
            ftp.Method = WebRequestMethods.Ftp.MakeDirectory;
            var response = (FtpWebResponse)await ftp.GetResponseAsync();
            return await Task.FromResult($"Directory Created, status {response.StatusDescription}");
        }

        public async Task<string> DeleteDirectoryAsync(string directoryName)
        {
            Initialize(directoryName);
            ftp.Method = WebRequestMethods.Ftp.RemoveDirectory;
            var response = (FtpWebResponse)await ftp.GetResponseAsync();
            return await Task.FromResult($"Directory Deleted, status {response.StatusDescription}");
        }

        public async Task<string> DeleteFileAsync(string fileName)
        {
            Initialize(fileName);
            ftp.Method = WebRequestMethods.Ftp.DeleteFile;
            var response = (FtpWebResponse)await ftp.GetResponseAsync();
            return await Task.FromResult($"File Deleted, status {response.StatusDescription}");
        }

        public async Task<string> DownloadFileAsync(string fileName)
        {
            var splittedPath = fileName.Split("/");
            Initialize(fileName);
            ftp.Method = WebRequestMethods.Ftp.DownloadFile;
            using var response = (FtpWebResponse)await ftp.GetResponseAsync();
            using var stream = response.GetResponseStream();
            using StreamReader sr = new StreamReader(stream);
            await File.WriteAllTextAsync(splittedPath[splittedPath.Length - 1], await sr.ReadToEndAsync());

            return await Task.FromResult($"Download Complete, status {response.StatusDescription}");
        }

        public async Task<string> ListDirectoryAsync(string directoryName)
        {
            Initialize(directoryName);
            ftp.Method = WebRequestMethods.Ftp.ListDirectory;
            var response = (FtpWebResponse)await ftp.GetResponseAsync();
            return await ReadStreamAsync(response);
        }

        public async Task<string> ListDirectoryDetailsAsync(string directoryName)
        {
            Initialize(directoryName);
            ftp.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            var response = (FtpWebResponse)await ftp.GetResponseAsync();
            return await ReadStreamAsync(response);
        }

        public async Task<string> RenameDirectoryAsync(string sourceDirectoryName, string targetDirectoryName)
        {
            Initialize(sourceDirectoryName);
            ftp.Method = WebRequestMethods.Ftp.Rename;
            ftp.RenameTo = targetDirectoryName;
            var response = (FtpWebResponse)await ftp.GetResponseAsync();
            return await Task.FromResult($"Directory Renamed, status {response.StatusDescription}");
        }

        public async Task<string> UploadFileAsync(string fileName, string data)
        {
            var splittedPath = fileName.Split("/");
            Initialize(fileName);
            ftp.Method = WebRequestMethods.Ftp.UploadFile;
            byte[] content = Encoding.UTF8.GetBytes(data);
            using var fs = new FileStream(splittedPath[splittedPath.Length - 1], FileMode.OpenOrCreate, FileAccess.ReadWrite);
            await fs.WriteAsync(content, 0, content.Length);
            ftp.ContentLength = content.Length;
            using Stream requestStream = ftp.GetRequestStream();
            fs.Position = 0;
            await fs.CopyToAsync(requestStream, content.Length);
            await fs.FlushAsync();
            await fs.DisposeAsync();
            return await Task.FromResult($"File Uploaded Successfully.");
        }

        private async Task<string> ReadStreamAsync(FtpWebResponse stream)
        {
            using var response = stream.GetResponseStream();
            using var reader = new StreamReader(response);
            return await reader.ReadToEndAsync();
        }

        private async Task<Stream> WriteStreamAsync(string data)
        {
            byte[] content = Encoding.UTF8.GetBytes(data);
            ftp.ContentLength = content.Length;
            using var stream = await ftp.GetRequestStreamAsync();
            await stream.WriteAsync(content, 0, content.Length);
            return stream;
        }
    }
}