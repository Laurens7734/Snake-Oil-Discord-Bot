using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace Snake_Oil_Discord_Bot
{
    class FileReader
    {
        public static Dictionary<string, string> GetSettings()
        {
            Dictionary<string, string> settings = new Dictionary<string, string>();

            string line;
            StreamReader file = new StreamReader("TextFiles/Settings.txt");

            while ((line = file.ReadLine()) != null)
            {
                string[] pair = line.Split('=');
                settings.Add(pair[0], pair[1]);
            }
            return settings;
        }

        public static async Task<List<string>> GetWordsAsync()
        {
            string words = await File.ReadAllTextAsync("TextFiles/WordList.txt");
            return new List<string>(words.Split("\n"));
        }

        public static async Task<List<string>> GetRolesAsync()
        {
            string words = await File.ReadAllTextAsync("TextFiles/CustomerList.txt");
            return new List<string>(words.Split("\n"));
        }
    }
}