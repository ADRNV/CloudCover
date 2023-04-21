namespace CloudCover.Core.Managers
{
    public interface IDriveManager
    {
        public event Func<object, IEnumerable<DriveInfo>, Task> DriveSetChanged;

        public List<DriveInfo> DriveSet { get; }

        Task<int> ForceRefresh();

        public Task<DriveInfo?> GetDrive(string name);

        public Task<DriveInfo?> GetDrive(string name, string label);

        public Task<IEnumerable<DriveInfo>> GetDrivesOfType(DriveType driveType);

        public IEnumerable<DriveInfo> GetReadyDrives();
    }
}
