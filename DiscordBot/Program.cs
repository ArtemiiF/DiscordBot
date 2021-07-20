using DSharpPlus;
using System.Threading.Tasks;
using System.Configuration;
using DSharpPlus.CommandsNext;
using DSharpPlus.VoiceNext;
using DSharpPlus.Net;
using DSharpPlus.Lavalink;
using Microsoft.Extensions.Logging;

namespace DiscordBot
{
    class Program
    {
        public static DiscordClient botClient;
        static void Main(string[] args)
        {
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        internal static async Task MainAsync(string[] args)
        {
            botClient = new DiscordClient(new DiscordConfiguration()
            {
                Token = ConfigurationManager.AppSettings.Get("BotApiKey"),
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged,
                MinimumLogLevel = LogLevel.Debug
            });

            var endpoint = new ConnectionEndpoint
            {
                Hostname = "127.0.0.1",
                Port = 2333
            };

            var lavalinkConfig = new LavalinkConfiguration
            {
                Password = "youshallnotpass", 
                RestEndpoint = endpoint,
                SocketEndpoint = endpoint
            };
            
            botClient.UseVoiceNext();

            var lavalink = botClient.UseLavalink();

            var commands = botClient.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new[] { "\\" },               
            });

            commands.RegisterCommands<Commands.Commands>();

            await botClient.ConnectAsync();
            await lavalink.ConnectAsync(lavalinkConfig);
            await Task.Delay(-1);
        }
    }

}
