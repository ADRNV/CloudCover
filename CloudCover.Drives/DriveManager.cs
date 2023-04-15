﻿using CloudCover.Core.Managers;
using System.Management;

namespace CloudCover.Drives
{
    public class DriveManager : IDriveManager
    {
        private readonly ManagementEventWatcher _watcher = new ManagementEventWatcher();

        private List<DriveInfo> _drives = new();

        public event Func<object, IEnumerable<DriveInfo>, Task> DriveSetChanged;

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

        private async void OnDriveSetChanged(object sender, EventArrivedEventArgs e)
        {
            await Task.Run(() => _drives = DriveInfo.GetDrives()
                .Where(d => d.IsReady)
                .ToList());

            await DriveSetChanged.Invoke(this, _drives);
        }

        public async Task<DriveInfo?> GetDrive(string name) =>
            await Task.Run(() => _drives.Where(d => d.Name == name)
                .FirstOrDefault());

        public async Task<DriveInfo?> GetDrive(string name, string label) =>
           await Task.Run(() => GetReadyDrives()
            .Where(d => d.Name == name && d.VolumeLabel == label)
            .FirstOrDefault());

        public async Task<IEnumerable<DriveInfo>> GetDrivesOfType(DriveType driveType) =>
           await Task.Run(() => GetReadyDrives()
            .Where(d => d.DriveType == driveType));

        public IEnumerable<DriveInfo> GetReadyDrives() =>
            _drives.Where(d => d.IsReady);
    }
}
