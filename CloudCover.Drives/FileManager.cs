using CloudCover.Core.Managers;
using Microsoft.Extensions.Configuration;

namespace CloudCover.Drives
{
    /// <summary>
    /// Superstructure on standart <see cref="FileInfo"/> and <see cref="DirectoryInfo"/>
    /// Responses for getting files and mapping to <see cref="FileStream"/>
    /// </summary>
    public class FileManager : IFileManager
    {
        public FileManager()
        {

        }

        /// <summary>
        /// Gets all nested dirs
        /// </summary>
        /// <param name="path"></param>
        /// <returns>All nested dirs</returns>
        public IEnumerable<string> GetAllDirectories(string path)
        {
            var topLevelDirs = GetDirectories(path, "*", SearchOption.TopDirectoryOnly);

            foreach (var dir in topLevelDirs)
            {
                foreach (var nested in GetDirectories(dir, "*", SearchOption.AllDirectories))
                {
                    yield return nested;
                }
            }
        }

        /// <summary>
        /// Gets all nested dirs async
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task<IEnumerable<DirectoryInfo>> GetAllDirectoriesAsync(string path)
        {
            var topLevelDirs = await GetDirectoriesAsync(path, "*", SearchOption.TopDirectoryOnly);

            var nestedDirs = new List<DirectoryInfo>();

            Parallel.ForEach(topLevelDirs, async dir =>
            {
                foreach (var nested in GetDirectories(dir, "*", SearchOption.AllDirectories))
                {
                    nestedDirs.Add(new DirectoryInfo(nested));
                }
            });

            return nestedDirs.AsEnumerable();
        }

        /// <summary>
        /// Gets stream by file name
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public FileStream GetFile(string path)
        {
            return new FileStream(path, FileMode.Open);
        }

        /// <summary>
        /// Gets all file and map his to stream
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public IEnumerable<FileStream> GetFiles(string directory)
        {
            foreach (var file in Directory.GetFiles(directory))
            {
                yield return new FileStream(file, FileMode.Open);
            }
        }

        /// <summary>
        /// Gets all file and map his to stream
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="filter">file lilter</param>
        /// <returns></returns>
        public IEnumerable<FileStream> GetFiles(string directory, string filter)
        {
            foreach (var topLevelDir in GetAllDirectories(directory))
            {
                var dirInfo = new DirectoryInfo(topLevelDir);

                foreach (var file in dirInfo.GetFiles(filter, SearchOption.AllDirectories))
                {
                    yield return new FileStream(file.FullName, FileMode.Open, FileAccess.Read);
                }
            }

        }

        public IEnumerable<string> GetFilesPaths(string directory, string filter)
        {
            foreach (var topLevelDir in GetAllDirectories(directory))
            {
                var dirInfo = new DirectoryInfo(topLevelDir);

                foreach (var file in dirInfo.GetFiles(filter, SearchOption.AllDirectories))
                {
                    yield return file.FullName;
                }
            }

        }

        private IEnumerable<string> GetDirectories(string path, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            try
            {
                return Directory.GetDirectories(path, searchPattern, searchOption)
                    .AsParallel();
            }
            catch (UnauthorizedAccessException)
            {
                return new List<string>()
                    .AsParallel();
            }
        }

        private async Task<IEnumerable<string>> GetDirectoriesAsync(string path, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            try
            {
                return await Task.Run(() => Directory.GetDirectories(path, searchPattern, searchOption)
                    .AsEnumerable())
                    .ConfigureAwait(false);
            }
            catch (UnauthorizedAccessException)
            {
                return await Task.FromResult(new List<string>()
                    .AsEnumerable())
                    .ConfigureAwait(false);
            }
        }
    }
}
