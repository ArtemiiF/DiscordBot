using DSharpPlus.CommandsNext;
using DSharpPlus.Lavalink;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.Back
{
    class Music
    {
        private static bool pauseFlag = false;
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
            await VoiceChannel.VoiceChannel.JoinChannel(ctx);
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
