namespace CloudCover.Core.Managers
{
    /// <summary>
    /// Describe manager for upload files or etc.
    /// </summary>
    /// <typeparam name="T">Stream type</typeparam>
    public interface IUploadManager<T> where T : Stream
    {
        IEnumerable<T> Fetch();

        Task<IEnumerable<T>> FetchAsync();
    }
}
