using System;
using System.Collections.Generic;
using System.Text;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace Snake_Oil_Discord_Bot
{
    class Commands
    {
        public static Func<SocketMessage, SnakeOil, Task> ReadDiscordCommand(string command)
        {
            string[] commandText = command.ToLower().Split(" ");
            switch (commandText[0])
            {
                case "start": return StartGame;
                case "join": return AddPlayer;
                case "leave": return RemovePlayer;
                case "next": return NextRound;
                case "endgame": return EndGame;
                default: return CommandNotFound;
            }
        }

        public static async Task CommandNotFound(SocketMessage message, SnakeOil game)
        {
            await message.Channel.SendMessageAsync("Unknown command");
        }
        #region DiscordCommands
        public static async Task StartGame(SocketMessage message, SnakeOil game)
        {
            if (game != null)
                await message.Channel.SendMessageAsync("This channel still has an ongoing game. If the game has ended please use the endgame command");

            else
            {
                SnakeOil newGame = new SnakeOil(message.Channel, message.Author);
                await newGame.GetRoles();
                await newGame.GetWords();
                Program.AddNewGame(newGame);

                await message.Channel.SendMessageAsync($"The game has been created");
            }
        }
        public static async Task AddPlayer(SocketMessage message, SnakeOil game)
        {
            if (game == null)
                await message.Channel.SendMessageAsync("There is no game going on in this channel.");

            else
            {
                int i = game.AddPlayer(message.Author);
                if (i == 0)
                {
                    if (game.GetPlayerCount() < 3)
                        await message.Channel.SendMessageAsync($"The game currently has the following players {game.GetPlayerList()} \n \n you still need more players before you can start the game");
                    else
                        await message.Channel.SendMessageAsync($"The game currently has the following players {game.GetPlayerList()}");
                }
                else
                {
                    string answer = "";
                    switch (i)
                    {
                        case 1: answer = "You already joined the game. playing as 2 people is not allowed"; break;
                        case 2: answer = "You can't join because there are not enough words for that amount of players"; break;
                        default: answer = "Someone forgot to add an errorcode. so this game may now be broken :("; break;
                    }
                    await message.Channel.SendMessageAsync(answer);
                }
            }
        }
        public static async Task RemovePlayer(SocketMessage message, SnakeOil game)
        {
            if (game == null)
                await message.Channel.SendMessageAsync("You can't leave a game if there is no game");
            else
            {
                game.RemovePlayer(message.Author);
                if (game.GetPlayerCount() < 3)
                    await message.Channel.SendMessageAsync("You left the game. Not enough players left to keep playing");
                else
                    await message.Channel.SendMessageAsync("You left the game");
            }
        }
        public static async Task NextRound(SocketMessage message, SnakeOil game)
        {
            if (game == null)
                await message.Channel.SendMessageAsync("No game exists in this channel");
            else
            {
                await game.NextRound();
                await message.Channel.SendMessageAsync("All cards have now been send out");
            }

        }
        public static async Task EndGame(SocketMessage message, SnakeOil game)
        {
            Program.RemoveGame(game);
            await message.Channel.SendMessageAsync("The game has now ended");
        }
        #endregion
    }
}
