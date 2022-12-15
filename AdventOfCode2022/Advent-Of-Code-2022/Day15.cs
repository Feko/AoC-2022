using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestProject1
{
    public class Day15
    {
        private const char RANGE = '#';
        private const char BEACON = 'B';
        private const char SENSOR = 'S';
        private const char EMPTY = '.';

        public char[] Matrix { get; set; }
        //public char[,] Matrix { get; set; }
        public int MinX { get; set; }
        public int MinY { get; set; }
        public int TargetY { get; set; }

        public record Position
        {
            public int X { get; set; }
            public int Y { get; set; }

            public static Position Parse(string data)
            {
                //x=15, y=3
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
                //Sensor at x=14, y=3: closest beacon is at x=15, y=3
                var parts = data.Replace("Sensor at ", "").Replace(": closest beacon is", "").Split(" at ");
                return new SensorRead()
                {
                    SensorPosition = Position.Parse(parts[0]),
                    BeaconPosition = Position.Parse(parts[1]),
                };
            }

            public int MinX => Math.Min(SensorPosition.X, BeaconPosition.X);
            public int MaxX => Math.Max(SensorPosition.X, BeaconPosition.X);
            public int MinY => Math.Min(SensorPosition.Y, BeaconPosition.Y);
            public int MaxY => Math.Max(SensorPosition.Y, BeaconPosition.Y);

            public int GetRadius()
            {
                int distanceX = SensorPosition.X > BeaconPosition.X ? SensorPosition.X - BeaconPosition.X : BeaconPosition.X - SensorPosition.X;
                int distanceY = SensorPosition.Y > BeaconPosition.Y ? SensorPosition.Y - BeaconPosition.Y : BeaconPosition.Y - SensorPosition.Y;
                return distanceX + distanceY;
            }

            public bool InSignalRange(int row) => SensorPosition.Y + GetRadius() >= row && SensorPosition.Y - GetRadius() <= row;

            public int GetDistance(int row) => row > SensorPosition.Y ? (SensorPosition.Y + GetRadius() - row) : (row - (SensorPosition.Y - GetRadius()));
        }

        public void InitializeMatrix(List<SensorRead> reads, int targetRow) 
        {
            (int minX, int maxX) = (reads.Select(x => x.MinX).Min(), reads.Select(x => x.MaxX).Max());
            (int minY, int maxY) = (reads.Select(x => x.MinY).Min(), reads.Select(x => x.MaxY).Max());

            (int sizeX, int sizeY) = (maxX - minX, maxY - minY);

            //Matrix = new char[sizeY, sizeX];
            Matrix = new char[sizeX];
            TargetY = targetRow;

            MinX = minX;
            MinY = minY;
        }

        public (int x, int y) NormalizePosition(int x, int y) => (x - MinX, y - MinY);
        public bool IsOutbounds(int x, int y) => y != TargetY || x < 0 || x > Matrix.Length - 1 ;

        public void AddItem(char item, int x, int y)
        {
            var pos = NormalizePosition(x, y);
            if(!IsOutbounds(pos.x, pos.y))
                Matrix[pos.x] = item;
        }

        public bool ScanHitBeacon(Position position, int x, int y) => position.X == x && position.Y == y;
        public bool ScanHitBeacon(SensorRead read, int currentDistance, int row)
        {
            return ScanHitBeacon(read.BeaconPosition, read.SensorPosition.X + ((currentDistance - 1 - row) * -1), read.SensorPosition.Y - row)
                || ScanHitBeacon(read.BeaconPosition, read.SensorPosition.X + ((currentDistance - 1 - row) * -1), read.SensorPosition.Y + row)
                || ScanHitBeacon(read.BeaconPosition, read.SensorPosition.X + (currentDistance - 1 - row), read.SensorPosition.Y - row)
                || ScanHitBeacon(read.BeaconPosition, read.SensorPosition.X + (currentDistance - 1 - row), read.SensorPosition.Y + row);
        }

        public int Scan(List<SensorRead> reads)
        {
            HashSet<Position> positions = new();
            foreach (var read in reads)
            {
                if (read.InSignalRange(TargetY))
                {
                    int distance = read.GetDistance(TargetY);
                    int radius = read.GetRadius();
                    for (int i = read.SensorPosition.X - distance; i < read.SensorPosition.X + distance; i++)
                        positions.Add(new Position() { X = i, Y = TargetY });
                }
                //int currentDistance = 0;
                //bool hitBeacon = false;
                //while (!hitBeacon)
                //{
                //    currentDistance++;
                //    if (currentDistance % 200 == 0)
                //        Console.Write($"\r{currentDistance}");
                //    for (int row = 0; row < currentDistance; row++)
                //    {
                //        AddItem(RANGE, read.SensorPosition.X + ((currentDistance - 1 - row) * -1), read.SensorPosition.Y - row);
                //        AddItem(RANGE, read.SensorPosition.X + ((currentDistance - 1 - row) * -1), read.SensorPosition.Y + row);
                //        AddItem(RANGE, read.SensorPosition.X + (currentDistance - 1 - row), read.SensorPosition.Y + row);
                //        AddItem(RANGE, read.SensorPosition.X + (currentDistance - 1 - row), read.SensorPosition.Y - row);
                //        if (ScanHitBeacon(read, currentDistance, row))
                //            hitBeacon = true;
                //    }
                //}
            }

            return positions.Count;
        }

        public void AddBeaconsSensors(List<SensorRead> reads)
        {
            foreach (var read in reads)
            {
                AddItem(BEACON, read.BeaconPosition.X, read.BeaconPosition.Y);
                AddItem(SENSOR, read.SensorPosition.X, read.SensorPosition.Y);
            }
        }


        [Fact]
        public void Day15_Part1()
        {
            int onLine = 2000000; int expected = 26; var lines = File.ReadAllLines("Inputs/day15.txt");
            //int onLine = 10; int expected = 26; var lines = File.ReadAllLines("Inputs/day15_sample.txt");
            var reads = lines.Select(line => SensorRead.Parse(line)).ToList();

            InitializeMatrix(reads, onLine);
            //Scan(reads);
            //int whereBeaconsNotPresent = Scan(new[] { reads[6] }.ToList());
            //AddBeaconsSensors(reads);

            //int expectedY = 10;
            //var normalized = NormalizePosition(0,y)

            //var whereBeaconsNotPresent = Enumerable.Range(0, Matrix.GetLength(1))
            //    .Select(idx => Matrix[onLine, idx]).Count(x => x == RANGE);

            //var whereBeaconsNotPresent = Matrix.Count(x => x == RANGE);
            var whereBeaconsNotPresent = Scan(reads);

            Assert.Equal(expected, whereBeaconsNotPresent);
            
        }
    }
}
