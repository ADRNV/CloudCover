using CloudCover.Core.Managers;
using CloudCover.Drives;
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
        }
    }
}
