#region usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordNet;
using Timer = System.Timers.Timer;

#endregion

namespace StreamBot
{
    internal class Program
    {
        private static DiscordSocketClient _client;

        private CommandService _commands;

        private static string Token => $"{BotUser.Default.Token}";

        private static Task LogAsync(LogMessage log)
        {
            Log.Write(log.ToString());
            return Task.CompletedTask;
        }

        // This is not the recommended way to write a bot - consider
        // reading over the Commands Framework sample.
        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            // Don't process the command if it was a system message
            SocketUserMessage message = messageParam as SocketUserMessage;

            if (!(messageParam is SocketUserMessage))
                return;

            #region Log messages

            SocketGuildChannel guildChannel = message.Channel as SocketGuildChannel;

            if (message.Channel is ITextChannel)
            {
                Log.Write($"{(guildChannel == null ? "-[ERROR]-" : guildChannel.Guild.Name)}/{message.Channel}/{message.Author}");
                Log.Write($"{message.Content}", false);
            }
            else
            {
                Log.DmWrite($"DM/{message.Author}");
                Log.DmWrite($"{message.Content}", false);
            }

            #endregion

            if (!BotInfo.BotIsPaused || IsAnAllowedByPassCommand(message.Content))
            {
                // Create a number to track where the prefix ends and the command begins
                int argPos = 0;

                // Determine if the message is a command based on the prefix and make sure no bots trigger commands
                if (!(messageParam.Channel is IDMChannel))
                    if ((!(message.HasStringPrefix(BotInfo.Prefix, ref argPos) ||
                           message.HasMentionPrefix(_client.CurrentUser, ref argPos)) || message.Author.IsBot))
                        return;

                // Create a WebSocket-based command context based on the message
                SocketCommandContext context = new SocketCommandContext(_client, message);

                // Execute the command with the command context we just
                // created, along with the service provider for precondition checks.
                await _commands.ExecuteAsync(context, argPos, null);
            }
        }

        private static bool IsAnAllowedByPassCommand(string messageContent)
        {
            string[] commands = {"continue", "info"};

            foreach (string command in commands)
            {
                if (messageContent.StartsWith($"{BotInfo.Prefix}{command}"))
                    return true;
            }

            return false;
        }

        #region Startup functions

        private Program()
        {
            // It is recommended to Dispose of a client when you are finished
            // using it, at the end of your app's lifetime.
            _client = new DiscordSocketClient();

            _client.Log += LogAsync;
            _client.Ready += ReadyAsync;
            _client.MessageReceived += HandleCommandAsync;
            _client.UserVoiceStateUpdated += _client_UserVoiceStateUpdated;

            Timer timer = new Timer(1000);
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        private SocketUser _guestInActive;
        private DateTime _guestInActiveDateTime = DateTime.Now;
        private int activeChatTimeMax = 10;

        private async void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (_waitingVoiceChannel == null || _activeVoiceChannel == null || userVoiceInfos.Count == 0) return;

            if (userVoiceInfos[0].ConnectedDateTime.AddSeconds(activeChatTimeMax) > DateTime.Now ||
                _guestInActiveDateTime.AddSeconds(activeChatTimeMax) > DateTime.Now) return;

            if (_guestInActive != null)
            {
                // ReSharper disable once PossibleNullReferenceException
                await (_guestInActive as IGuildUser)?.ModifyAsync(x => x.Channel = _waitingVoiceChannel);
                UserVoiceInfo u = new UserVoiceInfo();
                u.Set(_guestInActive, _waitingVoiceChannel, DateTime.Now);
                userVoiceInfos.Add(u);
                _guestInActive = null;
            }

            _guestInActive = userVoiceInfos[0].User;
            // ReSharper disable once PossibleNullReferenceException
            await (userVoiceInfos[0].User as IGuildUser)?.ModifyAsync(x => x.Channel = _activeVoiceChannel);
            _guestInActiveDateTime = DateTime.Now;
            userVoiceInfos.RemoveAt(0);
        }

        private SocketVoiceChannel _activeVoiceChannel;
        private SocketVoiceChannel _waitingVoiceChannel;
        private class UserVoiceInfo
        {
            public SocketUser User;
            // ReSharper disable once NotAccessedField.Local
            public SocketVoiceChannel VoiceChannel;
            public DateTime ConnectedDateTime;

            public void Set(SocketUser user, SocketVoiceChannel voiceChannel, DateTime connectedDateTime)
            {
                User = user;
                VoiceChannel = voiceChannel;
                ConnectedDateTime = connectedDateTime;
            }
        }
        private List<UserVoiceInfo> userVoiceInfos = new List<UserVoiceInfo>();

