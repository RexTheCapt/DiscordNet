#region usings

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

#endregion

namespace DiscordNet.CommandModules
{
    public class GetUserModule : ModuleBase<SocketCommandContext>
    {
        [Command("get-user")]
        [Summary("Search for a user thru every servers the bot is connected to.")]
        public async Task GetUser(string searchName)
        {
            EmbedBuilder embed = new EmbedBuilder();
            List<SocketGuildUser> matchList = new List<SocketGuildUser>();
            long totalUserCount = 0;

            searchName = searchName.Trim();
            embed.WithTitle($"Found users by search: {searchName}");

            foreach (SocketGuild guild in Context.User.MutualGuilds)
            {
                IReadOnlyCollection<SocketGuildUser> guildUsers = guild.Users;

                foreach (SocketGuildUser user in guildUsers)
                {
                    if (!$"{user} {user.Nickname} {user.Id}".ToLower().Contains(searchName.ToLower()))
                        continue;
                    if (matchList.Any(e => e.Id == user.Id))
                        continue;

                    matchList.Add(user);
                    totalUserCount++;
                }
            }

            string userString = "";

            foreach (SocketGuildUser user in matchList)
            {
                userString += $"{(user.Nickname != null ? $"\"{user.Nickname}\"" : "")} {user} {user.Id}\n";
            }

            embed.AddField($"Found users:", userString);

            embed.AddField("Total hits: ", totalUserCount, true);

            await ReplyAsync(embed: embed.Build());
        }
    }
}