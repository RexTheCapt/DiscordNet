using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace DiscordNet
{
    internal class Commands
    {
        private Log _log = new Log();

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
                IReadOnlyCollection<SocketGuildUser> socketGuildUsers = client.Guilds.First().Users;
                List<SocketGuildUser> userMatchList = new List<SocketGuildUser>();

                foreach (SocketGuildUser user in socketGuildUsers)
                    if ($"{user.Username}#{user.DiscriminatorValue} {user.Nickname}".ToLower()
                        .Contains(messageStrings[1].ToLower()))
                        userMatchList.Add(user);

                string userString = "";
                foreach (SocketGuildUser user in userMatchList)
                {
                    string m =
                        $"`{user.Nickname}` {user.Username}#{user.DiscriminatorValue} {user.Id}".ToLower();
                    _log.Write(m);
                    userString += m + "\n";
                }

                await message.Channel.SendMessageAsync($"Found users:\n```\n{userString}\n```");
                return;
            }

            await message.Channel.SendMessageAsync($"Correct usage:\n```\nuid <username>\n```");
        }

        public async Task Shutdown(SocketMessage message, bool IsOwner)
        {
            if (message.Channel is ITextChannel)
            {
                await message.Channel.SendMessageAsync(
                    $"Sorry, can only do that thru DM {message.Author.Mention}");
            }
            else
            {
                if (IsOwner)
                {
                    await message.Channel.SendMessageAsync($"Sorry, you have no access to kill me.");
                }
                else
                {
                    await message.Channel.SendMessageAsync("Good bye!");
                    Environment.Exit(0);
                }
            }
        }

        public async Task Info(string instanceId, SocketMessage message, bool isFrozen)
        {
            string output = $"Bot Info:\n" +
                            $"```\n" +
                            $"Instance ID: {instanceId} {(isFrozen ? $"(on ice)" : $"")}\n" +
                            $"```";

            await message.Channel.SendMessageAsync(output);
        }

        public async Task<bool> Pause(string id, InstanceID instanceId, bool isOwner, SocketMessage message)
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
            else
            {
                await message.Channel.SendMessageAsync(
                    $"Sorry {message.Author.Mention}, you do not have the priviledges to put me on ice.");
                return false;
            }
        }

        public async Task<bool> Continue(string id, InstanceID instanceId, bool isOwner, SocketMessage message)
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
    }
}