using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace TestProject1
{
    public class Day22
    {
        const char EMPTY = ' ';
        const char NAVIGABLE = '.';
        const char WALL = '#';

        char[,] Grid = new char[0, 0];
        FacingDirection CurrentFacing = FacingDirection.Right; 
        int CurrentLine = 0;
        int CurrentColumn = 0;
        WrapStrategy WrapHandler;

        [Fact]
        public void Day22_Part1()
        {
            //int expected = 5031; var text = File.ReadAllText("Inputs/day22_sample.txt");
            int expected = 73346; var text = File.ReadAllText("Inputs/day22.txt");
            var parts = text.Split("\n\n");
            var gridLines = parts[0].Split("\n");
            var movements = parts[1];

            CreateGrid(gridLines);

            WrapHandler = new Part1Wrap(Grid);

            PerformMovements(movements);
            int score = (1000 * (CurrentLine + 1)) + (4 * (CurrentColumn + 1)) + ((int)CurrentFacing);
            Assert.Equal(expected, score);
        }

        [Fact]
        public void Day22_Part2()
        {
            //var text = File.ReadAllText("Inputs/day22_sample.txt");
            int expected = 106392; var text = File.ReadAllText("Inputs/day22.txt");
            var parts = text.Split("\n\n");
            var gridLines = parts[0].Split("\n");
            var movements = parts[1];

            CreateGrid(gridLines);

            WrapHandler = new Part2MyInputWrap(Grid);
            //WrapHandler = new Part2ExampleWrap(Grid);

            PerformMovements(movements);
            int score = (1000 * (CurrentLine + 1)) + (4 * (CurrentColumn + 1)) + ((int)CurrentFacing);
            Assert.Equal(expected, score);
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

        private bool IsOutbounds(Direction pos) => WrapHandler.IsOutbounds(pos);

        private Direction WrapAround(Direction direction) => WrapHandler.Wrap(direction);

        private Direction MoveOnDirection(Direction pos)
        {
            return CurrentFacing switch
            {
                FacingDirection.Right => new Direction(pos.line, pos.column +1, CurrentFacing),
                FacingDirection.Left => new Direction(pos.line, pos.column -1, CurrentFacing),
                FacingDirection.Up => new Direction(pos.line -1, pos.column, CurrentFacing),
                FacingDirection.Down => new Direction(pos.line +1, pos.column, CurrentFacing),
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
                CurrentFacing = pos.direction;
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

        private Direction GetNewPosition()
        {
            Direction newPosition = new Direction(CurrentLine, CurrentColumn, CurrentFacing);
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
    }
}
