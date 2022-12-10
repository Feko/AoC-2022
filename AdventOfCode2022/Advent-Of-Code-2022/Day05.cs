using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestProject1
{
    public class Day05
    {
        private List<Stack<char>> Stacks = new();

        private int CalculateAmountStacks(string separatorLine) => Convert.ToInt32(separatorLine.Split("  ").Last().Trim());

        private void InitializeStacks(int amount)
        {
            Stacks = new(amount);
            for (int i = 0; i < amount; i++)
                Stacks.Add(new());
        }

        private void PopulateStacks(string line)
        {
            for (int stack = 1; stack <= Stacks.Count; stack++)
            {
                char box = line[(stack * 4) - 3];
                if (box != ' ')
                    Stacks[stack - 1].Push(box);
            }
        }

        private void MoveBoxes(string line)
        {
            var parts = line.Split(' ');
            var (size, from, to) = (Convert.ToInt32(parts[1]), Convert.ToInt32(parts[3]), Convert.ToInt32(parts[5]));

            for (int i = 0; i < size; i++)
            {
                var box = Stacks[from - 1].Pop();
                Stacks[to - 1].Push(box);
            }
        }

        private void BlockMoveBoxes(string line)
        {
            var parts = line.Split(' ');
            var (size, from, to) = (Convert.ToInt32(parts[1]), Convert.ToInt32(parts[3]), Convert.ToInt32(parts[5]));
            List<char> tmp = Enumerable.Range(0, size)
                .Select(x => Stacks[from - 1].Pop()).ToList();

            tmp.Reverse();
            tmp.ForEach(x => Stacks[to - 1].Push(x));
        }

        [Fact]
        public void Day05_Part1()
        {
            var lines = System.IO.File.ReadAllLines("Inputs/day05_sample.txt");
            var separatorIndex = lines.TakeWhile(x => !x.StartsWith(" 1   2   3")).Count();

            InitializeStacks(CalculateAmountStacks(lines[separatorIndex]));

            // Load the stacks
            for (int line = separatorIndex - 1; line >= 0; line--)
                PopulateStacks(lines[line]);

            // Move
            for (int line = separatorIndex + 2; line < lines.Count(); line++)
                MoveBoxes(lines[line]);

            var result = new string(Stacks.Select(stack => stack.Pop()).ToArray());
            Assert.Equal("CMZ", result);
        }

        [Fact]
        public void Day05_Part2()
        {
            var lines = System.IO.File.ReadAllLines("Inputs/day05_sample.txt");
            var separatorIndex = lines.TakeWhile(x => !x.StartsWith(" 1   2   3")).Count();

            InitializeStacks(CalculateAmountStacks(lines[separatorIndex]));

            // Load the stacks
            for (int line = separatorIndex - 1; line >= 0; line--)
                PopulateStacks(lines[line]);

            // Move
            for (int line = separatorIndex + 2; line < lines.Count(); line++)
                BlockMoveBoxes(lines[line]);

            var result = new string(Stacks.Select(stack => stack.Pop()).ToArray());
            Assert.Equal("MCD", result);
        }
    }
}
