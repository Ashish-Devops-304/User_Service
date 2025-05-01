namespace UserService
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webHost => { webHost.UseStartup<Startup>(); })
            .ConfigureLogging(builder =>
            {
                builder.AddLog4Net("log4net.config");
                builder.SetMinimumLevel(LogLevel.Information);
            });

    }
}