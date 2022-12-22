using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestProject1
{
    public class Day22
    {
        enum FacingDirection
        {
            Right = 0,
            Down = 1,
            Left = 2,
            Up = 3,
        }

        const char EMPTY = ' ';
        const char NAVIGABLE = '.';
        const char WALL = '#';

        char[,] Grid = new char[0, 0];
        FacingDirection CurrentFacing = FacingDirection.Right; 
        int CurrentLine = 0;
        int CurrentColumn = 0;

        [Fact]
        public void Day22_Part1()
        {
            //var text = File.ReadAllText("Inputs/day22_sample.txt");
            var text = File.ReadAllText("Inputs/day22.txt");
            var parts = text.Split("\n\n");
            var gridLines = parts[0].Split("\n");
            var movements = parts[1];

            CreateGrid(gridLines);
            PerformMovements(movements);
            Print();
            Console.WriteLine($"\n\nFinished on line {CurrentLine + 1}, column {CurrentColumn + 1} and facing  {CurrentFacing}.");
            Console.WriteLine($"Score: {(1000 * (CurrentLine + 1)) + (4 * (CurrentColumn + 1)) + ((int)CurrentFacing)}");
        }

        private void Turn(char direction)
        {
            if (direction == 'L')
            {
                CurrentFacing = CurrentFacing == FacingDirection.Right ? FacingDirection.Up : (FacingDirection)((int)CurrentFacing) - 1;
            }
            else
            {
                CurrentFacing = CurrentFacing == FacingDirection.Up ? FacingDirection.Right : (FacingDirection)((int)CurrentFacing) + 1;
            }
        }

        private bool IsOutbounds((int line, int column) pos)
        {
            return pos.line < 0 || pos.column < 0 || pos.line >= Grid.GetLength(0) || pos.column >= Grid.GetLength(1);
        }

        private (int line, int column) WrapAround((int line, int column) pos)
        {
            (int line, int column) newPos = (pos.line, pos.column);

            if (pos.line < 0)
                newPos.line = Grid.GetLength(0) - 1;
            if (pos.line >= Grid.GetLength(0))
                newPos.line = 0;
            if (pos.column < 0)
                newPos.column = Grid.GetLength(1) - 1;
            if (pos.column >= Grid.GetLength(1))
                newPos.column = 0;
            
            return newPos;
        }

        private (int line, int column) MoveOnDirection((int line, int column) pos)
        {
            return CurrentFacing switch
            {
                FacingDirection.Right => (pos.line, pos.column +1),
                FacingDirection.Left => (pos.line, pos.column -1),
                FacingDirection.Up => (pos.line -1, pos.column),
                FacingDirection.Down => (pos.line +1, pos.column),
                _ => throw new Exception("Just..... how?")
            };
        }

        private void PerformMovements(string movements)
        {
            var moves = CreateMovements(movements);
            foreach (var movement in moves)
            {
                if (string.Equals(movement, "L") || string.Equals(movement, "R"))
                    Turn(movement[0]);
                else
                    Walk(Convert.ToInt32(movement));
            }
        }

        private void Walk(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                Grid[CurrentLine, CurrentColumn] = GetPathMarker();
                var pos = GetNewPosition();
                if (Grid[pos.line, pos.column] == WALL)
                    break;

                CurrentLine = pos.line;
                CurrentColumn = pos.column;
            }
        }

        private char GetPathMarker()
        {
            return CurrentFacing switch
            {
                FacingDirection.Right => '>',
                FacingDirection.Left => '<',
                FacingDirection.Up => '^',
                FacingDirection.Down => 'V',
                _ => throw new Exception("Just..... how?")
            };
        }

        private (int line, int column) GetNewPosition()
        {
            (int line, int column) newPosition = (CurrentLine, CurrentColumn);

            do
            {
                newPosition = MoveOnDirection(newPosition);
                if(IsOutbounds(newPosition))
                    newPosition = WrapAround(newPosition);
            } 
            while (Grid[newPosition.line, newPosition.column] == EMPTY);
            return newPosition;
        }

        private List<string> CreateMovements(string movements)
        {
            List<string> moves = new();
            for (int i = 0; i < movements.Length; i++)
            {
                if (Char.IsDigit(movements[i]))
                {
                    var sb = new StringBuilder();
                    sb.Append(movements[i]);
                    //How big is this number?
                    int current = i;
                    while (current < movements.Length -1 && Char.IsDigit(movements[current + 1]))
                    {
                        current++;
                        sb.Append(movements[current]);
                    }
                    i = current;
                    moves.Add(sb.ToString());
                }
                else
                {
                    moves.Add(movements[i].ToString());
                }
            }
            return moves;
        }

        private void CreateGrid(string[] gridLines)
        {
            var columnSize = gridLines.Max(x => x.Length);
            Grid = new char[gridLines.Length, columnSize];

            CurrentColumn = gridLines[0].IndexOf(NAVIGABLE);

            for (int line = 0; line < gridLines.Length; line++)
                for (int column = 0; column < columnSize; column++)
                    Grid[line, column] = EMPTY;

            for (int line = 0; line < gridLines.Length; line++)
                for (int column = 0; column < gridLines[line].Length; column++)
                    Grid[line, column] = gridLines[line][column];
        }

        private void Print()
        {
            Console.Clear();
            for (int line = 0; line < Grid.GetLength(0); line++)
            {
                for (int column = 0; column < Grid.GetLength(1); column++)
                    Console.Write(Grid[line, column]);
                Console.Write("\n");
            }
        }
    }
}
