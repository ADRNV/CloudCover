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
            this.Bind<IDriveManager>()
                .To<DriveManager>();

            this.Bind<IFileManager>()
                .To<FileManager>();
        }
    }
}
