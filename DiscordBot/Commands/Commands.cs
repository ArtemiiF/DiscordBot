using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DiscordBot.Back;
using DSharpPlus;
using DiscordBot.Back.VoiceChannel;
using DSharpPlus.VoiceNext;
using DSharpPlus.Entities;
using DSharpPlus.Net;
using DSharpPlus.Lavalink;
using System.Linq;

namespace DiscordBot.Commands
{
    public class Commands : BaseCommandModule
    {

        [Command("h")]
        public async Task HelpCommand(CommandContext ctx)
        {
            await ctx.RespondAsync("\\play,\\p <YouTube link> - play video from YouTube(audio)");
        }

        [Command("join"), Aliases("j")]
        public async Task Join(CommandContext ctx)
        {
            Console.WriteLine("Join command");

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
        }


        [Command("Play"), Aliases("p")]
        public async Task Play(CommandContext ctx, [RemainingText] string link)
        {
            Console.WriteLine("Play command");
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync("You are not in a voice channel.");
                return;
            }

            var lava = ctx.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (conn != null)
            {
                var loadResult = await node.Rest.GetTracksAsync(link);
                if (loadResult.LoadResultType == LavalinkLoadResultType.LoadFailed
                    || loadResult.LoadResultType == LavalinkLoadResultType.NoMatches)
                {
                    await ctx.RespondAsync($"{link} search failed...");
                    return;
                }

                var track = loadResult.Tracks.First();

                await conn.PlayAsync(track);

                await ctx.RespondAsync($"Now playing {track.Title}!");
            }

        }

        [Command("Skip"), Aliases("s")]
        public async Task Skip(CommandContext ctx)
        {

        }

        [Command("leave"), Aliases("l")]
        public async Task LeaveCommand(CommandContext ctx)
        {
            Console.WriteLine("Leave command");
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

        }
    }
}
