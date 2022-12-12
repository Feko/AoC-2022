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
        private int GetCost(char c) => c switch
        {
            'S' => 100,
            'E' => 'z' - 96,
            _ => c - 96
        };

        private string GetNodeName(int line, int column) => $"N{line}x{column}";

        [Fact]
        public void Day06_Part1()
        {
            var lines = File.ReadAllLines("Inputs/day12.txt");
            //var lines = File.ReadAllLines("Inputs/day12_sample.txt");
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

                    Dictionary<string, int> neighbours = new();
                    int thisCost = GetCost(lines[line][column]);

                    if (line > 0 && GetCost(lines[line - 1][column]) <= thisCost + 1)
                        neighbours.Add(GetNodeName(line - 1, column), GetCost(lines[line - 1][column]));

                    if (line < lines.Length - 1 && GetCost(lines[line + 1][column]) <= thisCost + 1)
                        neighbours.Add(GetNodeName(line + 1, column), GetCost(lines[line + 1][column]));

                    if (column > 0 && GetCost(lines[line][column - 1]) <= thisCost + 1)
                        neighbours.Add(GetNodeName(line, column - 1), GetCost(lines[line][column - 1]));

                    if (column < lines[line].Length - 1 && GetCost(lines[line][column + 1]) <= thisCost + 1)
                        neighbours.Add(GetNodeName(line, column + 1), GetCost(lines[line][column + 1]));

                    graph.Add(GetNodeName(line, column), neighbours);
                }
            }
            var costMatrix = KindOfDijikistra(graph, start);

            Assert.Equal(31, costMatrix[finish].steps);
        }

        private static Dictionary<string, (string from, int cost, int steps)> KindOfDijikistra(Dictionary<string, Dictionary<string, int>> graph, string startPoint)
        {
            Dictionary<string, (string from, int cost, int steps)> costMatrix = new(graph.Count);
            costMatrix.Add(startPoint, (startPoint, 0, 0));

            string current = startPoint;
            Dictionary<string, bool> alreadyVisited = new(graph.Count);

            for (int i = 0; i < graph.Count; i++)
            {
                if (current is null)
                    continue;

                foreach (var link in graph[current])
                {
                    if (alreadyVisited.ContainsKey(link.Key))
                        continue;

                    if (link.Value > costMatrix[current].cost + 1)
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

                alreadyVisited.Add(current, true);
                current = costMatrix.Where(c => !alreadyVisited.ContainsKey(c.Key))
                    .OrderBy(x => x.Value.steps).FirstOrDefault().Key;
            }

            return costMatrix;
        }
    }
}
