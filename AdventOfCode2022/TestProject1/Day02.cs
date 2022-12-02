using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestProject1
{
    public enum Hand
    {
        Rock,
        Paper,
        Scissors
    }

    public class Day02
    {
        private Dictionary<string, Hand> _hands = new Dictionary<string, Hand>()
        {
            { "A", Hand.Rock },
            { "B", Hand.Paper },
            { "C", Hand.Scissors },
            { "X", Hand.Rock },
            { "Y", Hand.Paper },
            { "Z", Hand.Scissors },
        };       

        private Dictionary<Hand, (Hand wins, Hand loses)> _possibilityMatrix = new Dictionary<Hand, (Hand wins, Hand losese)>()
        {
            { Hand.Paper, (Hand.Rock, Hand.Scissors) },
            { Hand.Rock, (Hand.Scissors, Hand.Paper) },
            { Hand.Scissors, (Hand.Paper, Hand.Rock) }
        };

        private Dictionary<Hand, int> _handScores = new Dictionary<Hand, int>()
        {
            { Hand.Rock, 1 },
            { Hand.Paper, 2 },
            { Hand.Scissors, 3 },
        };

        private bool Draw(Hand opponent, Hand myHand) => opponent == myHand;

        private bool Win(Hand opponent, Hand myHand) =>
            (myHand == Hand.Rock && opponent == Hand.Scissors) ||
            (myHand == Hand.Paper && opponent == Hand.Rock) ||
            (myHand == Hand.Scissors && opponent == Hand.Paper);

        private int RoundScore(Hand opponent, Hand myHand)
        {
            int score = Win(opponent, myHand) ? 6 : (Draw(opponent, myHand) ? 3 : 0);
            return score + _handScores[myHand];
        }

        private Hand GetHand(Hand opponent, string strategy)
        {
            if (strategy == "Y") // draw
                return opponent;
            
            if (strategy == "X") // I should lose, returns what wins
                return _possibilityMatrix[opponent].wins;

            return _possibilityMatrix[opponent].loses;
        }


        [Fact]
        public void Day02_Part1()
        {
            var input = File.ReadAllLines("Inputs/day02_sample.txt");
            //var input = File.ReadAllLines("Inputs/day02.txt");

            var games = input.Where(s => !string.IsNullOrEmpty(s))
                .Select(s => { var item = s.Split(' '); return new KeyValuePair<Hand, Hand>(_hands[item[0]], _hands[item[1]]); });

            var myScore = games.Select(x => RoundScore(x.Key, x.Value)).Sum();

            Assert.Equal(15, myScore);
        }

        [Fact]
        public void Day02_Part2()
        {
            var input = File.ReadAllLines("Inputs/day02_sample.txt");
            //var input = File.ReadAllLines("Inputs/day02.txt");

            var games = input.Where(s => !string.IsNullOrEmpty(s))
                .Select(s => { var item = s.Split(' '); return new KeyValuePair<Hand, Hand>(_hands[item[0]], GetHand(_hands[item[0]], item[1])); });

            var myScore = games.Select(x => RoundScore(x.Key, x.Value)).Sum();

            Assert.Equal(12, myScore);
        }
    }
}
