﻿// See https://aka.ms/new-console-template for more information

var test = new TestProject1.Day14();
test.Day14_Part2();

void ShowMatrix() //Impossible to visualize Part2, it's over 1000 columns
{
    Console.Write("  ");
    for (int column = 0; column < test.Matrix.GetLength(1); column++)
        Console.Write(column % 10);

    int rows = 0;
    Console.Write($"\n{rows} ");
    for (int y = 0; y < test.Matrix.GetLength(0); y++)
    {
        rows = rows == 9 ? 0 : rows + 1;
        for (int x = 0; x < test.Matrix.GetLength(1); x++)
            Console.Write(test.Matrix[y, x]);
        Console.Write($"\n{rows} ");
    }
}