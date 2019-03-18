#region usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

#endregion

namespace DiscordNet
{
    internal class Commands
    {
        public async Task<string> Prefix(string[] messageStrings, SocketMessage message, string prefix)
        {
            if (messageStrings.Length == 1)
            {
                await message.Channel.SendMessageAsync($"Prefix: `{prefix}`");
                return prefix;
            }

            if (messageStrings.Length > 2)
            {
                await message.Channel.SendMessageAsync(
                    "Too many arguments!\nUsage:```\nprefix <prefix>\n```");
                return prefix;
            }

            await message.Channel.SendMessageAsync($"Prefix changed to `{prefix}`");
            return messageStrings[1];
        }

        public async void GetUser(string[] messageStrings, DiscordSocketClient client, SocketMessage message)
        {
            if (messageStrings.Length == 2)
            {
                string userString = "";
                ulong userCountTotal = 0;

                foreach (SocketGuild clientGuild in client.Guilds)
                {
                    IReadOnlyCollection<SocketGuildUser> socketGuildUsers = clientGuild.Users;
                    List<SocketGuildUser> userMatchList = new List<SocketGuildUser>();

                    foreach (SocketGuildUser user in socketGuildUsers)
                        if (messageStrings[1] != "*" && $"{user} {user.Nickname}".ToLower().Contains(messageStrings[1].ToLower()))
                        {
                            userMatchList.Add(user);
                            userCountTotal++;
                        }
                        else if (messageStrings[1] == "*")
                        {
                            userMatchList.Add(user);
                            userCountTotal++;
                        }

                    foreach (SocketGuildUser user in userMatchList)
                    {
                        string m = $"`{user.Nickname}` {user.Username}#{user.DiscriminatorValue} {user.Id} [{clientGuild.Name}]".ToLower();
                        Log.Write(m);

                        if (userString.Length < 1800)
                        {
                            userString += m + "\n";
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                if (userString.Length >= 1800)
                    userString += $"\n.....";

                await message.Channel.SendMessageAsync($"Found users:\n```\n{userString}\n```\nTotal hits: {userCountTotal}");
                return;
            }

            await message.Channel.SendMessageAsync("Correct usage:\n```\nuid <username>\n```");
        }

        public async Task Shutdown(SocketMessage message, bool isOwner)
        {
            if (message.Channel is ITextChannel)
            {
                await message.Channel.SendMessageAsync(
                    $"Sorry, can only do that thru DM {message.Author.Mention}");
            }
            else
            {
                if (isOwner)
                {
                    await message.Channel.SendMessageAsync("Sorry, you have no access to kill me.");
                }
                else
                {
                    await message.Channel.SendMessageAsync("Good bye!");
                    Environment.Exit(0);
                }
            }
        }

        public async Task Info(string instanceId, SocketMessage message, bool isFrozen, DiscordSocketClient client, DateTime startDateTime)
        {
            IReadOnlyCollection<SocketGuildUser> socketGuildUsers = client.Guilds.First().Users;
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;
            TimeSpan timeActive = DateTime.Now - startDateTime;

            SocketGuildUser owner = null;

            foreach (SocketGuildUser user in socketGuildUsers)
            {
                if (user.Id == BotUser.Default.OwnerID)
                {
                    owner = user;
                    break;
                }
            }

            string output = "Bot Info:\n" +
                            "```\n" +
                            $"Version: {version}\n" +
                            $"Instance ID: {instanceId} {(isFrozen ? "(on ice)" : "")}\n" +
                            $"Owner: {(owner == null ? "[NONE]#****" : $"{owner.Username}#{owner.Discriminator}")}\n" +
                            $"Start: {startDateTime.ToLongDateString()} {startDateTime.ToLongTimeString()}\n" +
                            $"Online for: {timeActive.Hours:00}:{timeActive.Minutes:00}:{timeActive.Seconds:00}.{timeActive.Milliseconds:000}\n" +
                            "```";

            await message.Channel.SendMessageAsync(output);
        }

        public async Task<bool> Pause(string id, InstanceId instanceId, bool isOwner, SocketMessage message)
        {
            if (isOwner)
            {
                if (id == instanceId.Id())
                {
                    await message.Channel.SendMessageAsync("NOOO!!! I DON'T WANT TO GET FROoo.....");
                    return true;
                }

                return false;
            }

            await message.Channel.SendMessageAsync(
                $"Sorry {message.Author.Mention}, you do not have the priviledges to put me on ice.");
            return false;
        }

        public async Task<bool> Continue(string id, InstanceId instanceId, bool isOwner, SocketMessage message)
        {
            if (isOwner)
            {
                if (id == instanceId.Id())
                {
                    await message.Channel.SendMessageAsync("oooOOOZEN!!!... Oh... hello :D");
                    return false;
                }

                return true;
            }

            return true;
        }

        public async Task GetServers(DiscordSocketClient client, SocketMessage message)
        {
            string s = "";

            foreach (SocketGuild clientGuild in client.Guilds)
            {
                s += $"{clientGuild.Name}, ";
            }

            await message.Channel.SendMessageAsync(s);
        }
    }
}