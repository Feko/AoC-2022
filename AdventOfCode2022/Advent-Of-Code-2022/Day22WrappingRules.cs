using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject1
{
    internal enum FacingDirection
    {
        Right = 0,
        Down = 1,
        Left = 2,
        Up = 3,
    }
    internal record Direction(int line, int column, FacingDirection direction);

    internal interface WrapStrategy
    {
        Direction Wrap(Direction direction);
        bool IsOutbounds(Direction direction);
    }

    internal class Part1Wrap : WrapStrategy
    {
        public char[,] Grid { get; }

        public Part1Wrap(char[,] grid)
        {
            Grid = grid;
        }

        public bool IsOutbounds(Direction pos)
        {
            return pos.line < 0 || pos.column < 0 || pos.line >= Grid.GetLength(0) || pos.column >= Grid.GetLength(1);
        }

        public Direction Wrap(Direction direction)
        {
            (int line, int column) newPos = (direction.line, direction.column);

            if (newPos.line < 0)
                newPos.line = Grid.GetLength(0) - 1;
            if (newPos.line >= Grid.GetLength(0))
                newPos.line = 0;
            if (newPos.column < 0)
                newPos.column = Grid.GetLength(1) - 1;
            if (newPos.column >= Grid.GetLength(1))
                newPos.column = 0;

            return new Direction(newPos.line, newPos.column, direction.direction);
        }
    }

    internal class Part2ExampleWrap : WrapStrategy
    {
        const char EMPTY = ' ';
        public char[,] Grid { get; }

        public Part2ExampleWrap(char[,] grid)
        {
            Grid = grid;
        }

        public bool IsOutbounds(Direction pos)
        {
            return pos.line < 0 || pos.column < 0 || pos.line >= Grid.GetLength(0) || pos.column >= Grid.GetLength(1) || Grid[pos.line, pos.column] == EMPTY;
        }

        public Direction Wrap(Direction pos)
        {
            int pieceSize = 4;
            (int newLine, int newColumn, FacingDirection newDirection) result = pos switch
            {
                //1 intersects 2
                { line: -1 } => (4, 3 - (pos.column % pieceSize), FacingDirection.Down), //1 intersects 2, going from 1 to 2
                { line: 3, column: <= 3 } => (0, 11 - pos.column, FacingDirection.Down), //1 intersects 2, going from 2 to 1

                //5 intersects 2
                { line: 12, column: <= 11 } => (7, 3 - (pos.column % pieceSize), FacingDirection.Up), //5 intersects 2, going from 5 to 2
                { line: 8, column: <= 3 } => (11, 11 - pos.column, FacingDirection.Up), //5 intersects 2, going from 2 to 5

                //2 intersects 6
                { column: < 0 } => (11, 15 - (pos.line % pieceSize), FacingDirection.Up), //2 intersects 6, going from 2 to 6
                { line: 12, column: > 11 } => (7 - (pos.column % pieceSize), 0, FacingDirection.Right), //2 intersects 6, going from 6 to 2

                //6 intersects 1
                { column: 16 } => (3 - (pos.line % pieceSize), 11, FacingDirection.Left), //6 intersects 1, going from 6 to 1
                { line: <= 3, column: 12 } => (11 - (pos.line % pieceSize), 15, FacingDirection.Left), //6 intersects 1, going from 1 to 6

                //5 intersects 3
                { line: >= 8, column: 7 } => (7, 7 - (pos.line % pieceSize), FacingDirection.Up), //5 intersects 3, going from 5 to 3
                { line: 8, column: >= 4 } => (11 - (pos.column % pieceSize), 8, FacingDirection.Right), //5 intersects 3, going from 3 to 5

                //3 intersects 1
                { line: 3, column: >= 4 } => (pos.column % pieceSize, 8, FacingDirection.Right), //3 intersects 1, going from 3 to 1
                { line: <= 3, column: 7 } => (4, 4 + pos.line, FacingDirection.Down), //3 intersects 1, going from 1 to 3

                //4 intersects 6
                { column: 12 } => (8, 15 - (pos.line % pieceSize), FacingDirection.Down), //4 intersects 6, going from 4 to 6
                { line: 7, column: >= 12 } => (7 - (pos.column % pieceSize), 11, FacingDirection.Left), //4 intersects 6, going from 6 to 4

                _ => throw new NotImplementedException()
            };
            return new Direction(result.newLine, result.newColumn, result.newDirection);

        }
    }

    internal class Part2MyInputWrap : WrapStrategy
    {
        const char EMPTY = ' ';
        public char[,] Grid { get; }

        public Part2MyInputWrap(char[,] grid)
        {
            Grid = grid;
        }

        public bool IsOutbounds(Direction pos)
        {
            return pos.line < 0 || pos.column < 0 || pos.line >= Grid.GetLength(0) || pos.column >= Grid.GetLength(1) || Grid[pos.line, pos.column] == EMPTY;
        }

        public Direction Wrap(Direction pos)
        {
            //This rules are hardcoded for my input. Unless other inputs are in the same format, it's not going to work
            /* My input had this shape:
             * |----|
             * |  ##
             * |  #
             * | ##
             * | #
             * |----|
             
             */
            int pieceSize = 50;
            (int newLine, int newColumn, FacingDirection newDirection) result = pos switch
            {
                //1 intersects 4
                { line: <= 49, column: 49 } => (149 - (pos.line % pieceSize), 0, FacingDirection.Right), //1 intersects 4, going from 1 to 4
                { line: <= 149, column: -1 } => (49 - (pos.line % pieceSize), 50, FacingDirection.Right), //1 intersects 4, going from 4 to 1

                //1 intersects 6
                { line: -1, column: <= 99 } => (150 + (pos.column % pieceSize), 0, FacingDirection.Right), //1 intersects 6, going from 1 to 6
                { line: >= 150, column: -1 } => (0, 50 + (pos.line % pieceSize), FacingDirection.Down), //1 intersects 6, going from 6 to 1

                //2 intersects 3
                { line: 50, column: >= 100, direction: FacingDirection.Down } => (50 + (pos.column % pieceSize), 99, FacingDirection.Left), //2 intersects 3, going from 2 to 3
                { line: <= 99, column: 100, direction: FacingDirection.Right } => (49, 100 + (pos.line % pieceSize), FacingDirection.Up), //2 intersects 3, going from 3 to 2

                //2 intersects 5
                { column: 150 } => (149 - (pos.line % pieceSize), 99, FacingDirection.Left), //2 intersects 5, going from 2 to 5
                { line: >= 100, column: 100 } => (49 - (pos.line % pieceSize), 149, FacingDirection.Left), //2 intersects 5, going from 5 to 2

                //2 intersects 6
                { line: -1, column: >= 100 } => (199, 0 + (pos.column % pieceSize), FacingDirection.Up), //2 intersects 6, going from 2 to 6
                { line: 200 } => (0, 100 + (pos.column % pieceSize), FacingDirection.Down), //2 intersects 6, going from 6 to 2

                //3 intersects 4
                { line: >= 50, column: 49, direction: FacingDirection.Left } => (100, 0 + (pos.line % pieceSize), FacingDirection.Down), //3 intersects 4, going from 3 to 4
                { line: 99, column: <= 49, direction: FacingDirection.Up } => (50 + (pos.column % pieceSize), 50, FacingDirection.Right), //3 intersects 4, going from 4 to 3

                //5 intersects 6
                { line: 150, column: >= 50, direction: FacingDirection.Down } => (150 + (pos.column % pieceSize), 49, FacingDirection.Left), //5 intersects 6, going from 5 to 6
                { line: >= 150, column: 50, direction: FacingDirection.Right } => (149, 50 + (pos.line % pieceSize), FacingDirection.Up), //5 intersects 6, going from 6 to 5

                _ => throw new NotImplementedException()
            };
            return new Direction(result.newLine, result.newColumn, result.newDirection);

        }
    }
}
