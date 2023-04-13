using CloudCover.Core.Clients;
using CloudCover.Core.Managers;
using CloudCover.Drives;
using CloudCover.Services;
using Microsoft.Extensions.Configuration;
using Ninject.Modules;

namespace CloudCover.App.IoC
{
    public class FilesModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<IDriveManager>()
                .To<DriveManager>();

            this.Bind<IFileManager>()
                .To<FileManager>();

            var config = new ConfigurationBuilder()
                .AddJsonFile($"{Environment.CurrentDirectory}/Configuration/appConfig.json")
                .Build();

            this.Bind<IDiskClient>()
                .To<YandexDiskClient>()
                .WithConstructorArgument("token", "12345");

            this.Bind<UploadManager>()
                .ToSelf()
                .WithConstructorArgument("configuration", config);
        }
    }
}
