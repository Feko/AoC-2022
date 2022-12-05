using System.IO;
using System.Linq;
using Xunit;

namespace TestProject1
{
    public class Day03
    {
        private int GetPriority(char c) => c > 96 ? c - 96 : c - 38;

        [Fact]
        public void Day03_Part1()
        {
            var input = File.ReadAllLines("Inputs/day03_sample.txt");
            //var input = File.ReadAllLines("Inputs/day03.txt");

            var rucksack = input.Where(s => !string.IsNullOrEmpty(s))
                .Select(s => { int half = s.Length / 2; return new { firstPocket = s[..half], secondPocket = s[half..] }; })
                .Select(rucksack => rucksack.firstPocket.Intersect(rucksack.secondPocket).First()).ToList();

            Assert.Equal(157, rucksack.Sum(c => GetPriority(c)));

        }

        [Fact]
        public void Day03_Part2()
        {
            var input = File.ReadAllLines("Inputs/day03_sample.txt");
            //var input = File.ReadAllLines("Inputs/day03.txt");

            var groupBadges = input.Where(s => !string.IsNullOrEmpty(s))
                .Chunk(3)
                .Select(chunk => chunk[0].Intersect(chunk[1]).Intersect(chunk[2]).First());

            Assert.Equal(70, groupBadges.Sum(c => GetPriority(c)));
        }
    }
}
