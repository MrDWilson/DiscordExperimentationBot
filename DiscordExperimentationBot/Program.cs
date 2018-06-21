using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordExperimentationBot
{
    class Program
    {
        /// <summary>
        /// This file and main function is the first thing the program runs on startup.
        /// It will run the RunAsync function from startup, which will connec to discord server,
        /// initialise the bot and leave it running. Open Startup.cs to see this process.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static Task Main(string[] args)
            => Startup.RunAsync(args); 
    }
}
