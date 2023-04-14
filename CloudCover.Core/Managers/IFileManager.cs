namespace CloudCover.Core.Managers
{
    public interface IFileManager
    {
        IEnumerable<string> GetAllDirectories(string path);

        Task<IEnumerable<DirectoryInfo>> GetAllDirectoriesAsync(string path);

        FileStream GetFile(string path);

        IEnumerable<FileStream> GetFiles(string directory);

        IEnumerable<FileStream> GetFiles(string directory, string filter);

        IEnumerable<string> GetFilesPaths(string directory, string filter);
    }
}
