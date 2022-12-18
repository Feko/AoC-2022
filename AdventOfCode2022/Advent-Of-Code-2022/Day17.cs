using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Xunit;

namespace TestProject1;

public class Day17
{
    public static char[,] Matrix = new char[0, 0];
    public const char OCCUPIED = '#';
    public const char EMPTY = ' ';
    public const int WIDTH = 7;
    public static long MAX_HEIGHT = 0;
    public static long DIFFERENCE_ACCUMULATOR = 0;
    //public static long BUFFER_SIZE = 1000;
    public static long BUFFER_SIZE = 10_000_000;

    public abstract class Shape
    {
        public long Top { get; set; }
        public int Left { get; set; }
        public char[,] Piece { get; set; }
        public Shape(long top, int left)
        {
            Top = top;
            Left = left;
        }

        public char MatrixAt(long line, int column)
        {
            var pos = BUFFER_SIZE - (MAX_HEIGHT - line - DIFFERENCE_ACCUMULATOR);
            return Matrix[pos, column];
        }

        public void SaveMatrixAt(long line, int column, char c)
            => Matrix[BUFFER_SIZE - (MAX_HEIGHT - line - DIFFERENCE_ACCUMULATOR), column] = c;

        public void Move(char c)
        {
            if (c == '>')
                PushRight();
            if (c == '<')
                PushLeft();
        }

        public void PushLeft()
        {
            if (CanMoveLeft())
                Left--;
        }

        public void PushRight()
        {
            if (CanMoveRight())
                Left++;
        }

        public virtual bool CanMoveLeft()
        {
            if (Left == 0)
                return false;
            for (int i = 0; i < Height(); i++)
            {
                if (Piece[i, 0] == OCCUPIED && MatrixAt(Head() + i, Left - 1) == OCCUPIED)
                    return false;
            }
            return true;
        }

        public virtual bool CanMoveRight()
        {
            if (Left + Width() == WIDTH)
                return false;

            for (int i = 0; i < Height(); i++)
            {
                if (MatrixAt(Head() + i, Left + Width()) == OCCUPIED)
                    return false;
            }
            return true;
        }

        public virtual bool CanGoDown()
        {
            if (Top == MAX_HEIGHT - 1)
                return false;

            for (int i = 0; i < Width(); i++)
            {
                if (MatrixAt(Top + 1, Left + i) == OCCUPIED)
                    return false;
            }

            return true;
        }

        public void Print()
        {
            for (int line = 0; line < Height(); line++)
            {
                int position = Convert.ToInt32(BUFFER_SIZE - (MAX_HEIGHT - DIFFERENCE_ACCUMULATOR - Head()) + 1 + line);
                Console.SetCursorPosition(Left + 2, position);
                for (int column = 0; column < Width(); column++)
                    Console.Write(Piece[line, column]);
            }
        }

        public void Save()
        {
            for (int line = 0; line < Height(); line++)
            {
                for (int column = 0; column < Width(); column++)
                {
                    if (Piece[line, column] == OCCUPIED)
                    {
                        SaveMatrixAt(Head() + line, Left + column, OCCUPIED);
                    }
                }
            }
        }

        public void GoDown() => Top++;
        public int Width() => Piece.GetLength(1);
        public int Height() => Piece.GetLength(0);
        public long Head() => Top - Piece.GetLength(0) + 1;
        public long Bottom() => Top;

    }

    public class HorizontalLine : Shape
    {
        public HorizontalLine(long top, int left) : base(top, left)
        {
            Piece = new char[1, 4] { { '#', '#', '#', '#' } };
        }
    }

    public class PlusSign : Shape
    {
        public PlusSign(long top, int left) : base(top, left)
        {
            Piece = new char[3, 3]{  {' ', '#', ' '},
                                    {'#', '#', '#'},
                                    {' ', '#', ' '} };
        }

        public override bool CanGoDown()
        {
            if (Top == MAX_HEIGHT - 1)
                return false;

            if (MatrixAt(Top, Left) == OCCUPIED || MatrixAt(Top + 1, Left + 1) == OCCUPIED || MatrixAt(Top, Left + 2) == OCCUPIED)
                return false;

            return true;
        }

        public override bool CanMoveLeft()
        {
            if (Left == 0)
                return false;

            if (MatrixAt(Head(), Left) == OCCUPIED || MatrixAt(Head() + 1, Left - 1) == OCCUPIED || MatrixAt(Bottom(), Left) == OCCUPIED)
                return false;
            return true;
        }

