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
        public int MinX { get; set; }
        public int MinY { get; set; }

        public void InitializeMatrix(int minX, int maxX, int minY, int maxY)
        {
            (int sizeX, int sizeY) = (maxX - minX + 4, maxY  + 2);
            //(int sizeX, int sizeY) = (maxX - minX + 4, maxY - minY + 4);
            (MinX, MinY) = (minX, minY);

            Matrix = new char[sizeY, sizeX];

            for (int x = 0; x < sizeX; x++)
                for (int y = 0; y < sizeY; y++)
                    Matrix[y, x] = EMPTY;

        }

        public void ShowMatrix()
        {
            Console.Write("  ");
            for (int column = 0; column < Matrix.GetLength(1); column++)
                Console.Write(column % 10);

            int rows = 0;
            Console.Write($"\n{rows} ");
            for (int y = 0; y < Matrix.GetLength(0); y++)
            {
                rows = rows == 9 ? 0 : rows+1;
                for (int x = 0; x < Matrix.GetLength(1); x++)
                    Console.Write(Matrix[y, x]);
                Console.Write($"\n{rows} ");
            }
        }

        public (int X, int Y) NormalizePosition(int x, int y) => (x - MinX + 2, y);
        //public (int X, int Y) NormalizePosition(int x, int y) => (x - MinX + 2, y - MinY + 2);

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
                        // Y is changing
                        (int min, int max) = (Math.Min(left.Y, right.Y), Math.Max(left.Y, right.Y));
                        for (int i = min; i <= max; i++)
                        {
                            AddWall(left.X, i);
                        }
                    }
                    else
                    {
                        // X is changing
                        (int min, int max) = (Math.Min(left.X, right.X), Math.Max(left.X, right.X));
                        for (int i = min; i <= max; i++)
                        {
                            AddWall(i, left.Y);
                        }
                    }
                }
            }
        }


        [Fact]
        public void Test1()
        {
            var lines = System.IO.File.ReadAllLines("Inputs/day14.txt");
            //var lines = System.IO.File.ReadAllLines("Inputs/day14_sample.txt");
            var paths = lines.Select(line => line.Split(" -> ").Select(path => { var parts = path.Split(','); return (int.Parse(parts[0]), int.Parse(parts[1])); }).ToList()).ToList();

            var allParts = paths.SelectMany(x => x);
            (int minX, int maxX) = (allParts.Min(x => x.Item1), allParts.Max(x => x.Item1));
            (int minY, int maxY) = (allParts.Min(x => x.Item2), allParts.Max(x => x.Item2));

            //Console.WriteLine($"{nameof(minX)}{minX} - {nameof(maxX)}{maxX} - {nameof(minY)}{minY} - {nameof(maxY)}{maxY} - ");

            int amountSand = 0;
            InitializeMatrix(minX, maxX, minY, maxY);
            PopulatePaths(paths);
            //ShowMatrix();

            //for (int i = 0; i < 5; i++)
            //{
            //    var pos = GetStartingSandPositiong();
            //    var nextPos = GetNextSandPosition(pos);
            //    AddSand(nextPos.X, nextPos.Y);

            //}


            while (true)
            {
                var pos = GetStartingSandPositiong();
                var nextPos = GetNextSandPosition(pos);
                //Console.WriteLine($"Iteraction {amountSand} with position {nextPos.X} , {nextPos.Y}");
                if (IsOutbound(nextPos.Y))
                    break;

                AddSand(nextPos.X, nextPos.Y);
                amountSand++;
            }


            ShowMatrix();
            Console.WriteLine($"\nThe matrix was filled with {amountSand} sands");

            //Assert.Equal(24, amountSand);

        }
    }
}
