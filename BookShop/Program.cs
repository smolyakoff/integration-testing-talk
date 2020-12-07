using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

[assembly: InternalsVisibleTo("BookShop.Tests")]

namespace BookShop
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        }
    }
}