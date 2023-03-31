namespace CloudCover.Core.Managers
{
    public interface IDriveManager
    {
        public event Action<object, IEnumerable<DriveInfo>> OnDriveSetChanged;
        public DriveInfo? GetDrive(string name);

        public DriveInfo? GetDrive(string name, string label);

        public IEnumerable<DriveInfo> GetDrivesOfType(DriveType driveType);

        public IEnumerable<DriveInfo> GetReadyDrives();
    }
}
