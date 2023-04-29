using CloudCover.Core.Clients;
using CloudCover.Core.Managers;
using CloudCover.Drives.FilesExtensions;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace CloudCover.Drives
{
    /// <summary>
    /// Responses for where and which files upload on CloudDrive
    /// </summary>
    public class UploadManager : IFileUploadManager
    {
        private readonly FileManager _fileManager;

        private readonly IDiskClient _yandexDiskClient;

        private readonly Dictionary<string, string> _dirsAndFiles;

        private Func<IEnumerable<FileStream>, Task> Fetched;

        private List<string> _fetchedPaths = new List<string>();

        private readonly ILogger _logger;

        public UploadManager(FileManager fileManager, IDiskClient yandexDiskClient, IConfiguration configuration)
        {
            _fileManager = fileManager;

            _yandexDiskClient = yandexDiskClient;

            Fetched += OnFetched;

            //JSON identifies ':' in name of drive like end of key name.For this he has been replaced spacing
            _dirsAndFiles = configuration
                .GetSection("FileDirFilter")
                .GetChildren()
                .ToDictionary(c => c.Key.Replace(" ", @":"), c => c.Value)!;//When we manupelate in configuration manage all Ok
        }

        public UploadManager(FileManager fileManager, IDiskClient yandexDiskClient, IConfiguration configuration, ILogger logger) : this(fileManager, yandexDiskClient, configuration)
        {
            _logger = logger;
        }

        /// <summary>
        /// Gets all files as stream in dirs
        /// </summary>
        /// <returns>Streams of files</returns>
        public IEnumerable<FileStream> Fetch()
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
                _logger.Information("Found {files} of type {type}", files.Count, _dirsAndFiles[dir]);
            }


            Fetched?.Invoke(files);
            return files;
        }

        // <summary>
        /// Gets all files async as stream in dirs
        /// </summary>
        /// <returns>Streams of files</returns>
        public Task<IEnumerable<FileStream>> FetchAsync() =>
            Task.Run(() => Fetch());

        /// <summary>
        /// Event handler for fetched files
        /// </summary>
        /// <param name="streams">Stream of file</param>
        /// <returns></returns>
        protected virtual async Task OnFetched(IEnumerable<FileStream> streams)
        {
            var mappedStreams = await streams.ToArray()
                .MapStreamsToPathAsync(_fetchedPaths);

            foreach (var mappedStream in mappedStreams)
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
    }
}
