using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Lavalink;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace DiscordBot.Back.VoiceChannel
{
    class VoiceChannel
    {
        static public bool inChannelFlag = false;

        public async static Task JoinChannel(CommandContext ctx)
        {
            Console.WriteLine("Join command");

            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync("You are not in a voice channel.");
                return;
            }

            if (inChannelFlag)
                return;

            var channel = ctx.Member.VoiceState.Channel;

            var lava = ctx.Client.GetLavalink();
            if (!lava.ConnectedNodes.Any())
            {
                await ctx.RespondAsync("The Lavalink connection is not established");
                return;
            }

            var node = lava.ConnectedNodes.Values.First();

            if (channel.Type != ChannelType.Voice)
            {
                await ctx.RespondAsync("Not a valid voice channel.");
                return;
            }           

            await node.ConnectAsync(channel);
            await ctx.RespondAsync($"Joined {channel.Name}!");
            inChannelFlag = true;
        }

        public async static Task LeaveChannel(CommandContext ctx)
        {
            Console.WriteLine("Leave command");

            if (!inChannelFlag)
            {
                await ctx.RespondAsync("I'm not in channel ;-)");
                return;
            }

            var channel = ctx.Member.VoiceState.Channel;
            var lava = ctx.Client.GetLavalink();

            if (!lava.ConnectedNodes.Any())
            {
                await ctx.RespondAsync("The Lavalink connection is not established");
                return;
            }

            var node = lava.ConnectedNodes.Values.First();

            if (channel.Type != ChannelType.Voice)
            {
                await ctx.RespondAsync("Not a valid voice channel.");
                return;
            }

            var conn = node.GetGuildConnection(channel.Guild);

            if (conn == null)
            {
                await ctx.RespondAsync("Lavalink is not connected.");
                return;
            }

            await conn.DisconnectAsync();
            await ctx.RespondAsync($"Left {channel.Name}!");
            inChannelFlag = false;
        }
    }
}
