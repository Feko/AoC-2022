using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestProject1
{
    public class Day06
    {
        [Fact]
        public void Day06_Part1()
        {
            //var text = File.ReadAllText("Inputs/day06.txt");
            var text = "bvwbjplbgvbhsrlpgdmjqwftvncz";

            Assert.Equal(5, GetMarkerIndex(text, 4));
        }

        [Fact]
        public void Day06_Part2()
        {
            //var text = File.ReadAllText("Inputs/day06.txt");
            var text = "nznrnfrfntjfmvfwmzdfjlvtqnbhcprsg";

            Assert.Equal(29, GetMarkerIndex(text, 14));
        }

        private int GetMarkerIndex(string text, int blockSize)
        {
            (bool hasFound, int current) = (false, 0);

            while (!hasFound)
            {
                hasFound = text[current..(current + blockSize)].ToCharArray().Distinct().Count() == blockSize;
                current++;
            }

            return current + blockSize - 1;
        }
    }
}
