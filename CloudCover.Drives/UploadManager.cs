using CloudCover.Core.Clients;
using Microsoft.Extensions.Configuration;

namespace CloudCover.Drives
{
    public class UploadManager
    {
        private readonly FileManager _fileManager;

        private readonly IDiskClient _yandexDiskClient;

        private readonly Dictionary<string, string> _dirsAndFiles;

        private Func<IEnumerable<Stream>, Task> Fetched;

        private List<string> _fetchedPaths = new List<string>();

        public UploadManager(FileManager fileManager, IDiskClient yandexDiskClient, IConfiguration configuration)
        {
            _fileManager = fileManager;

            _yandexDiskClient = yandexDiskClient;

            Fetched += OnFetched;

            _dirsAndFiles = (Dictionary<string, string>)configuration.GetSection("FileDirFilter");
        }

        /// <summary>
        /// Gets all files as stream in dirs
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Stream> Fetch()
        {
            List<Stream> files = new List<Stream>();

            foreach (var dir in _dirsAndFiles.Keys)
            {
                foreach (var subDir in _fileManager.GetAllDirectories(dir))
                {
                    _fetchedPaths.AddRange(_fileManager.GetFilesPaths(dir, _dirsAndFiles[dir]));
                    var foundFiles = _fileManager.GetFiles(subDir, _dirsAndFiles[dir]);
                    files.AddRange(foundFiles);
                }
            }

            Fetched?.Invoke(files);
            return files;
        }

        protected virtual async Task OnFetched(IEnumerable<Stream> streams)
        {
            foreach (var mappedStream in MapStreamsToPath(streams.ToArray()))
            {
                await _yandexDiskClient.Upload(mappedStream.Key, mappedStream.Value, true);
            }
        }

        private IEnumerable<KeyValuePair<string, Stream>> MapStreamsToPath(Stream[] streams)
        {
            for (var path = 0; path < _fetchedPaths.Count; path++)
            {
                yield return new KeyValuePair<string, Stream>(_fetchedPaths[path], streams[path]);
            }
        }
    }
}
