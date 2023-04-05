namespace CloudCover.Core.Clients
{
    public interface IDiskClient
    {
        Task Upload(string path, Stream inputFile, bool overwrite);

        Task<Stream> Download(string path);
    }
}
