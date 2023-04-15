using CloudCover.App.IoC;
using CloudCover.Core.Managers;
using CloudCover.Drives;
using Ninject;

IKernel IoC = new StandardKernel();

IoC.Load(new FilesModule());

var uploader = IoC.Get<UploadManager>();

IoC.Get<IDriveManager>()
    .DriveSetChanged += IDriveManager_OnDriveSetChanged;

async Task IDriveManager_OnDriveSetChanged(object arg1, IEnumerable<DriveInfo> arg2)
{
    await uploader.FetchAsync();
}

Console.Read();