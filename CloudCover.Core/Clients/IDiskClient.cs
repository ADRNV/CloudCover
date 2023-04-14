namespace CloudCover.Core.Clients
{
    public interface IDiskClient
    {
        Task Upload(string path, FileStream inputFile, bool overwrite);

        Task<Stream> Download(string path);
    }
}
