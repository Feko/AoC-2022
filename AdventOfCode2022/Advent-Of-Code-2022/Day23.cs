using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace TestProject1
{
    [Collection("Day23")]
    public class Day23
    {
        internal const char NORTH = 'N';
        internal const char SOUTH = 'S';
        internal const char EAST = 'E';
        internal const char WEST = 'W';


        internal static HashSet<(int y, int x)> ElfPositions = new();
        internal static Dictionary<(int y, int x), int> MovementPledges = new();
        internal List<Elf> AllElves = new();

        internal class Elf
        {
            public int Y { get; private set; }
            public int X { get; private set; }

            private (int y, int x)? MovementPledge { get; set; }

            public Elf(int y, int x)
            {
                Y = y;
                X = x;
            }

            public (int y, int x) GetPosition() => (Y, X);
            public bool ShouldMove()
            {
                foreach (var position in Neighbours())
                {
                    if (ElfPositions.Contains(position))
                        return true;
                }
                return false; // It is surrounded by empty spaces, so don't need to move.
            }

            public void Pledge(List<char> movementSequence)
            {
                MovementPledge = null;
                foreach (var direction in movementSequence)
                {
                    if (CanMove(direction))
                    {
                        Pledge(direction);
                        return;
                    }
                }
            }

            public void Move()
            {
                if (!MovementPledge.HasValue) // Couldn't move anywhere
                    return;

                if (MovementPledges[MovementPledge.Value] > 1) // More than one elf wanted this position
                    return;

                (Y, X) = (MovementPledge.Value.y, MovementPledge.Value.x);
            }

            private void Pledge(char direction)
            {
                var position = direction switch
                {
                    NORTH => (Y - 1, X),
                    SOUTH => (Y + 1, X),
                    EAST => (Y, X + 1),
                    WEST => (Y, X - 1),
                    _ => throw new KeyNotFoundException(),
                };

                MovementPledge = position;
                if (MovementPledges.ContainsKey(position))
                    MovementPledges[position] = MovementPledges[position] + 1;
                else
                    MovementPledges[position] = 1;
            }

            private bool CanMove(char direction)
            {
                foreach (var neighbour in GetNeighbours(direction))
                {
                    if (ElfPositions.Contains(neighbour))
                        return false;
                }
                return true;
            }

            private (int y, int x)[] GetNeighbours(char direction)
            {
                return direction switch
                {
                    NORTH => NorthNeighbours(),
                    SOUTH => SouthNeighbours(),
                    EAST => EastNeighbours(),
                    WEST => WestNeighbours(),
                    _ => throw new KeyNotFoundException(),
                };
            }

            private (int y, int x)[] NorthNeighbours() => new[] { (Y - 1, X - 1), (Y - 1, X), (Y - 1, X + 1) };
            private (int y, int x)[] SouthNeighbours() => new[] { (Y + 1, X - 1), (Y + 1, X), (Y + 1, X + 1) };
            private (int y, int x)[] EastNeighbours() => new[] { (Y - 1, X + 1), (Y, X + 1), (Y + 1, X + 1) };
            private (int y, int x)[] WestNeighbours() => new[] { (Y - 1, X - 1), (Y, X - 1), (Y + 1, X - 1) };

            private (int y, int x)[] Neighbours()
            {
                HashSet<(int y, int x)> neighbours = new(8);
                neighbours.UnionWith(NorthNeighbours());
                neighbours.UnionWith(SouthNeighbours());
                neighbours.UnionWith(WestNeighbours());
                neighbours.UnionWith(EastNeighbours());
                return neighbours.ToArray();
            }
        }

        [Fact]
        public void Day23_Part1()
        {
            int expected = 4138; var lines = System.IO.File.ReadAllLines("Inputs/day23.txt");
            // int expected = 110; var lines = System.IO.File.ReadAllLines("/home/feko/src/dotnet/aoc/test/day05/day23_sample.txt");

            CreateElves(lines);
            List<char> MoveSequence = new[] { NORTH, SOUTH, WEST, EAST }.ToList();

            // Move the elfs 10 times
            for (int i = 0; i < 10; i++)
            {
                ElfPositions = AllElves.Select(elf => elf.GetPosition()).ToHashSet();

                TheElvesLikeToMoveItMoveIt(MoveSequence);

                MoveSequence = Cycle(MoveSequence);
            }

            //Count spaces
            ElfPositions = AllElves.Select(elf => elf.GetPosition()).ToHashSet();
            (int minY, int maxY) = (AllElves.Min(elf => elf.Y), AllElves.Max(elf => elf.Y));
            (int minX, int maxX) = (AllElves.Min(elf => elf.X), AllElves.Max(elf => elf.X));
            int amount = 0;
            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    if (!ElfPositions.Contains((y, x)))
                        amount++;
                }
            }

            Assert.Equal(expected, amount);
        }
        
        [Fact]
        public void Day23_Part2()
        {
            int expected = 1010; var lines = System.IO.File.ReadAllLines("Inputs/day23.txt");
            // int expected = 20; var lines = System.IO.File.ReadAllLines("/home/feko/src/dotnet/aoc/test/day05/day23_sample.txt");

            CreateElves(lines);

            int round = 0;
            List<char> MoveSequence = new[] { NORTH, SOUTH, WEST, EAST }.ToList();
            ElfPositions = AllElves.Select(elf => elf.GetPosition()).ToHashSet();

            while (AllElves.Any(x => x.ShouldMove()))
            {
                TheElvesLikeToMoveItMoveIt(MoveSequence);

                MoveSequence = Cycle(MoveSequence);
                ElfPositions = AllElves.Select(elf => elf.GetPosition()).ToHashSet();
                round++;
            }

            Assert.Equal(expected, round + 1);
        }

        private void TheElvesLikeToMoveItMoveIt(List<char> MoveSequence)
        {
            MovementPledges = new(ElfPositions.Count);

            var elvesToMove = AllElves.Where(elf => elf.ShouldMove()).ToList();
            foreach (var elf in elvesToMove)
                elf.Pledge(MoveSequence);
            foreach (var elf in elvesToMove)
                elf.Move();
        }

        private List<char> Cycle(List<char> current)
        {
            char first = current[0];
            current.RemoveAt(0);
            current.Add(first);
            return current;
        }

        private void CreateElves(string[] lines)
        {
            for (int line = 0; line < lines.Length; line++)
            {
                for (int column = 0; column < lines[line].Length; column++)
                {
                    if (lines[line][column] == '#')
                        AllElves.Add(new Elf(line, column));
                }
            }
        }

    }
}
