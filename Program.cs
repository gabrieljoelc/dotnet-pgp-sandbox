using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;

namespace PgpSandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            // create service collection
            var services = new ServiceCollection();
            ConfigureServices(services);

            // create service provider
            var serviceProvider = services.BuildServiceProvider();

            // entry to run app
            serviceProvider.GetRequiredService<App>().Run();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // build config
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });
            services.AddOptions();
            services.Configure<SecurityKeySource>(configuration.GetSection(nameof(SecurityKeySource)));
            //public string PrivateKeySource { get; set; }
            //public string PublicKeySource { get; set; }
            //public string PassPhrase { get; set; }
            services.Configure<FilePathes>(configuration.GetSection(nameof(FilePathes)));
            //public string Input { get; set; }
            //public string Output { get; set; }

            // add services:
            services.AddTransient<ISecretManager, SimpleSecretManager>();
            services.AddTransient<PgpService>();

            // add app
            services.AddTransient<App>();
        }
    }

    public class FilePathes
    {
        public string Input { get; set; }
        public string Output { get; set; }
    }

    internal class App
    {
        private FilePathes Pathes { get; }
        private PgpService Service { get; }

        public App(IOptions<FilePathes> options, PgpService service)
        {
            Pathes = options.Value;
            Service = service;
        }

        public void Run()
        {
            Console.WriteLine($"Pathes.Input: {Pathes.Input}");
            var input = Service.Encrypt(null, Pathes.Input, Pathes.Output);
            Console.WriteLine($"encrypted input: {input}");
            //var output = Service.Decrypt(null, input, Pathes.Output);
            //Console.WriteLine($"decrypted output: {output}");
        }
    }
}
