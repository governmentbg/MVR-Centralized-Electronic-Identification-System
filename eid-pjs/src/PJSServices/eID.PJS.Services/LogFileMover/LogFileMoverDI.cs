namespace eID.PJS.Services.LogFileMover
{
    public static class LogFileMoverDI
    {
        public static void RegisterLogFileMover(this IServiceCollection services)
        {
            services.AddOptions<LogFileMoverSettings>().BindConfiguration(nameof(LogFileMoverSettings));

            services.AddHostedService<LogFileMoverService>();
        }
    }
}
