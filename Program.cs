﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;

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
            serviceProvider.GetRequiredService<App>().Run(args);
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
        private ILogger<App> Log { get; }

        public App(IOptions<FilePathes> options, PgpService service, ILogger<App> log)
        {
            Pathes = options.Value;
            Service = service;
            Log = log;
        }

        public void Run(string[] args)
        {
            var firstArg = args.First();
            if (firstArg == "encrypt")
            {
              Log.LogInformation("Trying to encrypt {input} and write encrypted file to {output}...", Pathes.Input, Pathes.Output);
              var input = Service.Encrypt(null, Pathes.Input, Pathes.Output);
              Log.LogInformation("Successfully encrypted!");
            }
            if (firstArg == "decrypt")
            {
              Log.LogInformation("Trying to decrypt {input} and write decrypted file to {output}...", Pathes.Input, Pathes.Output);
              var input = Service.Encrypt(null, Pathes.Input, Pathes.Output);
              Log.LogInformation("Successfully decrypted!");
            }
        }
    }
}
