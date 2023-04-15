using CloudCover.Core.Clients;
using Microsoft.Extensions.Configuration;

namespace CloudCover.Drives
{
    public class UploadManager
    {
        private readonly FileManager _fileManager;

        private readonly IDiskClient _yandexDiskClient;

        private readonly Dictionary<string, string> _dirsAndFiles;

        private Func<IEnumerable<FileStream>, Task> Fetched;

        private List<string> _fetchedPaths = new List<string>();

        public UploadManager(FileManager fileManager, IDiskClient yandexDiskClient, IConfiguration configuration)
        {
            _fileManager = fileManager;

            _yandexDiskClient = yandexDiskClient;

            Fetched += OnFetched;

            _dirsAndFiles = configuration
                .GetSection("FileDirFilter")
                .GetChildren()
                .ToDictionary(c => c.Key.Replace(" ", @":"), c => c.Value)!;
        }

        /// <summary>
        /// Gets all files as stream in dirs
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Stream> Fetch()
        {
            List<FileStream> files = new List<FileStream>();

            foreach (var dir in _dirsAndFiles.Keys)
            {
                foreach (var subDir in _fileManager.GetAllDirectories(dir))
                {
                    _fetchedPaths.Add(subDir);
                    var foundFiles = _fileManager.GetFiles(subDir, _dirsAndFiles[dir]);
                    files.AddRange(foundFiles);
                }
            }

            Fetched?.Invoke(files);
            return files;
        }

        protected virtual async Task OnFetched(IEnumerable<FileStream> streams)
        {
            foreach (var mappedStream in MapStreamsToPath(streams.ToArray()))
            {
                await _yandexDiskClient
                    .Upload(ToApiPath(mappedStream.Value.Name), mappedStream.Value, true);
            }
        }

        private string ToApiPath(string path)
        {
            var apiPath = path
                .Split('\\')
                .Skip(1)
                .Aggregate((c, n) => c + $"/{n}");

            return apiPath;
        }

        private IEnumerable<KeyValuePair<string, FileStream>> MapStreamsToPath(FileStream[] streams)
        {
            for (var path = 0; path < _fetchedPaths.Count; path++)
            {
                yield return new KeyValuePair<string, FileStream>(_fetchedPaths[path], streams[path]);
            }
        }
    }
}
