using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
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
                int distanceX = Math.Abs(SensorPosition.X - BeaconPosition.X);// SensorPosition.X > BeaconPosition.X ? SensorPosition.X - BeaconPosition.X : BeaconPosition.X - SensorPosition.X;
                int distanceY = Math.Abs(SensorPosition.Y - BeaconPosition.Y);// SensorPosition.Y > BeaconPosition.Y ? SensorPosition.Y - BeaconPosition.Y : BeaconPosition.Y - SensorPosition.Y;
                return distanceX + distanceY;
            }

            public bool InSignalRange(int row) => SensorPosition.Y + GetRadius() >= row && SensorPosition.Y - GetRadius() <= row;
            
            public bool InSignalRange(int x, int y) => Math.Abs(x - SensorPosition.X) + Math.Abs(y - SensorPosition.Y) <= GetRadius();

            public int GetDistance(int row) => row > SensorPosition.Y ? (SensorPosition.Y + GetRadius() - row) : (row - (SensorPosition.Y - GetRadius()));

            public int GetMaxSensorRangeX() => SensorPosition.X + GetRadius();
            public int GetMaxSensorRangeY() => SensorPosition.Y + GetRadius();
            public int GetMinSensorRangeX() => SensorPosition.X - GetRadius();
            public int GetMinSensorRangeY() => SensorPosition.Y - GetRadius();
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

        public int Scan(List<SensorRead> reads, int TargetY)
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

        public int ScanPossibleY(List<SensorRead> reads, int TargetY, int max)
        {
            List<(int start, int end)> sensorRanges = new();
            List<(int start, int end)> influences = new();
            foreach (var read in reads)
            {
                if (read.InSignalRange(TargetY))
                {
                    int distance = read.GetDistance(TargetY);
                    sensorRanges.Add((Math.Max(0, read.SensorPosition.X - distance), Math.Min(read.SensorPosition.X + distance, max)));
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
            influences.Add((start, end));
            //HashSet<Position> positions = new(max);
            //foreach (var read in reads)
            //{
            //    if (read.InSignalRange(TargetY))
            //    {
            //        int distance = read.GetDistance(TargetY);
            //        int radius = read.GetRadius();
            //        for (int i = Math.Max(0, read.SensorPosition.X - distance); i < Math.Min(read.SensorPosition.X + distance, max) ; i++)
            //            positions.Add(new Position() { X = i, Y = TargetY });
            //    }
            //}

            return influences.Count;
        }

        public int ScanPossibleX(List<SensorRead> reads, int TargetX, int max)
        {
            List<(int start, int end)> sensorRanges = new();
            List<(int start, int end)> influences = new();
            foreach (var read in reads)
            {
                if (read.InSignalRange(TargetX))
                {
                    int distance = read.GetDistance(TargetX);
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
            influences.Add((start, end));
            //HashSet<Position> positions = new(max);
            //foreach (var read in reads)
            //{
            //    if (read.InSignalRange(TargetY))
            //    {
            //        int distance = read.GetDistance(TargetY);
            //        int radius = read.GetRadius();
            //        for (int i = Math.Max(0, read.SensorPosition.X - distance); i < Math.Min(read.SensorPosition.X + distance, max) ; i++)
            //            positions.Add(new Position() { X = i, Y = TargetY });
            //    }
            //}

            return influences.Count;
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
            int onLine = 2000000; int expected = 4748135; var lines = File.ReadAllLines("Inputs/day15.txt");
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
            var whereBeaconsNotPresent = Scan(reads, onLine);

            Assert.Equal(expected, whereBeaconsNotPresent);
        }

        private List<SensorRead> reads;
        Position untouched = new();

        private void ReportThread(int threadId, int current)
        {
            string msg = $"Thread {threadId} reporting at item {current} - {DateTime.Now}";
                Console.SetCursorPosition(0,threadId);
                Console.WriteLine(msg);
        }

        private void DoWork(int threadId)
        {
            int me = threadId;
            int MAX = 4000000;
            int slice = MAX / 10;
            int myTarget = me * slice;

            for (int y = myTarget - slice; y < myTarget; y++)
            {
                if (y % 25 == 0)
                {
                    ReportThread(me, y);
                    if (untouched.X > 0)
                        break;
                }
                for (int x = 0; x < MAX; x++)
                {
                    if (reads.Any(read => read.InSignalRange(x, y)))
                        continue;

                    untouched = new Position() { X = x, Y = y };
                    break;
                }
            }

        }


        [Fact]
        public void Day15_Part2()
        {
            int MAX = 4000000; int onLine = 2000000; int expected = 26; var lines = File.ReadAllLines("Inputs/day15.txt");
            //int MAX = 20; int onLine = 10; int expected = 56000011; var lines = File.ReadAllLines("Inputs/day15_sample.txt");
            reads = lines.Select(line => SensorRead.Parse(line)).ToList();

            InitializeMatrix(reads, onLine);
            //Scan(reads);
            //int whereBeaconsNotPresent = Scan(new[] { reads[6] }.ToList());
            //AddBeaconsSensors(reads);

            //int expectedY = 10;
            //var normalized = NormalizePosition(0,y)

            //var whereBeaconsNotPresent = Enumerable.Range(0, Matrix.GetLength(1))
            //    .Select(idx => Matrix[onLine, idx]).Count(x => x == RANGE);

            //var whereBeaconsNotPresent = Matrix.Count(x => x == RANGE);

            int multiplier = 4000000;
            //Position untouched = new();

            //for (int y = 0; y < MAX; y++)
            //{
            //    for (int x = 0; x < MAX; x++)
            //    {
            //        if (reads.Any(read => read.InSignalRange(x, y)))
            //            continue;

            //        untouched = new Position() { X = x, Y =y };
            //        break;
            //    }
            //}


            //int skipFirst = 3000;
            //var threads = Enumerable.Range(0, 10).Select(i => 
            //{ 
            //    var handle = new EventWaitHandle(false, EventResetMode.ManualReset);
            //    var t = new Thread(() => { DoWork(i + 1); handle.Set(); }) ; 
            //    t.Start();
            //    return handle; 
            //}).ToArray();
            ////foreach(Thread t in threads)
            ////    t.Join();
            //WaitHandle.WaitAll(threads);

            //int counter = 0;
            //foreach (var read in reads)
            //{ 
            //    Console.WriteLine($"Checking sensor {++counter}");
            //    int range = 4000;
            //    int[] attempts = new[] { read.GetMaxSensorRangeX(), read.GetMaxSensorRangeY(), read.GetMinSensorRangeX(), read.GetMinSensorRangeY() };
            //    foreach (int attempt in attempts)
            //    {
            //        for (int y = attempt - range; y < attempt + range; y++)
            //        {
            //            for (int x = attempt - range; x < attempt + range; x++)
            //            {
            //                if (x < 0 || y < 0 || x > MAX || y > MAX)
            //                    continue;

            //                if (reads.Any(read => read.InSignalRange(x, y)))
            //                    continue;

            //                untouched = new Position() { X = x, Y = y };
            //                break;
            //            }
            //        }
            //    }

            //}




            var possibleY = Enumerable.Range(0, MAX+1).Where(idx => ScanPossibleY(reads, idx, MAX) > 1).ToList();
            var possibleX = Enumerable.Range(0, MAX + 1).Where(idx => ScanPossibleX(reads, idx, MAX) > 1).ToList();

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

            //for (int i = 0; i < MAX; i++)
            //{
            //    var result = ScanPossibleY(reads, i, MAX);
            //    if(i % 175 == 0)
            //        Console.WriteLine($"On Y = {i} we have {result}");
            //}

            //for (int i = 0; i < MAX; i++)
            //{
            //    var result = ScanPossibleX(reads, i, MAX);
            //    Console.WriteLine($"On X = {i} we have {result}");
            //}



            Console.WriteLine("Found!!!!!");
            Console.WriteLine($"Untouched X, Y = {untouched.X}, {untouched.Y}");

            var result = BigInteger.Multiply(untouched.X, multiplier);
            result = BigInteger.Add(untouched.Y, result);
            Console.WriteLine(result);

            //Assert.Equal(expected, (untouched.X * multiplier) + untouched.Y);
        }
    }
}
