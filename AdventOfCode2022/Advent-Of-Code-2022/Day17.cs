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

    public abstract class Shape
    {
        public int Top { get; set; }
        public int Left { get; set; }
        public char[,] Piece { get; set; }
        public Shape(int top, int left)
        {
            Top = top;
            Left = left;
        }

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
                if (Piece[i, 0] == OCCUPIED && Matrix[Head() + i, Left - 1] == OCCUPIED)
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
                if (Matrix[Head() + i, Left + Width()] == OCCUPIED)
                    return false;
            }
            return true;
        }

        public virtual bool CanGoDown()
        {
            if (Top == Matrix.GetLength(0) - 1)
                return false;

            for (int i = 0; i < Width(); i++)
            {
                if (Matrix[Top + 1, Left + i] == OCCUPIED)
                    return false;
            }

            return true;
        }

        public void Print()
        {
            for (int line = 0; line < Height(); line++)
            {
                Console.SetCursorPosition(Left + 2, Head() + 1 + line);
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
                        Matrix[Head() + line, Left + column] = OCCUPIED;
                    }
                }
            }
        }

        public void GoDown() => Top++;
        public int Width() => Piece.GetLength(1);
        public int Height() => Piece.GetLength(0);
        public int Head() => Top - Piece.GetLength(0) + 1;
        public int Bottom() => Top;

    }

    public class HorizontalLine : Shape
    {
        public HorizontalLine(int top, int left) : base(top, left)
        {
            Piece = new char[1, 4] { { '#', '#', '#', '#' } };
        }
    }

    public class PlusSign : Shape
    {
        public PlusSign(int top, int left) : base(top, left)
        {
            Piece = new char[3, 3]{  {' ', '#', ' '},
                                    {'#', '#', '#'},
                                    {' ', '#', ' '} };
        }

        public override bool CanGoDown()
        {
            if (Top == Matrix.GetLength(0) - 1)
                return false;

            if (Matrix[Top, Left] == OCCUPIED || Matrix[Top + 1, Left + 1] == OCCUPIED || Matrix[Top, Left + 2] == OCCUPIED)
                return false;

            return true;
        }

        public override bool CanMoveLeft()
        {
            if (Left == 0)
                return false;

            if (Matrix[Head(), Left] == OCCUPIED || Matrix[Head() + 1, Left - 1] == OCCUPIED || Matrix[Bottom(), Left] == OCCUPIED)
                return false;
            return true;
        }

        public override bool CanMoveRight()
        {
            if (Left + Width() == WIDTH)
                return false;

            if (Matrix[Head(), Left + Width() - 1] == OCCUPIED
                || Matrix[Head() + 1, Left + Width()] == OCCUPIED
                || Matrix[Bottom(), Left + Width() - 1] == OCCUPIED)
                return false;
            return true;
        }
    }

    public class RightBottomCorner : Shape
    {
        public RightBottomCorner(int top, int left) : base(top, left)
        {
            Piece = new char[3, 3]{  {' ', ' ', '#'},
                                    {' ', ' ', '#'},
                                    {'#', '#', '#'} };
        }
    }

    public class VerticalLine : Shape
    {
        public VerticalLine(int top, int left) : base(top, left)
        {
            Piece = new char[4, 1] { { '#' }, { '#' }, { '#' }, { '#' } };
        }
    }

    public class CubeBlock : Shape
    {
        public CubeBlock(int top, int left) : base(top, left)
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

    public Shape GetPiece(char piece, int currentHeight)
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

        Thread.Sleep(200);

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

        int currentHeight = Matrix.GetLength(0) - 1;

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
        int maxHeight = (amountPieces * 4) - currentHeight - 1;
        Assert.Equal(expected, maxHeight);
        Console.WriteLine("The height is: " + maxHeight);
    }
}