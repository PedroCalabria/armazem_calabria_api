using Microsoft.AspNetCore;

namespace ArmazemCalabria.API
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var currentDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var webHost = WebHost.CreateDefaultBuilder(args).UseStartup<Startup>();

                webHost.Build().Run();
            }
            catch (Exception ex)
            {
                var currentDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                throw;
            }
        }


    }
}