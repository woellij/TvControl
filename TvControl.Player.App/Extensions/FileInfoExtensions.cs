using System.IO;

namespace TvControl.Player.App.Extensions
{
    public static class FileInfoExtensions
    {

        public static void Rename(this FileInfo fileInfo, string newName)
        {
            fileInfo.MoveTo(fileInfo.Directory.FullName + "\\" + newName);
        }

    }
}