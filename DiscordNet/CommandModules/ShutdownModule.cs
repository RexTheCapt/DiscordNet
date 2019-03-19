﻿#region usings

using System;
using System.Threading.Tasks;
using Discord.Commands;

#endregion

namespace DiscordNet.CommandModules
{
    public class ShutdownModule : ModuleBase<SocketCommandContext>
    {
        [Command("shutdown")]
        [Summary("Shuts down the bot. Only for the owner of the bot.")]
        public async Task Shutdown(string instanceID)
        {
            if (instanceID == BotInfo.InstanceId.Id() && Context.Message.Author.Id == BotUser.Default.OwnerID)
            {
                await ReplyAsync("Shutting down...");
                Environment.Exit(0);
            }
        }
    }
}