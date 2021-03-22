using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace Snake_Oil_Discord_Bot
{
    class Program
    {
        public static void Main(string[] args)
        => new Program().MainAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();

            _client.Log += Log;

            string token = "";

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            _client.MessageReceived += readMessages;

            // Block this task until the program is closed.
            bool running = true;
            while (running)
            {
                string s = Console.ReadLine();
                if (s == "close")
                {
                    running = false;
                    await _client.LogoutAsync();
                }
            }
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private Task readMessages(SocketMessage message)
        {
            Console.WriteLine(message.Content);
            if (message.Content.Contains("boo"))
                message.Channel.SendMessageAsync("whaaa", false);
            return Task.CompletedTask;
        }
    }
}
