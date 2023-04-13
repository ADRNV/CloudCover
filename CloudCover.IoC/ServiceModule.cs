using CloudCover.Core.Clients;
using CloudCover.Services;
using Ninject.Modules;

namespace CloudCover.IoC
{
    public class ServiceModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<IDiskClient>()
                .To<YandexDiskClient>(); 
        }
    }
}
