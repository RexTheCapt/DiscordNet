﻿#region usings

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

#endregion

namespace DiscordNet
{
    internal class Program
    {
        private readonly DiscordSocketClient _client;

        private static string Token => $"{BotUser.Default.Token}";

        private CommandService _commands;

        #region Startup functions

        private Program()
        {
            // It is recommended to Dispose of a client when you are finished
            // using it, at the end of your app's lifetime.
            _client = new DiscordSocketClient();

            _client.Log += LogAsync;
            _client.Ready += ReadyAsync;
            _client.MessageReceived += HandleCommandAsync;
        }
        // Discord.Net heavily utilizes TAP for async, so we create
        // an asynchronous context from the beginning.
        private static void Main()
        {
            Console.Title = "Discord Bot";

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
                         $"## Please replace the text <token> with your bot token. Please input your owner ID to get full access (require bot restart).\n" +
                         $"\n" +
                         $"BotToken=<token>\n" +
                         $"OwnerID=<owner>\n");

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
            
            Log.Info($"Instance ID: {BotInfo.InstanceId}");

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

                if (!optionFound)
                {
                    Log.Error("OwnerID not found in BotToken.txt");
                }
            }

            #endregion

            _commands = new CommandService();

            await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: null);

            // Block the program until it is closed.
            await Task.Delay(-1);
        }
        private Task ReadyAsync()
        {
            Log.Info($"{_client.CurrentUser} is connected!");

            return Task.CompletedTask;
        }

        #endregion

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
            var channel = messageParam.Channel;
            if (message == null)
                return;

            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;

            // Determine if the message is a command based on the prefix and make sure no bots trigger commands
            if (!(channel is IDMChannel) &&
                (!(message.HasStringPrefix(BotInfo.Version, ref argPos) ||
                   message.HasMentionPrefix(_client.CurrentUser, ref argPos)) || message.Author.IsBot))
            return;

            // Create a WebSocket-based command context based on the message
            SocketCommandContext context = new SocketCommandContext(_client, message);

            // Execute the command with the command context we just
            // created, along with the service provider for precondition checks.
            await _commands.ExecuteAsync(context: context, argPos: argPos, services: null);
        }
    }
}