        private async Task _client_UserVoiceStateUpdated(SocketUser user, SocketVoiceState previousVoiceState, SocketVoiceState currentVoiceState)
        {
            Log.Info($"User: {user}, prev: {previousVoiceState}, curr: {currentVoiceState.VoiceChannel}");

            if (_activeVoiceChannel == null || _waitingVoiceChannel == null)
            {
                SocketGuild guild = _client.GetGuild(566158253474709505);

                IReadOnlyCollection<SocketVoiceChannel> voiceChannels = guild.VoiceChannels;

                foreach (SocketVoiceChannel channel in voiceChannels)
                {
                    if (channel.Name == "Active")
                    {
                        _activeVoiceChannel = channel;
                    }

                    if (channel.Name == "Waiting")
                    {
                        _waitingVoiceChannel = channel;
                    }
                }

                Log.Info($"{_activeVoiceChannel} {_waitingVoiceChannel}");
            }

            UserVoiceInfo userVoice = new UserVoiceInfo();
            userVoice.Set(user, currentVoiceState.VoiceChannel, DateTime.Now);

            if (currentVoiceState.VoiceChannel == _waitingVoiceChannel)
            {
                if(!userVoiceInfos.Any(u => u.User.Id == user.Id))
                    userVoiceInfos.Add(userVoice);

                Log.Info($"Waiting users");

                foreach (UserVoiceInfo info in userVoiceInfos)
                {
                    Log.Info($"{info.User} [{info.ConnectedDateTime}]", false);
                }
            }
            else if (currentVoiceState.VoiceChannel == _activeVoiceChannel || currentVoiceState.VoiceChannel == null)
            {
                foreach (UserVoiceInfo info in userVoiceInfos.Where(u=>u.User.Id == user.Id))
                {
                    while (true)
                    {
                        if (!userVoiceInfos.Remove(info))
                            break;
                    }
                }

                Log.Info($"Waiting users");

                foreach (UserVoiceInfo info in userVoiceInfos)
                {
                    Log.Info($"{info.User} [{info.ConnectedDateTime}]", false);
                }
            }
        }

        // Discord.Net heavily utilizes TAP for async, so we create
        // an asynchronous context from the beginning.
        private static void Main()
        {
            Console.Title = "Stream Bot";

            Log.Info($"Version: {BotInfo.Version}");

            bool hasToken = false;

            while (!hasToken)
                if (File.Exists("BotToken.txt"))
                {
                    StreamReader sr = new StreamReader("BotToken.txt");
                    bool del = true;

                    while (!sr.EndOfStream)
                    {
                        string read = sr.ReadLine();

                        if (read != null && read.StartsWith("BotToken="))
                        {
                            del = false;

                            string[] tmp = read.Split('=');

                            BotUser.Default.Token = tmp[1];
                            BotUser.Default.Save();

                            hasToken = true;
                        }
                    }

                    sr.Dispose();

                    if (del)
                        File.Delete("BotToken.txt");
                }
                else
                {
                    GenerateTokenFile();
                }

            try
            {
                new Program().MainAsync().GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                if (e.ToString().Contains("401: Unauthorized"))
                {
                    Log.Error("Unauthorized... resetting token file...");
                    File.Delete("BotToken.txt");
                    GenerateTokenFile();
                }
                else
                {
                    Log.Error(e);
                }
            }
        }

        private static void GenerateTokenFile()
        {
            StreamWriter sw = new StreamWriter("BotToken.txt");
            sw.WriteLine($"## This is an autogenerated file generated at {DateTime.Now.ToLongDateString()}.\n" +
                         "## Please replace the text <token> with your bot token. Please input your owner ID to get full access (require bot restart).\n" +
                         "\n" +
                         "BotToken=<token>\n" +
                         "OwnerID=<owner>\n");

            sw.Flush();
            sw.Close();
            sw.Dispose();

            foreach (string file in Directory.GetFiles("./", "*.txt")) Console.WriteLine(file);

            Log.Info("Generated default file and opening...");

            Process.Start("BotToken.txt");

            Environment.Exit(0);
        }

        private async Task MainAsync()
        {
            // Tokens should be considered secret data, and never hard-coded.
            await _client.LoginAsync(TokenType.Bot, Token, false);
            await _client.StartAsync();

            Log.Info($"Instance ID: {BotInfo.InstanceId.Id()}");
            Log.Info($"Prefix: {BotInfo.Prefix}");

            #region Set Owner ID

            {
                bool optionFound = false;

                StreamReader sr = new StreamReader("BotToken.txt");
                while (!sr.EndOfStream)
                {
                    string read = sr.ReadLine();

                    if (read != null && read.StartsWith("OwnerID="))
                    {
                        optionFound = true;

                        try
                        {
                            string[] split = read.Split('=');
                            BotUser.Default.OwnerID = Convert.ToUInt64(split[1]);
                            BotUser.Default.Save();
                            Log.Info($"Owner ID set to {BotUser.Default.OwnerID}");
                        }
                        catch
                        {
                            Log.Error("Unable to set Owner ID");
                        }

                        break;
                    }
                }

                if (!optionFound) Log.Error("OwnerID not found in BotToken.txt");
            }

            #endregion

            _commands = new CommandService();

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);

            // Block the program until it is closed.
            await Task.Delay(-1);
        }

        private Task ReadyAsync()
        {
            Log.Info($"{_client.CurrentUser} is connected!");

            return Task.CompletedTask;
        }

        #endregion
    }
}