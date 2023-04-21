using CloudCover.Core.Managers;
using Serilog;
using System.Management;

namespace CloudCover.Drives
{
    public class DriveManager : IDriveManager
    {
        private readonly ManagementEventWatcher _watcher = new ManagementEventWatcher();

        public List<DriveInfo> DriveSet { get; private set; } = new List<DriveInfo>();

        public event Func<object, IEnumerable<DriveInfo>, Task> DriveSetChanged;

        private ILogger _logger;

        public DriveManager()
        {
            _watcher = new ManagementEventWatcher();
            WqlEventQuery query =
                new WqlEventQuery("SELECT * FROM Win32_VolumeChangeEvent WHERE EventType = 2");
            _watcher.EventArrived += new EventArrivedEventHandler(OnDriveSetChanged);
            _watcher.Query = query;
            _watcher.Start();
            _watcher.WaitForNextEvent();

        }

        public DriveManager(ILogger logger) 
        {
            _watcher = new ManagementEventWatcher();
            WqlEventQuery query =
                new WqlEventQuery("SELECT * FROM Win32_VolumeChangeEvent WHERE EventType = 2");
            _watcher.EventArrived += new EventArrivedEventHandler(OnDriveSetChanged);
            _watcher.Query = query;
            _watcher.Start();
            _watcher.WaitForNextEvent();

            _logger = logger;

        }

        private async void OnDriveSetChanged(object sender, EventArrivedEventArgs e)
        {
            await Task.Run(() => DriveSet = DriveInfo.GetDrives()
                .Where(d => d.IsReady)
                .ToList());

            _logger?.Debug("{Time} Changed drive set {DriveName}", DateTime.Now, DriveSet.Last().Name);

            if(DriveSetChanged is not null)
            {
                await DriveSetChanged?.Invoke(this, DriveSet);
            }  
        }

        public async Task<DriveInfo?> GetDrive(string name) =>
            await Task.Run(() => DriveSet.Where(d => d.Name == name)
                .FirstOrDefault());

        public async Task<DriveInfo?> GetDrive(string name, string label) =>
           await Task.Run(() => GetReadyDrives()
            .Where(d => d.Name == name && d.VolumeLabel == label)
            .FirstOrDefault());

        public async Task<int> ForceRefresh() =>
            await Task.Run(async () =>
            {
                DriveSet = DriveInfo.GetDrives()
                .ToList();

                if(DriveSetChanged is not null)
                {
                    await DriveSetChanged?.Invoke(this, DriveSet);
                }
                
                return DriveSet.Count;
            });

        public async Task<IEnumerable<DriveInfo>> GetDrivesOfType(DriveType driveType) =>
           await Task.Run(() => GetReadyDrives()
            .Where(d => d.DriveType == driveType));

        public IEnumerable<DriveInfo> GetReadyDrives() =>
            DriveSet.Where(d => d.IsReady);
    }
}
