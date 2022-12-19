using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestProject1
{
    public class Day18
    {
        public record Cube
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Z { get; set; }

            public int ExposedSides { get; set; } = 6;
            public void DecreaseExposedSides() => ExposedSides--;

            public IList<(int x, int y, int z)> GetNeighbours() => GetNeighbours(GetPos());
            public static IList<(int x, int y, int z)> GetNeighbours((int X, int Y, int Z) position)
            {
                return new List<(int x, int y, int z)>()
                {
                    (position.X + 1, position.Y, position.Z),
                    (position.X - 1, position.Y, position.Z),
                    (position.X, position.Y + 1, position.Z),
                    (position.X, position.Y - 1, position.Z),
                    (position.X, position.Y, position.Z + 1),
                    (position.X, position.Y, position.Z - 1)
                };
            }

            public (int x, int y, int z) GetPos() => (X, Y, Z);

            public static Cube Parse(string line)
            { 
                var parts = line.Split(',');
                return new Cube()
                {
                    X = int.Parse(parts[0]),
                    Y = int.Parse(parts[1]),
                    Z = int.Parse(parts[2]),
                };
            }

            public static Cube FromPos((int x, int y, int z) pos)
            {
                return new Cube()
                {
                    X = pos.x,
                    Y = pos.y,
                    Z = pos.z,
                };
            }

         }

        [Fact]
        public void Day18_Part1()
        {
            int expected = 4580; var lines = File.ReadAllLines("Inputs/day18.txt");
            //int expected = 64; var lines = File.ReadAllLines("Inputs/day18_sample.txt");

            Dictionary<(int x, int y, int z), Cube> cubes = new Dictionary<(int x, int y, int z), Cube>(lines.Length);

            foreach (var line in lines)
            {
                Cube cube = Cube.Parse(line);
                cubes.Add(cube.GetPos(), cube);
            }

            int totalExposed = GetSurface(cubes);
            Assert.Equal(expected, totalExposed);
        }

        public int GetSurface(Dictionary<(int x, int y, int z), Cube> cubes)
        {
            foreach (var (_, cube) in cubes)
            {
                foreach (var neighbour in cube.GetNeighbours())
                {
                    if (cubes.ContainsKey(neighbour))
                        cube.DecreaseExposedSides();
                }
            }

            var totalExposed = cubes.Sum(x => x.Value.ExposedSides);
            return totalExposed;
        }

        [Fact]
        public void Day18_Part2()
        {
            int expected = 2610; var lines = File.ReadAllLines("Inputs/day18.txt");
            //int expected = 58; var lines = File.ReadAllLines("Inputs/day18_sample.txt");

            var cubes = GetCubes(lines);
            (int min, int max) = GetMinMaxDimensions(cubes);

            var emptyCubes = GetInverseCube(cubes, min, max);


            //Flood the empty cube coors
            var startCoord = (min, min, min);
            Flood(startCoord, emptyCubes);

            var exteriorSurface = GetSurface(cubes);

            // Inner cubes are not touched.
            var innerCubes = emptyCubes.Where(x => !x.Value).ToDictionary(k => k.Key, v => Cube.FromPos(v.Key));
            var innerSurface = GetSurface(innerCubes);

            var result = exteriorSurface - innerSurface;

            Assert.Equal(expected, result);

        }

        private (int min, int max) GetMinMaxDimensions(Dictionary<(int x, int y, int z), Cube> cubes)
        {
            (int minX, int minY, int minZ) = (cubes.Keys.Min(k => k.x), cubes.Keys.Min(k => k.y), cubes.Keys.Min(k => k.z));
            (int maxX, int maxY, int maxZ) = (cubes.Keys.Max(k => k.x), cubes.Keys.Max(k => k.y), cubes.Keys.Max(k => k.z));
            int min = Math.Min(minX, Math.Min(minY, minZ)) - 1;
            int max = Math.Max(maxX, Math.Max(maxY, maxZ)) + 1;
            return (min, max);
        }

        private static Dictionary<(int x, int y, int z), Cube> GetCubes(string[] lines)
        {
            Dictionary<(int x, int y, int z), Cube> cubes = new Dictionary<(int x, int y, int z), Cube>(lines.Length);

            foreach (var line in lines)
            {
                Cube cube = Cube.Parse(line);
                cubes.Add(cube.GetPos(), cube);
            }

            return cubes;
        }

        private static Dictionary<(int x, int y, int z), bool> GetInverseCube(Dictionary<(int x, int y, int z), Cube> cubes, int min, int max)
        {
            //This create an "inverse matrix" of cubes: Ignore the cubes from file, create only the "empty spaces" of cubes.
            // the idea is to initialize them with false. As we traverse them and adjacent, mark as true. 
            Dictionary<(int x, int y, int z), bool> emptyCubeCoordinates = new();

            for (int x = min; x <= max; x++)
            {
                for (int y = min; y <= max; y++)
                {
                    for (int z = min; z <= max; z++)
                    {
                        if (!cubes.ContainsKey((x, y, z)))
                            emptyCubeCoordinates.Add((x, y, z), false);
                    }
                }
            }

            return emptyCubeCoordinates;
        }

        private void Flood((int x, int y, int z) startPos, Dictionary<(int x, int y, int z), bool> emptyCubeCoords)
        {
            // From a initial position, mark it as true, and process neighbours.
            // As it was created with an inverse from the actual cubes, the inner ones will remain untouched (false)
            var queue = new Queue<(int x, int y, int z)>();
            queue.Enqueue(startPos);

            while (queue.Any())
            {
                var current = queue.Dequeue();
                emptyCubeCoords[current] = true;
                foreach (var neighbour in Cube.GetNeighbours(current))
                {
                    if (emptyCubeCoords.ContainsKey(neighbour) && !emptyCubeCoords[neighbour] && !queue.Contains(neighbour))
                        queue.Enqueue(neighbour);
                }
            }
        }
    }
}
