using log4net;
using log4net.Appender;

namespace WebAppication.Infrastructure.Common
{
    public static class ConfigureLogging
    {
        public static ILog For<T>(string connectionString)
        {
            var logger = LogManager.GetLogger(typeof(T));

            var loggerRepositories = LogManager.GetAllRepositories();

            if (!loggerRepositories.Any())
                throw new Exception("Missing log4net repositories, check log4net.config file is correctly configured");

            foreach (var loggerRepository in loggerRepositories)
            {
                var appenders = loggerRepository.GetAppenders().OfType<AdoNetAppender>();

                foreach (var adoNetAppender in appenders)
                {
                    adoNetAppender.ConnectionString = connectionString;
                    adoNetAppender.ActivateOptions();
                }
            }

            return logger;
        }

    }

}
