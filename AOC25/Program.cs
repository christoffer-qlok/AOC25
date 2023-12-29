namespace AOC25
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const int picksCount = 100;
            const int topCount = 10;

            var lines = File.ReadAllLines("input.txt");

            var machine = new Machine(lines);

            var total = machine.CountConnected();
            Console.WriteLine($"Total nodes: {total}");

            var r = new Random();
            var picks = new HashSet<Tuple<string, string>>();
            var allNodes = machine.Connections.Keys.ToList();
            for (int i = 0; i < picksCount; i++)
            {
                var first = allNodes[r.Next(allNodes.Count())];
                var second = allNodes[r.Next(allNodes.Count())];
                if(first == second || picks.Contains(new (second, first)))
                    continue;
                picks.Add(new (first, second));
            }

            Console.WriteLine($"Made {picks.Count()} path picks");

            var seen = new Dictionary<Tuple<string, string>, int>();

            foreach (var nodePair in picks)
            {
                var prev = new Dictionary<string, string>();
                var visited = new HashSet<string>();
                var frontier = new Queue<string>();
                frontier.Enqueue(nodePair.Item1);
                while(frontier.Count() > 0)
                {
                    var cur = frontier.Dequeue();
                    if (cur == nodePair.Item2)
                        break;
                    if (visited.Contains(cur))
                        continue;
                    visited.Add(cur);
                    foreach (var neighbor in machine.Connections[cur])
                    {
                        if(visited.Contains(neighbor))
                            continue;

                        if (!prev.ContainsKey(neighbor))
                            prev[neighbor] = cur;

                        frontier.Enqueue(neighbor);
                    }
                }
                var node = nodePair.Item2;
                while (prev.ContainsKey(node))
                {
                    var prevNode = prev[node];
                    List<string> keyParts = new List<string> { node, prevNode };
                    keyParts.Sort();
                    var key = new Tuple<string, string>(keyParts[0], keyParts[1]);
                    if (seen.ContainsKey(key))
                        seen[key]++;
                    else
                        seen[key] = 1;

                    node = prevNode;
                }
            }

            var topConnectionsWithCounts = seen.OrderByDescending(kvp => kvp.Value).Take(topCount).ToArray();

            Console.WriteLine("Top visited connections:");
            foreach (var kvp in topConnectionsWithCounts)
            {
                Console.WriteLine($"\t{kvp.Key}: {kvp.Value}");
            }

            var topConnections = topConnectionsWithCounts.Select(kvp => kvp.Key).ToArray();

            for (int ai = 0;ai < topConnections.Length - 2;ai++)
            {
                var a = topConnections[ai];
                for (int bi = ai + 1; bi < topConnections.Length - 1; bi++)
                {
                    var b = topConnections[bi];
                    for (int ci = bi + 1; ci < topConnections.Length; ci++)
                    {
                        var c = topConnections[ci];
                        machine.RemoveConnection(a.Item1, a.Item2);
                        machine.RemoveConnection(b.Item1, b.Item2);
                        machine.RemoveConnection(c.Item1, c.Item2);
                        var count = machine.CountConnected();
                        if(count < total)
                        {
                            Console.WriteLine($"Found cut: {string.Join(',', machine.Disconnected.Where((_, i) => i % 2 == 0))}");
                            Console.WriteLine($"Answer: {count} * {total - count} = {count * (total - count)}");
                            return;
                        }
                        machine.Disconnected.Clear();
                    }
                }
            }
            Console.WriteLine("No answer found");
        }

    }
}
