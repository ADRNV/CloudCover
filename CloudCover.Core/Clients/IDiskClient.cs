namespace CloudCover.Core.Clients
{
    /// <summary>
    /// Discribe cloud drive behavior
    /// </summary>
    public interface IDiskClient
    {
        Task Upload(string path, FileStream inputFile, bool overwrite);

        Task<Stream> Download(string path);
    }
}
