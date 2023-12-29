using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC25
{
    internal class Machine
    {
        public Dictionary<string, List<string>> Connections { get; set; } = new Dictionary<string, List<string>>();
        public Dictionary<string, string> Disconnected { get; set; } = new Dictionary<string, string>();
        public int Hash { get; set; }

        public Machine(string[] lines)
        {
            // Populate base list
            foreach (var line in lines)
            {
                var parts = line.Split(": ");
                Connections[parts[0]] = parts[1].Split(" ").ToList();
            }

            // Populate reverse direction
            var tmpKeys = new List<string>(Connections.Keys);
            foreach (var con in tmpKeys)
            {
                foreach (var dst in Connections[con])
                {
                    if (!Connections.ContainsKey(dst))
                        Connections[dst] = new List<string>();

                    Connections[dst].Add(con);
                }
            }
        }

        private Machine(Dictionary<string, List<string>> connctions, Dictionary<string, string> disconnected, int hash)
        {
            Connections = connctions;
            Hash = hash;
        }

        public Machine Copy()
        {
            var disCopy = new Dictionary<string, string>(Disconnected);
            return new Machine(Connections, disCopy, Hash);
        }

        public void RemoveConnection(string c1, string c2)
        {
            Disconnected[c1] = c2;
            Disconnected[c2] = c1;
        }

        public int CountConnected()
        {
            string start = Connections.Keys.First();
            var visited = new HashSet<string>();
            var frontier = new Stack<string>();
            frontier.Push(start);
            while (frontier.Count > 0)
            {
                var cur = frontier.Pop();
                if (visited.Contains(cur))
                    continue;
                visited.Add(cur);
                foreach (var conn in Connections[cur])
                {
                    if(visited.Contains(conn))
                        continue;
                    if (Disconnected.ContainsKey(cur) && Disconnected[cur] ==  conn)
                        continue;
                    frontier.Push(conn);
                }
            }
            return visited.Count();
        }

        public override bool Equals(object? obj)
        {
            return Hash == Hash;
        }

        public override int GetHashCode()
        {
            return Hash;
        }
    }
}
