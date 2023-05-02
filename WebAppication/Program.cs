using log4net;
using log4net.Config;

namespace WebAppication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            XmlConfigurator.Configure();
            XmlConfigurator.Configure(new FileInfo("log4net.config"));
            ILog log = LogManager.GetLogger(typeof(Program));
            try
            {
                log.Info("Starting host...");
                var host = CreateHostBuilder(args).Build();
                host.Run();
            }
            catch (Exception ex)
            {
                log.Error($"Stopped program because of exception {ex}");
                throw;
            }
            finally
            {
                LogManager.Shutdown();
            }
            return;
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).ConfigureLogging(logging =>
            {
                // clear default logging providers
                logging.ClearProviders();
                logging.AddConsole();
                logging.AddDebug();
                logging.AddEventLog();
                // add more providers here
            }).ConfigureWebHostDefaults(webBuilder =>
               {
                   webBuilder.UseStartup<Startup>();
               });
    }
}
