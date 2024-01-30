

using System.IO;


namespace Telegram_WPF.Services.FilesService
{
    internal interface IFilesWork
    {
        FileStream CreateFile(string path);
        bool ClearFolder(string path);
    }
}
