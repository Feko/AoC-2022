using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestProject1
{
    public class Day12
    {
        public int GetCost(char c) => c switch
        {
            'S' => 'a' - 96,
            'E' => 'z' - 96,
            _ => c - 96
        };

        private string GetNodeName(int line, int column) => $"L{line}xC{column}";

        public class PathContext
        {
            public string Start { get; set; }
            public string Finish { get; set; }
            public Dictionary<string, Dictionary<string, int>> Graph { get; set; }
        }

        public PathContext GetGraph(string[] lines, Func<int, int, bool> comparer)
        {
            string start = "", finish = "";

            Dictionary<string, Dictionary<string, int>> graph = new();
            for (int line = 0; line < lines.Length; line++)
            {
                for (int column = 0; column < lines[line].Length; column++)
                {
                    if (lines[line][column] == 'S')
                        start = GetNodeName(line, column);

                    if (lines[line][column] == 'E')
                        finish = GetNodeName(line, column);

                    var neighbours = GetNeighbours(lines, comparer, line, column);

                    graph.Add(GetNodeName(line, column), neighbours);
                }
            }
            return new PathContext() { Start = start, Finish = finish, Graph = graph };
        }

        private Dictionary<string, int> GetNeighbours(string[] lines, Func<int, int, bool> comparer, int line, int column)
        {
            Dictionary<string, int> neighbours = new();
            int thisCost = GetCost(lines[line][column]);

            List<(int line, int column)> neighboursPosition = new()
            {
                (line -1, column),
                (line +1, column),
                (line, column -1),
                (line, column +1),
            };

            int GetPositionCost((int line, int column) pos) => GetCost(lines[pos.line][pos.column]);
            bool Inbounds((int line, int column) pos)
            {
                return pos.line >= 0 && pos.line < lines.Length &&
                    pos.column >= 0 && pos.column < lines[line].Length;
            }

            foreach (var position in neighboursPosition)
            {
                if (Inbounds(position) && comparer(GetPositionCost(position), thisCost))
                    neighbours.Add(GetNodeName(position.line, position.column), GetPositionCost(position));
            }

            return neighbours;
        }

        [Fact]
        public void Day12_Part1()
        {
            var lines = File.ReadAllLines("Inputs/day12.txt"); int expected = 462;
            //var lines = File.ReadAllLines("Inputs/day12_sample.txt"); int expected = 31;

            var context = GetGraph(lines, (next, thisCost) =>  next <= thisCost + 1);

            var costMatrix = KindOfDijikistra(context.Graph, context.Start);

            Assert.Equal(expected, costMatrix[context.Finish].steps);
        }

        [Fact]
        public void Day12_Part2()
        {
            var lines = File.ReadAllLines("Inputs/day12.txt"); int expected = 451;
            //var lines = File.ReadAllLines("Inputs/day12_sample.txt"); int expected = 29;

            var context = GetGraph(lines, (next, thisCost) => next >= thisCost -1);
            int lowest = Int32.MaxValue;
            var costMatrix = KindOfDijikistra(context.Graph, context.Finish);

            for (int line = 0; line < lines.Length; line++)
            {
                for (int column = 0; column < lines[line].Length; column++)
                {
                    if (lines[line][column] == 'a')
                    {
                        if (costMatrix.ContainsKey(GetNodeName(line, column)))
                            lowest = Math.Min(lowest, costMatrix[GetNodeName(line, column)].steps);
                    }
                }
            }

            Assert.Equal(expected, lowest);
        }

        private static Dictionary<string, (string from, int cost, int steps)> KindOfDijikistra(Dictionary<string, Dictionary<string, int>> graph, string startPoint)
        {
            Dictionary<string, (string from, int cost, int steps)> costMatrix = new(graph.Count);
            costMatrix.Add(startPoint, (startPoint, 0, 0));

            string current = startPoint;
            HashSet<string> alreadyVisited = new(graph.Count);

            for (int i = 0; i < graph.Count; i++)
            {
                if (current is null)
                    continue;

                foreach (var link in graph[current])
                {
                    if (alreadyVisited.Contains(link.Key))
                        continue;

                    int thisCost = link.Value + costMatrix[current].cost;
                    int thisSteps = 1 + costMatrix[current].steps;

                    if (!costMatrix.ContainsKey(link.Key))
                        costMatrix[link.Key] = (current, thisCost, thisSteps);
                    else
                    {
                        var existing = costMatrix[link.Key];
                        if (existing.steps > thisSteps)
                            costMatrix[link.Key] = (current, thisCost, thisSteps);
                    }
                }

                alreadyVisited.Add(current);
                current = costMatrix.Where(c => !alreadyVisited.Contains(c.Key))
                    .OrderBy(x => x.Value.steps).FirstOrDefault().Key;
            }

            return costMatrix;
        }
    }
}
