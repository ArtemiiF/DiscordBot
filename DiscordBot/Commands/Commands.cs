using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;
using DiscordBot.Back;
using DiscordBot.Back.VoiceChannel;


namespace DiscordBot.Commands
{
    public class Commands : BaseCommandModule
    {
        [Command("h")]
        public async Task HelpCommand(CommandContext ctx)
        {
            await ctx.RespondAsync("\\play,\\p <YouTube link> - play video from YouTube(audio)\n" +
                                   "\\stop - stop\\resume track\n\\skip,\\s - skip track\n\\leave,\\l - leave voicechannel");
        }

        [Command("join"), Aliases("j")]
        public async Task Join(CommandContext ctx)
        {
            await VoiceChannel.JoinChannel(ctx);
        }

        [Command("Play"), Aliases("p")]
        public async Task Play(CommandContext ctx, [RemainingText] string link)
        {
            await Music.PlayMusic(ctx, link);
        }

        [Command("Skip"), Aliases("s")]
        public async Task Skip(CommandContext ctx)
        {
            await Music.SkipMusic(ctx);
        }

        [Command("Stop")]
        public async Task Stop(CommandContext ctx)
        {
            await Music.StopMusic(ctx);
        }

        [Command("leave"), Aliases("l")]
        public async Task LeaveCommand(CommandContext ctx)
        {
            await VoiceChannel.LeaveChannel(ctx);
        }
    }
}
