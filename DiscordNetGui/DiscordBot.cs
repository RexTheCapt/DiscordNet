using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordNet;

namespace DiscordNetGui
{
    internal class DiscordBot
    {
        private readonly DiscordSocketClient _client;
        private string Token => BotUser.Default.Token;
        private string Prefix => BotUser.Default.Prefix;

        public DiscordBot()
        {
            // It is recommended to Dispose of a client when you are finished
            // using it, at the end of your app's lifetime.
            _client = new DiscordSocketClient();

            _client.Log += LogAsync;
            _client.Ready += ReadyAsync;
            _client.MessageReceived += MessageReceivedAsync;
        }

        private Task ReadyAsync()
        {
            Console.WriteLine($"{_client.CurrentUser} is connected!");

            return Task.CompletedTask;
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        public async Task MainAsync()
        {
            // Tokens should be considered secret data, and never hard-coded.
            await _client.LoginAsync(TokenType.Bot, Token, false);
            await _client.StartAsync();

            // Block the program until it is closed.
            await Task.Delay(-1);
        }

        private async Task MessageReceivedAsync(SocketMessage message)
        {
            Log(message.Content, false);
        }

        private void Log(string message, bool useIndent = true, ConsoleColor color = ConsoleColor.Gray)
        {
            Console.ForegroundColor = color;

            string time = DateTime.Now.ToLongTimeString();
            string indent = "";

            if (useIndent)
            {
                for (int i = 0; i < time.Length; i++) indent += " ";

                time = indent;
            }

            Console.WriteLine($"{time} {message}");
        }
    }
}