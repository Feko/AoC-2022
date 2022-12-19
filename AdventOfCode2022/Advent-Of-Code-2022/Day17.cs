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

    public long PerformMovement(int amountPieces, string movements, Func<int, int, int> indexModifierCallback = null)
    {
        var pieces = "-+L|#";
        var infiniteDirections = GetInfiniteSeries(movements.ToArray()).GetEnumerator();
        var infinitePieces = GetInfiniteSeries(pieces.ToArray()).GetEnumerator();

        int maxHeight = amountPieces * 4;
        Matrix = new char[maxHeight, WIDTH];
        for (int line = 0; line < amountPieces * 4; line++)
            for (int column = 0; column < WIDTH; column++)
                Matrix[line, column] = EMPTY;

        int currentHeight = maxHeight - 1;

        for (int i = 0; i < amountPieces; i++)
        {
            var realHeight = maxHeight - currentHeight - 1;
            if (indexModifierCallback != null)
                i = indexModifierCallback(i, realHeight);

            infinitePieces.MoveNext();
            var piece = GetPiece(infinitePieces.Current, currentHeight);
            infiniteDirections.MoveNext();
            piece.Move(infiniteDirections.Current);
            do
            {
                if (piece.CanGoDown())
                {
                    piece.GoDown();
                }
                infiniteDirections.MoveNext();
                piece.Move(infiniteDirections.Current);
            }
            while (piece.CanGoDown());

            piece.Save();
            currentHeight = Math.Min(currentHeight, piece.Head() - 1);
        }

        return (amountPieces * 4) - currentHeight - 1;
    }

    [Fact]
    public void Day17_Part1()
    {
        int expected = 3071; var text = System.IO.File.ReadAllText("Inputs/day17.txt");
        //int expected = 3068; var text = ">>><<><>><<<>><>>><<<>>><<<><<<>><>><<>>";
        int amountPieces = 2022;

        long maxHeight = PerformMovement(amountPieces, text);
        Assert.Equal(expected, maxHeight);
    }

    [Fact]
    public void Day17_Part2()
    {
        long expected = 1523615160362; var text = System.IO.File.ReadAllText("Inputs/day17.txt");
        long hugeAssNumber = 1_000_000_000_000;
        //int expected = 3068; var text = ">>><<><>><<<>><>>><<<>>><<<><<<>><>><<>>";

        // OK, iterating 1 trillion times proved inefficient (Who knew!!!!!) 
        // I'm running the pieces a few thousand times to create a dataset, and then look for a repeating pattern.
        int iterations = 100_000;
        _ = PerformMovement(iterations, text);

        //Now that we have some dataset, look for a pattern
        (int patternStartLine, int patternFinishLine) = FindPattern();


        //Start the falling pieces again. This time, record the height and pieces before the recurring pattern,
        //  also the pieces in a pattern, and the height of the last run, if any.
        var patternSize = patternFinishLine - patternStartLine;
        int piecesBeforePattern = 0;
        int heightBeforePattern = 0;
        int piecesWhenPatternStarted = 0;
        int patternSizeInPieces = 0;
        int remainingPieces = 0;
        int heightLastPatternRunStart = 0;
        int heightLastPatternRunFinish = 0;

        _ = PerformMovement(iterations, text, (piece, height) => 
        {
            if (height < patternStartLine)
            {
                piecesBeforePattern = piece;
                heightBeforePattern = height;
            }
            if (height == patternStartLine)
            {
                piecesWhenPatternStarted = piece;
            }
            if (height == patternFinishLine)
            {
                patternSizeInPieces = piece - piecesWhenPatternStarted;
                //break;
                //OK, so now that we know the pattern size, and how many pieces we moved ... we need to find how many pieces we need to move in the last pattern loop.
                remainingPieces = Convert.ToInt32((hugeAssNumber - piecesBeforePattern) % patternSizeInPieces);
                heightLastPatternRunStart = height;
                heightLastPatternRunFinish = height;
                return iterations - remainingPieces - 1;
               
            }
            if (heightLastPatternRunStart > 0)
            {
                heightLastPatternRunFinish = height;
            }
            return piece;
        });

        long repeatedPatternHeight = ((hugeAssNumber - piecesBeforePattern) / patternSizeInPieces) * patternSize;
        long answer = heightBeforePattern + repeatedPatternHeight + (heightLastPatternRunFinish - heightLastPatternRunStart);

        Assert.Equal(expected, answer);
    }

    public (int patternStartLine, int patternFinishLine) FindPattern()
    {
        int patternThreshold = 500; // It's not a real pattern unless we have some big amount of consecutive rows
        for (int line = 1; line < Matrix.GetLength(0); line++)
        {
            for (int previousLine = 0; previousLine < line; previousLine++)
            {
                if (LineEquals(line, previousLine))
                {
                    int amount = 1;
                    while (LineEquals(line + amount, previousLine + amount) && amount <= patternThreshold)
                    {
                        amount++;
                    }
                    if (amount > patternThreshold)
                    {
                        return (previousLine + 1, line + 1);
                    }
                }
            }
        }
        throw new Exception("Pattern not found.");
    }

    public bool LineEquals(long line1, long line2)
    {
        line1 = Matrix.GetLength(0) - 1 - line1;
        line2 = Matrix.GetLength(0) - 1 - line2;
        for (int column = 0; column < WIDTH; column++)
            if (Matrix[line1, column] != Matrix[line2, column])
                return false;

        return true;
    }


}