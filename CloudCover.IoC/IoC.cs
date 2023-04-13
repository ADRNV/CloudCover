using Ninject;

namespace CloudCover.IoC
{
    public class IoC
    {
        private readonly IKernel _kernelInstance = new StandardKernel(new DrivesModule(), new ServiceModule());

        public IKernel Kernel { get => _kernelInstance; }

        public T GetRequeredService<T>()
        {
            return _kernelInstance.Get<T>();
        }
    }
}