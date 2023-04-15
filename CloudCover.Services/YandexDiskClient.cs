using CloudCover.Core.Clients;
using YandexDisk.Client;
using YandexDisk.Client.Clients;
using YandexDisk.Client.Http;
using YandexDisk.Client.Protocol;

namespace CloudCover.Services
{
    public class YandexDiskClient : IDiskClient
    {
        private readonly string _token;

        private readonly IDiskApi _diskApi;

        public YandexDiskClient(string token)
        {
            _token = token;

            _diskApi = new DiskHttpApi(_token);
        }

        public async Task<Link?> CreateDirsForFile(string path)
        {
            string fullPath = string.Empty;

            Link? link = null;

            foreach (var dir in path.Split("/").SkipLast(1))
            {
                fullPath += $"/{dir}";

                link = await _diskApi.Commands.CreateDictionaryAsync(fullPath);                
            }
            
            return link;
        }

        public async Task Upload(string path, FileStream inputFile, bool overwrite)
        {
            try
            {
                await CreateDirsForFile(path);
                var link = await _diskApi.Files.GetUploadLinkAsync(path, overwrite);
                await _diskApi.Files.UploadAsync(link, inputFile);    
            }
            catch (YandexApiException)
            {
                var link = await _diskApi.Files.GetUploadLinkAsync(path, overwrite);
                await _diskApi.Files.UploadAsync(link, inputFile);
            }
            
        }

        public async Task<Stream> Download(string path)
            => await _diskApi.Files.DownloadFileAsync(path);

        public async Task<Resource> GetFileInfo(string path)
        {
            var request = new ResourceRequest()
            {
                Path = path
            };

            return await _diskApi.MetaInfo.GetInfoAsync(request);
        }

        public async Task Delete(string path)
        {
            DeleteFileRequest deleteFileRequest = new DeleteFileRequest()
            {
                Path = path,
                Permanently = true
            };

            await _diskApi.Commands.DeleteAsync(deleteFileRequest);
        }
    }
}
