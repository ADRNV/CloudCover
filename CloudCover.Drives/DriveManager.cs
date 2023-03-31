using CloudCover.Core.Managers;
using System.Management;

namespace CloudCover.Drives
{
    public class DriveManager : IDriveManager
    {
        private readonly ManagementEventWatcher _watcher = new ManagementEventWatcher();

        private List<DriveInfo> _drives = new();

        public event Action<object, IEnumerable<DriveInfo>> OnDriveSetChanged;
        
        public DriveManager()
        {
            _watcher = new ManagementEventWatcher();
            WqlEventQuery query = 
                new WqlEventQuery("SELECT * FROM Win32_VolumeChangeEvent WHERE EventType = 2 OR EventType = 3");
            _watcher.EventArrived += new EventArrivedEventHandler(HandleVolumeChange);
            _watcher.Query = query;
            _watcher.Start();
            _watcher.WaitForNextEvent();

        }

        private void HandleVolumeChange(object sender, EventArrivedEventArgs e)
        {
            _drives = DriveInfo.GetDrives()
                .Where(d => d.IsReady)
                .ToList();

            OnDriveSetChanged.Invoke(this, _drives);
        }

        public DriveInfo? GetDrive(string name) =>
            _drives.Where(d => d.Name == d.Name)
                .FirstOrDefault();

        public DriveInfo? GetDrive(string name, string label) =>
            GetReadyDrives()
            .Where(d => d.Name == name && d.VolumeLabel == label)
            .FirstOrDefault();

        public IEnumerable<DriveInfo> GetDrivesOfType(DriveType driveType) =>
            GetReadyDrives()
            .Where(d => d.DriveType == driveType);

        public IEnumerable<DriveInfo> GetReadyDrives() =>
            _drives.Where(d => d.IsReady);
    }
}
