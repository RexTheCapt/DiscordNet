#region usings

using System.Threading.Tasks;
using Discord.Commands;

#endregion

namespace DiscordNet.CommandModules
{
    public class ImitateModule : ModuleBase<SocketCommandContext>
    {
        [Command("imitate")]
        [Summary("Imitate")]
        public async Task TemplateTask(params string[] parsStrings)
        {
            if (Context.Message.Author.Id == BotUser.Default.OwnerID)
            {
                string s = "";

                foreach (string ss in parsStrings)
                {
                    s += $"{ss} ";
                }

                await Context.Message.DeleteAsync();
                await ReplyAsync(s);
            }
        }
    }
}