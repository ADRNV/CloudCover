using CloudCover.Core.Clients;
using CloudCover.Core.Managers;
using CloudCover.Drives;
using CloudCover.Services;
using Microsoft.Extensions.Configuration;
using Ninject.Modules;

namespace CloudCover.App.IoC
{
    public class UploadModule : NinjectModule
    {
        public override void Load()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile($"{Environment.CurrentDirectory}/Configuration/appConfig.json")
                .Build();

            this.Bind<IDiskClient>()
                .To<YandexDiskClient>()
                .WithConstructorArgument("token", config.GetSection("token").Value);

            this.Bind<IFileUploadManager>()
                .To<UploadManager>()
                .WithConstructorArgument("configuration", config);
        }
    }
}
