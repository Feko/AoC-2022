using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestProject1
{
    public class Day10
    {
        [Fact]
        public void Day10_Part1()
        {
            var lines = System.IO.File.ReadAllLines("Inputs/day10_sample.txt");
            //var lines = System.IO.File.ReadAllLines("Inputs/day10.txt");
            long X = 1;
            long cycle = 0;
            long signalAccumulator = 0;

            void AddCycle()
            {
                cycle++;
                if ((cycle - 20) % 40 == 0)
                {
                    long currentStrength = cycle * X;
                    signalAccumulator += currentStrength;
                }
            }

            foreach (var line in lines)
            {
                var parts = line.Split(' ');
                if (string.Equals("noop", parts[0]))
                    AddCycle();
                else
                {
                    AddCycle();
                    AddCycle();
                    X += Convert.ToInt64(parts[1]);
                }
            }

            Assert.Equal(13140, signalAccumulator);
        }

        [Fact]
        public void Day10_Part2()
        {
            var lines = System.IO.File.ReadAllLines("Inputs/day10_sample.txt");
            //var lines = System.IO.File.ReadAllLines("Inputs/day10.txt");
            int X = 1;
            int cycle = 0;

            StringBuilder sb = new StringBuilder();
            List<string> displayRows = new List<string>(8);

            void AddCycle()
            {
                cycle++;
                int pixel = (cycle % 40);
                string lit = (pixel >= X && pixel <= (X + 2)) ? "#" : ".";
                sb.Append(lit);
                if (cycle % 40 == 0)
                {
                    displayRows.Add(sb.ToString());
                    sb = new StringBuilder();
                }
            }

            foreach (var line in lines)
            {
                var parts = line.Split(' ');
                if (string.Equals("noop", parts[0]))
                    AddCycle();
                else
                {
                    AddCycle();
                    AddCycle();
                    X += Convert.ToInt32(parts[1]);
                }
            }

            foreach (var row in displayRows)
                Console.WriteLine(row);

            Assert.Equal("##..##..##..##..##..##..##..##..##..##..", displayRows.First());
        }
    }
}
