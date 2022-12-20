using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Xunit;

namespace TestProject1
{
    public class Day16
    {
        private KeyValuePair<string, (int, string[])> ParseValve(string valve)
        {
            var parts = valve.Split(" to valve");
            var leadTo = parts[1].Replace("s ","").Split(", ");
            var columns = parts[0].Split(' ');
            var valveName = columns[1];
            var rate = int.Parse(columns[4].Replace(";","").Split('=').Last());
            return new KeyValuePair<string, (int, string[])>(valveName, (rate, leadTo));
        }

        [Fact]
        public void Day16_Part1()
        {
            var valves = File.ReadAllLines("Inputs/day16_sample.txt").Select(ParseValve).ToDictionary(k => k.Key, v => v.Value);
        }
    }
}
