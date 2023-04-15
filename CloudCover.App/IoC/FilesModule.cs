using CloudCover.Core.Clients;
using CloudCover.Core.Managers;
using CloudCover.Drives;
using CloudCover.Services;
using Microsoft.Extensions.Configuration;
using Ninject.Modules;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace CloudCover.App.IoC
{
    public class FilesModule : NinjectModule
    {
        public override void Load()
        {
            ILogger logger = new LoggerConfiguration()
                .WriteTo
                .Console(LogEventLevel.Debug)
                .MinimumLevel.Debug()
                .CreateLogger();

            this.Bind<ILogger>()
                .ToConstant(logger);

            this.Bind<IDriveManager>()
                .To<DriveManager>();

            this.Bind<IFileManager>()
                .To<FileManager>();

            var config = new ConfigurationBuilder()
                .AddJsonFile($"{Environment.CurrentDirectory}/Configuration/appConfig.json")
                .Build();

            this.Bind<IDiskClient>()
                .To<YandexDiskClient>()
                .WithConstructorArgument("token", config.GetSection("token").Value);

            this.Bind<UploadManager>()
                .ToSelf()
                .WithConstructorArgument("configuration", config);
        }
    }
}
