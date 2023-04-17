using Ninject.Modules;
using Serilog.Events;
using Serilog;

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
