using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace DiscordNet.CommandModules
{
    public class PausePlayModule : ModuleBase<SocketCommandContext>
    {
        private readonly string[] _pauseStrings = new[]
            {"I see I am no longer needed, goodbye.", "As you wish", "Cya l8tr!"};

        private readonly string[] _continueStrings = {"Thank you.", "Yay, I'm back!", "I'm going to server now."};

        [Command("pause")]
        [Summary("Pause the bot so it won't accept commands (limited command)")]
        public async Task Pause(string id)
        {
            if (Context.Message.Author.Id == BotUser.Default.OwnerID)
            {
                if (BotInfo.InstanceId.Id() != id.Trim()) return;
                await ReplyAsync(RandomPauseText());
                BotInfo.BotIsPaused = true;
                return;
            }

            await ReplyAsync($"I am sorry {Context.Message.Author.Mention}, but you are naught but a lesser soul.");
        }

        [Command("continue")]
        [Summary("Same as pause but in reverse (limited command)")]
        public async Task Continue(string id)
        {
            if (Context.Message.Author.Id == BotUser.Default.OwnerID)
            {
                if (id != BotInfo.InstanceId.Id()) return;
                await ReplyAsync(RandomContinueText());
                BotInfo.BotIsPaused = false;
            }
        }

        private string RandomContinueText()
        {
            Random rdm = new Random();
            return _continueStrings[rdm.Next(_continueStrings.Length)];
        }

        private string RandomPauseText()
        {
            Random rdm = new Random();
            return _pauseStrings[rdm.Next(_pauseStrings.Length)];
        }
    }
}