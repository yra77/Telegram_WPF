

using System.IO;


namespace Telegram_WPF.Services.FilesService
{
    //new Uri(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..")) + "/Images/ic_mute.png"
    internal class FilesWork : IFilesWork
    {
        public bool ClearFolder(string path)
        {
            try
            {
                System.IO.DirectoryInfo di = new DirectoryInfo(path);
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }

                return true;
            }
            catch { return false; }
        }

        public FileStream CreateFile(string path)
        {
            var fileStream = File.Create(path);
            return fileStream;
        }
    }
}
