using CloudCover.App.IoC;
using CloudCover.Core.Managers;
using CloudCover.Drives;
using Ninject;

IKernel IoC = new StandardKernel();

IoC.Load(new FilesModule());

var uploader = IoC.Get<UploadManager>();

IoC.Get<IDriveManager>()
    .OnDriveSetChanged += IDriveManager_OnDriveSetChanged;

void IDriveManager_OnDriveSetChanged(object arg1, IEnumerable<DriveInfo> arg2)
{
    uploader.Fetch();
}