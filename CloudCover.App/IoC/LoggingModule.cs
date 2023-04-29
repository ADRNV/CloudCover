using Ninject.Modules;
using Serilog;
using Serilog.Events;

namespace CloudCover.App.IoC
{
    public class LoggingModule : NinjectModule
    {
        public override void Load()
        {
            ILogger logger = new LoggerConfiguration()
               .WriteTo
               .Console(LogEventLevel.Debug)
               .MinimumLevel.Debug()
               .CreateLogger();

            this.Bind<ILogger>()
                .ToConstant(logger);
        }
    }
}
