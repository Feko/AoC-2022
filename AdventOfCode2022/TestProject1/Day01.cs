using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestProject1
{
    public class Day01
    {
        [Fact]
        public void Day01_Part1()
        {
            //var input = File.ReadAllText("Inputs/day01_sample.txt");
            var input = File.ReadAllText("Inputs/day01.txt");

            var caloriesPerElf = input.Split("\n\n")
                .Select(x => x.Split("\n").Select(r => string.IsNullOrEmpty(r) ? 0 : Convert.ToInt32(r)).Sum());

            Assert.Equal(24000, caloriesPerElf.Max());
        }

        [Fact]
        public void Day01_Part2()
        {
            //var input = File.ReadAllText("Inputs/day01_sample.txt");
            var input = File.ReadAllText("Inputs/day01.txt");

            var caloriesPerElf = input.Split("\n\n")
                .Select(x => x.Split("\n").Select(r => string.IsNullOrEmpty(r) ? 0 : Convert.ToInt32(r)).Sum()).ToList();

            caloriesPerElf.Sort();
            caloriesPerElf.Reverse();
            var top3 = caloriesPerElf.Take(3);

            Assert.Equal(45000, top3.Sum());
        }
    }
}
