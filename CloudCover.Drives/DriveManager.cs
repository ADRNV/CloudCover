using CloudCover.Core.Managers;
using Serilog;
using System.Management;

namespace CloudCover.Drives
{
    /// <summary>
    /// Superstructure on drive manager, may react to drives in system changing
    /// </summary>
    public class DriveManager : IDriveManager
    {
        private readonly ManagementEventWatcher _watcher = new ManagementEventWatcher();

        public List<DriveInfo> DriveSet { get; private set; } = new List<DriveInfo>();

        public event Func<object, IEnumerable<DriveInfo>, Task> DriveSetChanged;

        private ILogger _logger;

        /// <summary>
        /// Crates instance
        /// </summary>
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

        /// <summary>
        /// Creates instance
        /// </summary>
        /// <param name="logger"></param>
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

        /// <summary>
        /// Gets drive with name
        /// </summary>
        /// <param name="name">Name of drive</param>
        /// <returns>Drive else - null</returns>
        public async Task<DriveInfo?> GetDrive(string name) =>
            await Task.Run(() => DriveSet.Where(d => d.Name == name)
                .FirstOrDefault());

        /// <summary>
        /// Gets drive by label and name
        /// </summary>
        /// <param name="name">Name of drive</param>
        /// <param name="label">Label of drive</param>
        /// <returns></returns>
        public async Task<DriveInfo?> GetDrive(string name, string label) =>
           await Task.Run(() => GetReadyDrives()
            .Where(d => d.Name == name && d.VolumeLabel == label)
            .FirstOrDefault());

        /// <summary>
        /// Forces refreshes drive set and invokes <see cref="DriveSetChanged"/>
        /// </summary>
        /// <returns>Count of drives</returns>
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

        /// <summary>
        /// Gets drives of <see cref="DriveType"/>
        /// </summary>
        /// <param name="driveType">Type of drive</param>
        /// <returns></returns>
        public async Task<IEnumerable<DriveInfo>> GetDrivesOfType(DriveType driveType) =>
           await Task.Run(() => GetReadyDrives()
            .Where(d => d.DriveType == driveType));

        /// <summary>
        /// Gets all ready drives
        /// </summary>
        /// <returns>Ready drives enumerable</returns>
        public IEnumerable<DriveInfo> GetReadyDrives() =>
            DriveSet.Where(d => d.IsReady);
    }
}
