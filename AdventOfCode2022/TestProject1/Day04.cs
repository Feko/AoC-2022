using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestProject1
{
    public class Day04
    {
        private struct Elf
        {
            public int Start { get; set; }
            public int Finish { get; set; }

            public Elf(string range)
            {
                var parts = range.Split('-');
                (Start, Finish) = (Convert.ToInt32(parts[0]), Convert.ToInt32(parts[1]));
            }

            public bool Overlaps(Elf e)
                => (e.Start >= this.Start && e.Finish <= this.Finish)
                || (e.Start <= this.Start && e.Finish >= this.Finish);

            public bool Intersects(Elf e)
                => Enumerable.Range(Start, Finish - Start + 1).Intersect(Enumerable.Range(e.Start, e.Finish - e.Start + 1)).Any();
        }

        [Fact]
        public void Day04_Part1()
        {
            var input = File.ReadAllLines("Inputs/day04_sample.txt");
            //var input = File.ReadAllLines("Inputs/day04.txt");

            var elfPairs = input.Where(s => !string.IsNullOrEmpty(s))
                .Select(s => { var elfs = s.Split(','); return new { elf1 = new Elf(elfs[0]), elf2 = new Elf(elfs[1]) }; });

            Assert.Equal(2, elfPairs.Count(pair => pair.elf1.Overlaps(pair.elf2)));
        }

        [Fact]
        public void Day04_Part2()
        {
            var input = File.ReadAllLines("Inputs/day04_sample.txt");
            //var input = File.ReadAllLines("Inputs/day04.txt");

            var elfPairs = input.Where(s => !string.IsNullOrEmpty(s))
                .Select(s => { var elfs = s.Split(','); return new { elf1 = new Elf(elfs[0]), elf2 = new Elf(elfs[1]) }; });

            Assert.Equal(4, elfPairs.Count(pair => pair.elf1.Intersects(pair.elf2)));
        }
    }
}
