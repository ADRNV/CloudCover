namespace CloudCover.Drives.FilesExtensions
{
    internal static class FilesExtensions
    {
        /// <summary>
        /// Maps streams to paths
        /// </summary>
        /// <param name="streams">Stream of file</param>
        /// <param name="paths">Paths to mapping</param>
        /// <returns>Pair of path and stream</returns>
        public static IEnumerable<KeyValuePair<string, FileStream>> MapStreamsToPath(this FileStream[] streams, IList<string> paths)
        {
            for (var path = 0; path < paths.Count(); path++)
            {
                yield return new KeyValuePair<string, FileStream>(paths[path], streams[path]);
            }
        }

        /// <summary>
        /// Maps streams to paths async
        /// </summary>
        /// <param name="streams">Stream of file</param>
        /// <param name="paths">Paths to mapping</param>
        /// <returns>Pair of path and stream</returns>
        public static async Task<IEnumerable<KeyValuePair<string, FileStream>>> MapStreamsToPathAsync(this FileStream[] streams, IList<string> paths) =>
           await Task.Run(() => MapStreamsToPath(streams, paths));
    }
}
