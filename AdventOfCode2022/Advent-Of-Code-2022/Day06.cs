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
            (bool hasFound, int current) = (false, 0);

            while (!hasFound)
            {
                hasFound = text[current..(current + 4)].ToCharArray().Distinct().Count() == 4;
                current++;
            }

            Assert.Equal(5, current+3);
        }

        [Fact]
        public void Day06_Part2()
        {
            
        }
    }
}
