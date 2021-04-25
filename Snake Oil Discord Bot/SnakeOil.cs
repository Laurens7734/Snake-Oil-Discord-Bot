using System;
using System.Collections.Generic;
using System.Text;
using Discord.WebSocket;
using Discord;
using System.Threading.Tasks;

namespace Snake_Oil_Discord_Bot
{
    class SnakeOil
    {
        private List<string> words;
        private List<string> roles;
        private List<SocketUser> players;
        public ISocketMessageChannel GameLocation { get; }
        public DateTime LastUse;

        public SnakeOil(ISocketMessageChannel channel, SocketUser player1)
        {
            GameLocation = channel;
            players = new List<SocketUser>()
            {
                player1
            };
            LastUse = DateTime.Now;
        }

        public async Task GetWords()
        {
            words = await FileReader.GetWordsAsync();
            if (words.Count < 12)
                throw new Exception("Wordlist is to short. games can never be played");
        }

        public async Task GetRoles()
        {
            roles = await FileReader.GetRolesAsync();
            if (roles.Count < 2)
                throw new Exception("You need at least 2 roles for this game to work");
        }

        //errorcodes: 
        //0 = succes 
        //1 = player already exists
        //2 = to many players, not enough words
        public int AddPlayer(SocketUser player)
        {
            if (players.Contains(player))
                return 1;
            else if (players.Count * 6 > words.Count)
                return 2;
            else
            {
                players.Add(player);
                LastUse = DateTime.Now;
                return 0;
            }

        }

        public void RemovePlayer(SocketUser player)
        {
            LastUse = DateTime.Now;
            players.Remove(player);
        }

        public int GetPlayerCount()
        {
            return players.Count;
        }

        public string GetPlayerList()
        {
            StringBuilder sb = new StringBuilder();
            foreach (SocketUser player in players)
            {
                sb.Append($"\n {player.Username}");
            }
            return sb.ToString();
        }

        public async Task NextRound()
        {
            LastUse = DateTime.Now;
            SocketUser customer = players[0];
            players.Remove(customer);
            int[][] hands = await Task.Run(RandomCardSelect);

            for (int i = 0; i < players.Count; i++)
            {
                StringBuilder sb = new StringBuilder("You get to choose from the following words:");
                foreach (int card in hands[i])
                {
                    sb.Append($"\n {words[card]}");
                }
                IDMChannel dm = await players[i].GetOrCreateDMChannelAsync();
                await dm.SendMessageAsync(sb.ToString());
            }

            if (roles.Count < 2)
                await GetRoles();

            Random rng = new Random();
            int r1 = rng.Next(roles.Count);
            string role1 = roles[r1];
            roles.RemoveAt(r1);

            int r2 = rng.Next(roles.Count);
            string role2 = roles[r2];
            roles.RemoveAt(r2);

            IDMChannel cdm = await customer.GetOrCreateDMChannelAsync();
            await cdm.SendMessageAsync($"You are the customer. You can choose between these roles: \n{role1}\n{role2}");

            players.Add(customer);
        }

        private int[][] RandomCardSelect()
        {
            Random rng = new Random();
            List<int> numbers = new List<int>();
            for (int i = 0; i < (players.Count * 6); i++)
            {
                int num = rng.Next(0, words.Count);
                while (numbers.Contains(num))
                    num = rng.Next(0, words.Count);

                numbers.Add(num);
            }

            return SplitList(numbers);
        }

        private int[][] SplitList(List<int> numbers)
        {
            int[][] hands = new int[players.Count][];
            int[] current = new int[6];
            for (int i = 0; i < numbers.Count; i++)
            {
                current[i % 6] = numbers[i];
                if (i % 6 == 5)
                {
                    int pos = (i / 6);
                    hands[pos] = current;
                    current = new int[6];
                }

            }

            return hands;
        }
    }
}
