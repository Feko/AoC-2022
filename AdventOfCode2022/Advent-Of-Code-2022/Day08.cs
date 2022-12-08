using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestProject1
{
    public class Day08
    {
        public int ToInt(char c) => c - '0';

        public bool IsVisible(string[] input, int line, int column)
        {
            int treeSize = ToInt(input[line][column]);

            if (input[0..line].Select(c => ToInt(c[column])).All(t => t < treeSize))
                return true;

            if (input[(line + 1)..].Select(c => ToInt(c[column])).All(t => t < treeSize))
                return true;

            if (input[line][0..column].Select(l => ToInt(l)).All(t => t < treeSize))
                return true;

            if (input[line][(column + 1)..].Select(l => ToInt(l)).All(t => t < treeSize))
                return true;

            return false;
        }

        [Fact]
        public void Day08_Part01()
        {
            //var input = File.ReadAllLines("Inputs/day08.txt");
            var input = File.ReadAllLines("Inputs/day08_sample.txt");

            int totalVisibles = input.Length * 2 + (input[0].Length * 2) - 4;

            for (int line = 1; line < input.Length - 1; line++)
            {
                for (int column = 1; column < input[line].Length - 1; column++)
                {
                    if (IsVisible(input, line, column))
                    {
                        totalVisibles++;
                    }
                }
            }

            Assert.Equal(21, totalVisibles);
        }

        private int GetScenicScore(string[] input, int line, int column)
        {
            int treeSize = ToInt(input[line][column]);
            var upside = input[0..line].Select(c => ToInt(c[column])).ToList();
            var downside = input[(line + 1)..].Select(c => ToInt(c[column])).ToList();
            var left = input[line][0..column].Select(l => ToInt(l)).ToList();
            var right = input[line][(column + 1)..].Select(l => ToInt(l)).ToList();

            var upsideScore = upside.Reverse<int>().TakeWhile(t => t < treeSize).Count();
            var downsideScore = downside.TakeWhile(t => t < treeSize).Count();
            var leftScore = left.Reverse<int>().TakeWhile(t => t < treeSize).Count();
            var rightScore = right.TakeWhile(t => t < treeSize).Count();

            return 
                Math.Min(upsideScore + 1, upside.Count) * 
                Math.Min(downsideScore + 1, downside.Count) * 
                Math.Min(leftScore + 1, left.Count) * 
                Math.Min(rightScore + 1, right.Count);
        }

        [Fact]
        public void Day08_Part02()
        {
            //var input = File.ReadAllLines("Inputs/day08.txt");
            var input = File.ReadAllLines("Inputs/day08_sample.txt");
            int sweetSpot = Int32.MinValue;

            for (int line = 1; line < input.Length - 1; line++)
                for (int column = 1; column < input[line].Length - 1; column++)
                    sweetSpot = Math.Max(sweetSpot, GetScenicScore(input, line, column));

            Assert.Equal(8, sweetSpot);
        }
    }
}
