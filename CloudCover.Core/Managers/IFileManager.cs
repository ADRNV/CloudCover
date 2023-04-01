namespace CloudCover.Core.Managers
{
    public interface IFileManager
    {
        IEnumerable<string> GetAllDirectories(string path);

        Task<IEnumerable<DirectoryInfo>> GetAllDirectoriesAsync(string path);

        Stream GetFile(string path);

        IEnumerable<Stream> GetFiles(string directory);

        IEnumerable<Stream> GetFiles(string directory, string filter);
    }
}
