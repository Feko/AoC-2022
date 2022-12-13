using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestProject1
{
    public class Day13
    {
        [Fact]
        public void Day13_Part1()
        {
            //int expected = 13;  var text = File.ReadAllText("Inputs/day13_sample.txt");
            int expected = 5292; var text = File.ReadAllText("Inputs/day13.txt");

            var pairs = text.Split("\n\n").Select(item =>
            {
                var parts = item.Split('\n');
                return (JToken.Parse(parts[0]), JToken.Parse(parts[1]));
            }).ToList();

            int sum = pairs.Select((pair, index) => ComparePair(pair) == -1 ? index + 1 : 0).Sum();

            Assert.Equal(expected, sum);
        }

        [Fact]
        public void Day13_Part2()
        {
            //int expected = 140; var lines = File.ReadAllLines("Inputs/day13_sample.txt")
            int expected = 23868; var lines = System.IO.File.ReadAllLines("Inputs/day13.txt")
                .Where(line => !string.IsNullOrEmpty(line))
                .Select(line => line.Replace("[", "").Replace("]", ""))
                .OrderBy(x => x)
                .ToList();

            (int marker2, int marker6) = (-1, -1);
            for (int i = 1; i <= lines.Count; i++)
            {
                if (marker2 == -1 && (lines[i - 1].StartsWith("3") || lines[i - 1].StartsWith("2")))
                    marker2 = i;

                if (marker6 == -1 && (lines[i - 1].StartsWith("7") || lines[i - 1].StartsWith("6")))
                {
                    marker6 = i + 1;
                    break;
                }

            }

            //Lines startin with 10 breaks string ordering. Lines only go up to 10, don't bother with 11, 12, 27, 44, 73...
            var linesWith10 = lines.Count(line => line.StartsWith("10"));
            marker2 = marker2 - linesWith10;
            marker6 = marker6 - linesWith10;
            Assert.Equal(expected, marker2 * marker6);

        }

        public int ComparePair((JToken left, JToken right) pair)
        {
            if (pair.left is JArray && pair.right is JArray)
            {
                var leftArray = pair.left.ToObject<List<JToken>>();
                var rightArray = pair.right.ToObject<List<JToken>>();

                int iterarions = Math.Min(leftArray.Count, rightArray.Count);
                for (int i = 0; i < iterarions; i++)
                {
                    var result = ComparePair((leftArray[i], rightArray[i]));
                    if (result != 0)
                        return result;
                }

                // run out of elements
                if (rightArray.Count == leftArray.Count)
                    return 0;
                return iterarions == leftArray.Count ? -1 : 1;
            }

            // Both sides are int
            if (pair.left is not JArray && pair.right is not JArray)
                return ((int)pair.left).CompareTo((int)pair.right);

            // Mixed types
            var right = (pair.left is JArray) ? JToken.FromObject(new int[] { (int)pair.right }) : pair.right;
            var left = (pair.right is JArray) ? JToken.FromObject(new int[] { (int)pair.left }) : pair.left;
            return ComparePair((left, right));
        }
    }
}
