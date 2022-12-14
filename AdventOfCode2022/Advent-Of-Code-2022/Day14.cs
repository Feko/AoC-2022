using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestProject1
{
    public class Day14
    {
        private const char WALL = '#';
        private const char SAND = 'o';
        private const char EMPTY = '.';

        public char[,] Matrix { get; set; }
        public int XPositionModifier { get; set; }

        private void CreateFloor()
        {
            var sizeX = Matrix.GetLength(1);
            var sizeY = Matrix.GetLength(0);
            for (int i = 0; i < sizeX; i++)
                Matrix[sizeY - 1, i] = WALL;
        }

        public void InitializeMatrix(int minX, int maxX, int minY, int maxY)
        {
            int sizeY = maxY + 3;

            // Well, not exactly horizontally infinite - but the maximum possible size, plus a little buffer
            int sizeX = (maxX - minX) + (maxY + minX) + (maxY + maxX) + 4;

            XPositionModifier = (sizeX / 2) * -1;
            Matrix = new char[sizeY, sizeX];

            for (int x = 0; x < sizeX; x++)
                for (int y = 0; y < sizeY; y++)
                    Matrix[y, x] = EMPTY;
        }

        public (int X, int Y) NormalizePosition(int x, int y) => (x - XPositionModifier + 2, y);

        public void AddWall(int x, int y)
        {
            var pos = NormalizePosition(x, y);
            Matrix[pos.Y, pos.X] = WALL;
        }

        public bool IsOutbound(int y) => y == Matrix.GetLength(0) - 1;

        public void AddSand(int x, int y) => Matrix[y, x] = SAND;

        public (int X, int Y) GetStartingSandPositiong() => NormalizePosition(500, 0);

        public (int X, int Y) GetNextSandPosition((int x, int y) pos)
        {
            //Are we outbounds?
            if(IsOutbound(pos.y))
                return pos;

            //Can we go down?
            if (Matrix[pos.y + 1, pos.x] == EMPTY)
                return GetNextSandPosition((pos.x, pos.y + 1));

            //Can we go left?
            if (Matrix[pos.y + 1, pos.x - 1] == EMPTY)
                return GetNextSandPosition((pos.x - 1, pos.y + 1));

            //Can we go right?
            if (Matrix[pos.y + 1, pos.x + 1] == EMPTY)
                return GetNextSandPosition((pos.x + 1, pos.y + 1));

            return pos;
        }

        public void PopulatePaths(List<List<(int X, int Y)>> paths)
        {
            foreach (var path in paths)
            {
                for (int index = 0; index < path.Count() - 1; index++)
                {
                    var left = path[index];
                    var right = path[index + 1];

                    if (left.X == right.X)
                    {
                        // Y-axis is changing
                        (int min, int max) = (Math.Min(left.Y, right.Y), Math.Max(left.Y, right.Y));
                        for (int i = min; i <= max; i++)
                            AddWall(left.X, i);

                    }
                    else
                    {
                        // X-axis is changing
                        (int min, int max) = (Math.Min(left.X, right.X), Math.Max(left.X, right.X));
                        for (int i = min; i <= max; i++)
                            AddWall(i, left.Y);

                    }
                }
            }
        }

        private void InitializeCaveMap(string[] lines)
        {
            var paths = lines.Select(line => line.Split(" -> ").Select(path => { var parts = path.Split(','); return (int.Parse(parts[0]), int.Parse(parts[1])); }).ToList()).ToList();

            var allParts = paths.SelectMany(x => x);
            (int minX, int maxX) = (allParts.Min(x => x.Item1), allParts.Max(x => x.Item1));
            (int minY, int maxY) = (allParts.Min(x => x.Item2), allParts.Max(x => x.Item2));

            InitializeMatrix(minX, maxX, minY, maxY);
            PopulatePaths(paths);
        }

        [Fact]
        public void Day14_Part1()
        {
            int expected = 745; var lines = System.IO.File.ReadAllLines("Inputs/day14.txt");
            //int expected = 24; var lines = System.IO.File.ReadAllLines("Inputs/day14_sample.txt");
            InitializeCaveMap(lines);

            int amountSand = 0;

            while (true)
            {
                var nextPos = GetNextSandPosition(GetStartingSandPositiong());
                if (IsOutbound(nextPos.Y))
                    break;

                AddSand(nextPos.X, nextPos.Y);
                amountSand++;
            }

            Assert.Equal(expected, amountSand);
        }

        [Fact]
        public void Day14_Part2()
        {
            int expected = 27551; var lines = System.IO.File.ReadAllLines("Inputs/day14.txt");
            //int expected = 93; var lines = System.IO.File.ReadAllLines("Inputs/day14_sample.txt");
            InitializeCaveMap(lines);

            CreateFloor();

            int amountSand = 0;

            while (true)
            {
                amountSand++;

                var pos = GetStartingSandPositiong();
                var nextPos = GetNextSandPosition(pos);
                if (pos.X == nextPos.X && pos.Y == nextPos.Y)
                    break;

                AddSand(nextPos.X, nextPos.Y);
            }

            Assert.Equal(expected, amountSand);
        }
    }
}
