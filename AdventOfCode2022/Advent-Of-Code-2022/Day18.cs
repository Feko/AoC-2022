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

            public IList<(int x, int y, int z)> GetNeighbours()
            {
                return new List<(int x, int y, int z)>()
                {
                    (X + 1, Y, Z),
                    (X - 1, Y, Z),
                    (X, Y + 1, Z),
                    (X, Y - 1, Z),
                    (X, Y, Z + 1),
                    (X, Y, Z - 1)
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

            foreach (var (_, cube) in cubes)
            {
                foreach (var neighbour in cube.GetNeighbours())
                {
                    if (cubes.ContainsKey(neighbour))
                        cube.DecreaseExposedSides();
                }
            }

            var totalExposed = cubes.Sum(x => x.Value.ExposedSides);
            Assert.Equal(expected, totalExposed);
        }
    }
}
