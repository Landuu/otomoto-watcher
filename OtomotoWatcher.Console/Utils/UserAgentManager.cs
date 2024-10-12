using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtomotoWatcher.Console.Utils
{
    internal class UserAgentManager
    {
        private const string DataFilePath = "Data/UserAgents.txt";
        private readonly Random _random = new();

        public List<string> UserAgents { get; private set; } = [];

        public UserAgentManager()
        {
            if (!File.Exists(DataFilePath))
                throw new Exception($"File {DataFilePath} not found");

            var lines = File.ReadAllLines(DataFilePath);

            foreach (var line in lines)
            {
                if (line.StartsWith("mozilla", StringComparison.InvariantCultureIgnoreCase))
                    UserAgents.Add(line);
            }

            if (UserAgents.Count == 0)
                throw new Exception("UserAgents missing (should start with: mozilla)");
        }

        public string GetOne()
        {
            return UserAgents[_random.Next(0, UserAgents.Count)];
        }
    }
}
