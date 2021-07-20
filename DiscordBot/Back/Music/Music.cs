using DSharpPlus.CommandsNext;
using DSharpPlus.Lavalink;
using DSharpPlus.Lavalink.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace DiscordBot.Back
{
    class Music
    {
        private static bool pauseFlag = false;
        private static bool queueHandlerFlag = true;
        private static List<LavalinkTrack> lavalinkTracks = new List<LavalinkTrack>();

        private async static Task QueueMusicHandler(LavalinkGuildConnection sender, TrackFinishEventArgs e)
        {
            if (lavalinkTracks.Count > 0)
                lavalinkTracks.RemoveAt(0);

            if (lavalinkTracks.Count > 0)
                await sender.PlayAsync(lavalinkTracks[0]);
        }

        public async static Task PlayMusic(CommandContext ctx, string link)
        {
            Console.WriteLine("PlayMusic");

            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync("You are not in a voice channel.");
                return;
            }

            await VoiceChannel.VoiceChannel.JoinChannel(ctx);

            var lava = ctx.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (conn != null)
            {
                if (queueHandlerFlag)
                {
                    conn.PlaybackFinished += QueueMusicHandler;
                    queueHandlerFlag = false;
                }

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

        public async static Task SkipMusic(CommandContext ctx)
        {
            Console.WriteLine("SkipMusic");

            if(lavalinkTracks.Count == 0 || ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync("Nothing to skip and/or you are not in voicechannel");
                return;
            }

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
           
            pauseFlag = false;
        }

        public async static Task StopMusic(CommandContext ctx)
        {
            Console.WriteLine("StopMusic");
            if(lavalinkTracks.Count == 0)
            {
                await ctx.RespondAsync("No track to stop/resume");
                return;
            }

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
