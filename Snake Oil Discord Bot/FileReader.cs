using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Snake_Oil_Discord_Bot
{
    class FileReader
    {
        static Dictionary<string, string> GetSettings()
        {
            Dictionary<string, string> settings = new Dictionary<string, string>();

            string line;
            StreamReader file = new StreamReader("\\TextFiles\\Settings.txt");

            while ((line = file.ReadLine()) != null)
            {
                string[] pair = line.Split('=');
                settings.Add(pair[0], pair[1]);
            }
            return settings;
        }
    }
}