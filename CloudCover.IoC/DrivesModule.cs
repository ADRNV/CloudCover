using CloudCover.Core.Managers;
using CloudCover.Drives;
using Microsoft.Extensions.Configuration;
using Ninject.Modules;

namespace CloudCover.IoC
{
    public class DrivesModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<IFileManager>()
                .To<FileManager>();

            this.Bind<IDriveManager>()
                .To<DriveManager>()
                .InSingletonScope();

            IConfiguration configuration = new ConfigurationBuilder()

            this.Bind<UploadManager>()
                .ToSelf()
                .InSingletonScope()
                .WithConstructorArgument();
        }
    }
}
