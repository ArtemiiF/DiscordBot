using DSharpPlus.CommandsNext;
using DSharpPlus.Lavalink;
using DSharpPlus.Lavalink.EventArgs;
using Emzi0767.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace DiscordBot.Back
{
    class Music
    {
        private static bool pauseFlag = false;
        private static List<LavalinkTrack> lavalinkTracks = new List<LavalinkTrack>();
        
        public async static Task PlayMusic(CommandContext ctx, string link)
        {
            await VoiceChannel.VoiceChannel.JoinChannel(ctx);

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
                conn.PlaybackFinished += QueueMusic;
                var loadResult = await node.Rest.GetTracksAsync(link);
                
                if (loadResult.LoadResultType == LavalinkLoadResultType.LoadFailed
                    || loadResult.LoadResultType == LavalinkLoadResultType.NoMatches)
                {
                    await ctx.RespondAsync($"{link} search failed...");
                    return;
                }

                var track = loadResult.Tracks.First();

                if (lavalinkTracks.Count == 0)
                {
                    await conn.PlayAsync(track);
                    await ctx.RespondAsync($"Now playing {track.Title}!");
                    lavalinkTracks.Add(track);
                }
                else
                {
                    lavalinkTracks.Add(track);
                    await ctx.RespondAsync($"{track.Title} has been added to queue!");
                }                             
            }
            await VoiceChannel.VoiceChannel.JoinChannel(ctx);
        }

        private async static Task QueueMusic(LavalinkGuildConnection sender, TrackFinishEventArgs e)
        {
            if(lavalinkTracks.Count > 0)
                lavalinkTracks.RemoveAt(0);

            if (lavalinkTracks.Count > 0)
                await sender.PlayAsync(lavalinkTracks[0]);
        }

        public async static Task SkipMusic(CommandContext ctx)
        {
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync("You are not in a voice channel.");
                return;
            }

            var lava = ctx.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            await conn.StopAsync();
            await ctx.RespondAsync("Track skipped");
        }

        public async static Task StopMusic(CommandContext ctx)
        {
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync("You are not in a voice channel.");
                return;
            }

            var lava = ctx.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if(!pauseFlag)
            {
                await conn.PauseAsync();
                pauseFlag = true;
            }
            else
            {
                await conn.ResumeAsync();   
                pauseFlag = false;
            }                
        }
     
    }
}
