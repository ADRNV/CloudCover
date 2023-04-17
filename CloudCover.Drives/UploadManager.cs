﻿using CloudCover.Core.Clients;
using CloudCover.Core.Managers;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace CloudCover.Drives
{
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

            _dirsAndFiles = configuration
                .GetSection("FileDirFilter")
                .GetChildren()
                .ToDictionary(c => c.Key.Replace(" ", @":"), c => c.Value)!;
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
            var mappedStreams = await MapStreamsToPathAsync(streams.ToArray());

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
  
        private IEnumerable<KeyValuePair<string, FileStream>> MapStreamsToPath(FileStream[] streams)
        {
            for (var path = 0; path < _fetchedPaths.Count; path++)
            {
                yield return new KeyValuePair<string, FileStream>(_fetchedPaths[path], streams[path]);
            }
        }

        private async Task<IEnumerable<KeyValuePair<string, FileStream>>> MapStreamsToPathAsync(FileStream[] streams) =>
           await Task.Run(() => MapStreamsToPath(streams));
    }
}