        public override bool CanMoveRight()
        {
            if (Left + Width() == WIDTH)
                return false;

            if (MatrixAt(Head(), Left + Width() - 1) == OCCUPIED
                || MatrixAt(Head() + 1, Left + Width()) == OCCUPIED
                || MatrixAt(Bottom(), Left + Width() - 1) == OCCUPIED)
                return false;
            return true;
        }
    }

    public class RightBottomCorner : Shape
    {
        public RightBottomCorner(long top, int left) : base(top, left)
        {
            Piece = new char[3, 3]{  {' ', ' ', '#'},
                                    {' ', ' ', '#'},
                                    {'#', '#', '#'} };
        }
    }

    public class VerticalLine : Shape
    {
        public VerticalLine(long top, int left) : base(top, left)
        {
            Piece = new char[4, 1] { { '#' }, { '#' }, { '#' }, { '#' } };
        }
    }

    public class CubeBlock : Shape
    {
        public CubeBlock(long top, int left) : base(top, left)
        {
            Piece = new char[2, 2]{  {'#', '#'},
                                    {'#', '#'} };
        }
    }

    private IEnumerable<T> GetInfiniteSeries<T>(IEnumerable<T> items)
    {
        while (true)
        {
            foreach (var item in items)
                yield return item;
        }
    }

    void ShowMatrix()
    {
        Console.Write("  ");
        for (int column = 0; column < Matrix.GetLength(1); column++)
            Console.Write(column % 10);

        int rows = 0;

        for (int y = 0; y < Matrix.GetLength(0); y++)
        {
            Console.Write($"\n{rows}|");
            rows = rows == 9 ? 0 : rows + 1;
            for (int x = 0; x < Matrix.GetLength(1); x++)
                Console.Write(Matrix[y, x]);
            Console.Write("|");
        }
        Console.Write($"\n +-------+");
    }

    public Shape GetPiece(char piece, long currentHeight)
    {
        int left = 2;
        currentHeight = currentHeight - 3;
        return piece switch
        {
            '-' => new HorizontalLine(currentHeight, left),
            '+' => new PlusSign(currentHeight, left),
            'L' => new RightBottomCorner(currentHeight, left),
            '|' => new VerticalLine(currentHeight, left),
            '#' => new CubeBlock(currentHeight, left),
            _ => throw new NotImplementedException("Yo man, WTF")
        };
    }

    public void Debug(Shape piece)
    {
        Console.SetCursorPosition(0, 0);
        ShowMatrix();
        piece.Print();

        Thread.Sleep(50);

    }

    [Fact]
    public void Part1()
    {
        //Console.Clear();
        int expected = 3068; var text = System.IO.File.ReadAllText("Inputs/day17.txt");
        //int expected = 3068;  var text = ">>><<><>><<<>><>>><<<>>><<<><<<>><>><<>>";
        var pieces = "-+L|#";
        var infiniteDirections = GetInfiniteSeries(text.ToArray()).GetEnumerator();
        var infinitePieces = GetInfiniteSeries(pieces.ToArray()).GetEnumerator();

        int amountPieces = 2022;
        Matrix = new char[amountPieces * 4, WIDTH];
        for (int line = 0; line < amountPieces * 4; line++)
            for (int column = 0; column < WIDTH; column++)
                Matrix[line, column] = EMPTY;

        long currentHeight = Matrix.GetLength(0) - 1;

        for (int i = 0; i < amountPieces; i++)
        {
            infinitePieces.MoveNext();
            var piece = GetPiece(infinitePieces.Current, currentHeight);
            infiniteDirections.MoveNext();
            piece.Move(infiniteDirections.Current);
            //Debug(piece);
            do
            {
                if (piece.CanGoDown())
                {
                    piece.GoDown();
                    //Debug(piece);
                }
                infiniteDirections.MoveNext();
                piece.Move(infiniteDirections.Current);
                //Debug(piece);
            }
            while (piece.CanGoDown());

            piece.Save();
            currentHeight = Math.Min(currentHeight, piece.Head() - 1);
        }

        //Console.SetCursorPosition(0, 0);
        //ShowMatrix();
        //Console.SetCursorPosition(0, 45);
        long maxHeight = (amountPieces * 4) - currentHeight - 1;
        Assert.Equal(expected, maxHeight);
        Console.WriteLine("The height is: " + maxHeight);
    }


