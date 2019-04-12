#region usings

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordNet;

#endregion

namespace StreamBot.CommandModules
{
    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        [Command("info")]
        [Summary("Get some information about this bot.")]
        public async Task Info()
        {
            IReadOnlyCollection<SocketGuild> guilds = Context.User.MutualGuilds;
            SocketGuildUser owner = null;
            TimeSpan timeActive = DateTime.Now - BotInfo.StartDateTime;
            EmbedBuilder embedBuilder = new EmbedBuilder().WithTitle("Bot Info");

            foreach (SocketGuild socketGuild in guilds)
            {
                foreach (SocketGuildUser user in socketGuild.Users)
                    if (user.Id == BotUser.Default.OwnerID)
                    {
                        owner = user;
                        break;
                    }

                if (owner != null)
                    break;
            }

            embedBuilder.WithColor(1f, 0f, 0f);

            embedBuilder.AddField("Version", $"{BotInfo.Version}")
                .AddField("Instance ID", $"{BotInfo.InstanceId.Id()}{(BotInfo.BotIsPaused ? " (paused)" : "")}")
                .AddField("Owner", $"{(owner == null ? "[NONE]#****" : $"{owner}")}")
                .AddField("Start time",
                    $"{BotInfo.StartDateTime.ToLongDateString()} {BotInfo.StartDateTime.ToLongTimeString()}")
                .AddField("Active Time",
                    $"{timeActive.TotalDays:00}:{timeActive.Hours:00}:{timeActive.Minutes:00}:{timeActive.Seconds:00}.{timeActive.Milliseconds:000}");

            await ReplyAsync(embed: embedBuilder.Build());
        }
    }
}