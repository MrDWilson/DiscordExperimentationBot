using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordExperimentationBot.Services;

namespace DiscordExperimentationBot
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup(string[] args)
        {
            var builder = new ConfigurationBuilder()        // Create a new instance of the config builder
                .SetBasePath(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName)      // Specify the default location for the config file
                .AddJsonFile("_configuration.json");        // Add this (json encoded) file to the configuration
            Configuration = builder.Build();                // Build the configuration
        }

        /// <summary>
        /// This is the entry point from Program.cs. It will make a new instance of this class, and the start the class running.
        /// the 'static' in the name means that is can be called without making an instance. So Startup.RunAsync() instead of
        /// Startup exampme = new Startup(); example.RunAsync()
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task RunAsync(string[] args)
        {
            var startup = new Startup(args); //use the above constructor to make an instance of this class
            await startup.RunAsync(); //then call the below function on this new instance
        }

        public async Task RunAsync()
        {
            var services = new ServiceCollection();             // Create a new instance of a service collection
            ConfigureServices(services); //Call the function below this one to setup all of the services needed to run

            var provider = services.BuildServiceProvider();     // Build the service provider
            provider.GetRequiredService<LoggingService>();      // Start the logging service
            provider.GetRequiredService<CommandHandler>(); 		// Start the command handler service

            await provider.GetRequiredService<StartupService>().StartAsync();       // Start the startup service

            /*
             * Now that we have the logging service, the command handler and the startup service all running, we want to keep 
             * them going so that the bot stays on rather than shutting down here. the 'await' is an async command, meaning
             * that the program will continue past this point until the call is completed. This is useful in programs
             * that call web services, as you can say make a web request, and as this may take a few seconds to get the 
             * required data, the await tells the program to continue running code. E.g., the command above this comment. 
             * Normally, the code would halt on that line until the StartAsync function has finished, but in this case, 
             * the await tells it to keep going, and it will do its thing when it is completed. 
             * 
             * The reason why we have used this below, is that Task.Delay makes the program halt for a given time (-1 is
             * infinite in this case, so this line would halt forever), but if this line halts forever, the program would get stuck
             * here and freeze as it is being told to wait. The await means that it can go past this line, and continue running,
             * but the delay of -1 will never finish, so the code will never go past this line in this function, meaning the program
             * will never stop.
             */
            await Task.Delay(-1);                               // Keep the program alive
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
            {                                       // Add discord to the collection
                LogLevel = LogSeverity.Verbose,     // Tell the logger to give Verbose amount of info (this means a lot of logging, good for debugging)
                MessageCacheSize = 1000             // Cache 1,000 messages per channel
            }))
            .AddSingleton(new CommandService(new CommandServiceConfig
            {                                       // Add the command service to the collection
                LogLevel = LogSeverity.Verbose,     // Tell the logger to give Verbose amount of info
                DefaultRunMode = RunMode.Async,     // Force all commands to run async by default
                CaseSensitiveCommands = false       // Ignore case when executing commands
            }))
            .AddSingleton<StartupService>()         // Add startupservice to the collection
            .AddSingleton<LoggingService>()         // Add loggingservice to the collection
            .AddSingleton<CommandHandler>()         // Add commandhandler to the collection
            .AddSingleton<Random>()                 // Add random to the collection
            .AddSingleton(Configuration);           // Add the configuration to the collection
        }
    }
}