    [Fact]
    public void Part2()
    {
        Console.Clear();
        //int expected = 3068; var text = System.IO.File.ReadAllText("Inputs/day17.txt");
        long expected = 3068; var text = ">>><<><>><<<>><>>><<<>>><<<><<<>><>><<>>";
        var pieces = "-+L|#";
        var infiniteDirections = GetInfiniteSeries(text.ToArray()).GetEnumerator();
        var infinitePieces = GetInfiniteSeries(pieces.ToArray()).GetEnumerator();

        long amountPieces = 2022;
        //int matrixBufferSize = 100;

        char [,] GetCleanMatrix()
        {
            var newMatrix = new char[BUFFER_SIZE, WIDTH];
            for (int line = 0; line < BUFFER_SIZE; line++)
                for (int column = 0; column < WIDTH; column++)
                    newMatrix[line, column] = EMPTY;
            return newMatrix;
        }
        int ResetMatrix(long amountLinesToKeep)
        {
            var newMatrix = GetCleanMatrix();
            int nonEmptyLines = 0;
            for (int line = 1; line <= amountLinesToKeep; line++)
            {
                bool lineIsEmpty = true;
                for (int column = 0; column < WIDTH; column++)
                {
                    newMatrix[BUFFER_SIZE - line, column] = Matrix[amountLinesToKeep - line, column];
                    if (lineIsEmpty && newMatrix[BUFFER_SIZE - line, column] == OCCUPIED)
                        lineIsEmpty = false;
                }

                if (lineIsEmpty)
                {
                    break;
                }
                nonEmptyLines++;
            }
            Matrix = newMatrix; 
            return nonEmptyLines;
        }

        Matrix = GetCleanMatrix();

        long hugeAssNumber = 1_000_000_000_000;
        MAX_HEIGHT = 4 * hugeAssNumber;
        long currentHeight = MAX_HEIGHT -1;
        
        long linesToKeep = Math.Max(10, BUFFER_SIZE / 20);
        long threshold = BUFFER_SIZE - (linesToKeep /2) - 1;

        amountPieces = hugeAssNumber;

        for (long i = 0; i < amountPieces; i++)
        {
            if (i % 1_000_000 == 0)
                Console.WriteLine("Current: " + i + ", height at " + (MAX_HEIGHT - currentHeight));

            infinitePieces.MoveNext();
            var piece = GetPiece(infinitePieces.Current, currentHeight);
            infiniteDirections.MoveNext();
            piece.Move(infiniteDirections.Current);
            //Debug(piece);
            do
            {
                if (piece.CanGoDown())
                {
                    piece.GoDown();
                    //Debug(piece);
                }
                infiniteDirections.MoveNext();
                piece.Move(infiniteDirections.Current);
                //Debug(piece);
            }
            while (piece.CanGoDown());

            piece.Save();
            currentHeight = Math.Min(currentHeight, piece.Head() - 1);

            var heightDiff = MAX_HEIGHT - DIFFERENCE_ACCUMULATOR - currentHeight;
            //var heightDiff = MAX_HEIGHT + DIFFERENCE_ACCUMULATOR - currentHeight;
            if (heightDiff > threshold)
            {
                int nonEmpty = ResetMatrix(linesToKeep);
                //heightOnLastReset = MAX_HEIGHT - (currentHeight + heightOnLastReset);
                DIFFERENCE_ACCUMULATOR = DIFFERENCE_ACCUMULATOR + (heightDiff - nonEmpty) -1;
            }

            //if (MAX_HEIGHT - (currentHeight + heightOnLastReset) > threshold)
            //{
            //    int nonEmpty = ResetMatrix(linesToKeep);
            //    heightOnLastReset = MAX_HEIGHT - (currentHeight + heightOnLastReset);
            //    DIFFERENCE_ACCUMULATOR = DIFFERENCE_ACCUMULATOR + (heightOnLastReset - nonEmpty) - 1;
            //}

        }

        Console.SetCursorPosition(0, 0);
        //ShowMatrix();
        //Console.SetCursorPosition(0, 45);
        long maxHeightWeReached = MAX_HEIGHT - currentHeight - 1;
        //Assert.Equal(expected, maxHeightWeReached);
        Console.WriteLine("The height is: " + maxHeightWeReached);
    }
}