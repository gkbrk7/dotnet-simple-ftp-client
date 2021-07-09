using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using src.Services;

namespace SimpleFTPClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var ftp = new FtpClientRepositoryAsync();
            var resp = ftp.RenameDirectoryAsync("merhaba", "test").GetAwaiter().GetResult();
            System.Console.WriteLine(resp);
        }
    }
}
