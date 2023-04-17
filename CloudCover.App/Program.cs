using CloudCover.App.IoC;
using CloudCover.Core.Managers;
using Ninject;

IKernel IoC = new StandardKernel();

IoC.Load(new LoggingModule(), new FilesModule(), new UploadModule());

var uploader = IoC.Get<IFileUploadManager>();

IoC.Get<IDriveManager>()
    .DriveSetChanged += IDriveManager_OnDriveSetChanged;

async Task IDriveManager_OnDriveSetChanged(object arg1, IEnumerable<DriveInfo> arg2)
{
    await uploader.FetchAsync();
}

Console.Read();