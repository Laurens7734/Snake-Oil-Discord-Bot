using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using System.Collections.Generic;

namespace Snake_Oil_Discord_Bot
{
    class Program
    {
        string commandPrefix;
        static List<SnakeOil> ongoingGames;
        DateTime CheckGames;

        public static void Main(string[] args)
        => new Program().MainAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;

        public async Task MainAsync()
        {
            CheckGames = DateTime.Now.AddHours(2.0);
            _client = new DiscordSocketClient();
            _client.Log += Log;

            ongoingGames = new List<SnakeOil>();

            Dictionary<string, string> Settings = FileReader.GetSettings();
            string token = Settings["Token"];
            commandPrefix = Settings["CommandPrefix"];

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            _client.MessageReceived += ReadMessages;

            bool running = true;
            while (running)
            {
                string s = Console.ReadLine();
                if (s == "close")
                {
                    running = false;
                    await _client.LogoutAsync();
                }

                if(DateTime.Now > CheckGames)
                {
                    CheckGames = DateTime.Now.AddHours(2.0);
                    RemoveInactiveGames();
                }
            }
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private async Task ReadMessages(SocketMessage message)
        {
            if (message.Content.StartsWith(commandPrefix))
            {
                Func<SocketMessage, SnakeOil, Task> commandHandler = Commands.ReadDiscordCommand(message.Content.Substring(commandPrefix.Length));
                SnakeOil thisGame = ongoingGames.Find(x => x.GameLocation.Equals(message.Channel));
                await commandHandler(message, thisGame);
            }
        }

        void RemoveInactiveGames()
        {
            List<SnakeOil> removeableGames = new List<SnakeOil>();
            foreach(SnakeOil game in ongoingGames)
            {
                if (game.LastUse.AddMinutes(30.0) < DateTime.Now)
                {
                    removeableGames.Add(game);
                }
            }
            foreach (SnakeOil game in removeableGames)
            {
                RemoveGame(game);
            }
        }

        public static void AddNewGame(SnakeOil game)
        {
            ongoingGames.Add(game);
        }

        public static void RemoveGame(SnakeOil game)
        {
            ongoingGames.Remove(game);
        }
    }
}
