using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Xunit;

namespace TestProject1
{
    public class Day15
    {
        public record Position
        {
            public int X { get; set; }
            public int Y { get; set; }

            public static Position Parse(string data)
            {
                var parts = data.Split(", ");
                return new Position()
                {
                    X = int.Parse(parts[0].Split('=')[1]),
                    Y = int.Parse(parts[1].Split('=')[1]),
                };
            }

        }

        public record SensorRead
        {
            public Position SensorPosition { get; set; }
            public Position BeaconPosition { get; set; }

            public static SensorRead Parse(string data)
            {
                var parts = data.Replace("Sensor at ", "").Replace(": closest beacon is", "").Split(" at ");
                return new SensorRead()
                {
                    SensorPosition = Position.Parse(parts[0]),
                    BeaconPosition = Position.Parse(parts[1]),
                };
            }

            public int GetRadius()
            {
                int distanceX = Math.Abs(SensorPosition.X - BeaconPosition.X);
                int distanceY = Math.Abs(SensorPosition.Y - BeaconPosition.Y);
                return distanceX + distanceY;
            }

            public bool InSignalRange(int row) => SensorPosition.Y + GetRadius() >= row && SensorPosition.Y - GetRadius() <= row;
            
            public bool InSignalRange(int x, int y) => Math.Abs(x - SensorPosition.X) + Math.Abs(y - SensorPosition.Y) <= GetRadius();

            public int GetDistance(int row) => row > SensorPosition.Y ? (SensorPosition.Y + GetRadius() - row) : (row - (SensorPosition.Y - GetRadius()));
        }

        public int ScanPositionsTouchedBySensorRange(List<SensorRead> reads, int TargetY)
        {
            HashSet<Position> positions = new();
            foreach (var read in reads)
            {
                int distance = read.GetDistance(TargetY);
                for (int i = read.SensorPosition.X - distance; i < read.SensorPosition.X + distance; i++)
                    positions.Add(new Position() { X = i, Y = TargetY });
            }

            return positions.Count;
        }

        public int GetAmountShadowArea(List<SensorRead> reads, int target, int max, char direction)
        {
            List<(int start, int end)> sensorRanges = new();
            List<(int start, int end)> influences = new();
            foreach (var read in reads)
            {
                if (read.InSignalRange(target))
                {
                    int distance = read.GetDistance(target);
                    if(direction == 'Y')
                        sensorRanges.Add((Math.Max(0, read.SensorPosition.X - distance), Math.Min(read.SensorPosition.X + distance, max)));
                    else
                        sensorRanges.Add((Math.Max(0, read.SensorPosition.Y - distance), Math.Min(read.SensorPosition.Y + distance, max)));
                }
            }

            int start = 0; int end = 0;
            var sorted = sensorRanges.OrderBy(x => x.start).ToList();
            foreach (var sensorRange in sorted)
            {
                if (sensorRange.start >= start && sensorRange.start <= end)
                {
                    end = Math.Max(end, sensorRange.end);
                }
                else
                {
                    influences.Add((start, end));
                    start = sensorRange.start;
                    end = sensorRange.end;
                }
            }

            return influences.Count;
        }

        [Fact]
        public void Day15_Part1()
        {
            int onLine = 2000000; int expected = 4748135; var lines = File.ReadAllLines("Inputs/day15.txt");
            //int onLine = 10; int expected = 26; var lines = File.ReadAllLines("Inputs/day15_sample.txt");
            var reads = lines.Select(line => SensorRead.Parse(line)).ToList();

            var whereBeaconsNotPresent = ScanPositionsTouchedBySensorRange(reads, onLine);

            Assert.Equal(expected, whereBeaconsNotPresent);
        }

        [Fact]
        public void Day15_Part2()
        {
            // Not exactly fast, it takes around 30 seconds - but that's as far as I'm willing to go with this one.

            int MAX = 4000000; int expectedX = 3435885; int expectedY = 2639657; var lines = File.ReadAllLines("Inputs/day15.txt");
            //int MAX = 20; int expectedX = 14; int expectedY = 11; var lines = File.ReadAllLines("Inputs/day15_sample.txt");

            var reads = lines.Select(line => SensorRead.Parse(line)).ToList();
            int multiplier = 4000000;
            Position untouched = new();

            var possibleY = Enumerable.Range(0, MAX + 1).Where(idx => GetAmountShadowArea(reads, idx, MAX, 'Y') > 0).ToList();
            var possibleX = Enumerable.Range(0, MAX + 1).Where(idx => GetAmountShadowArea(reads, idx, MAX, 'X') > 0).ToList();

            foreach (var y in possibleY)
            {
                foreach (var x in possibleX)
                {
                    if (reads.Any(read => read.InSignalRange(x, y)))
                        continue;

                    untouched = new Position() { X = x, Y = y };
                    break;
                }
            }

            Console.WriteLine($"Untouched X, Y = {untouched.X}, {untouched.Y}");

            var result = BigInteger.Multiply(untouched.X, multiplier);
            result = BigInteger.Add(untouched.Y, result);
            Console.WriteLine(result);

            Assert.Equal(expectedX, untouched.X);
            Assert.Equal(expectedY, untouched.Y);
        }
    }
}
