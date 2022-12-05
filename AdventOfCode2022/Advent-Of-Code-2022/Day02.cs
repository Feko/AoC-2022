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

        private int GetHandScore(Hand hand) => (int)hand + 1;

        private bool Draw(Hand opponent, Hand myHand) => opponent == myHand;

        private bool Win(Hand opponent, Hand myHand) => _possibilityMatrix[opponent].loses == myHand;

        private int RoundScore(Hand opponent, Hand myHand)
        {
            int score = Win(opponent, myHand) ? 6 : (Draw(opponent, myHand) ? 3 : 0);
            return score + GetHandScore(myHand);
        }

        private Hand GetHand(Hand opponent, string strategy)
            => strategy switch
            {
                "Y" => opponent,
                "X" => _possibilityMatrix[opponent].wins,
                "Z" => _possibilityMatrix[opponent].loses,
                _ => throw new NotImplementedException()
            };


        [Fact]
        public void Day02_Part1()
        {
            var input = File.ReadAllLines("Inputs/day02_sample.txt");
            //var input = File.ReadAllLines("Inputs/day02.txt");

            var games = input.Where(s => !string.IsNullOrEmpty(s))
                .Select(s => { var item = s.Split(' '); return new { opponent = _hands[item[0]], myHand = _hands[item[1]] }; });

            var myScore = games.Select(x => RoundScore(x.opponent, x.myHand)).Sum();

            Assert.Equal(15, myScore);
        }

        [Fact]
        public void Day02_Part2()
        {
            var input = File.ReadAllLines("Inputs/day02_sample.txt");
            //var input = File.ReadAllLines("Inputs/day02.txt");

            var games = input.Where(s => !string.IsNullOrEmpty(s))
                .Select(s => { var item = s.Split(' '); return new { opponent = _hands[item[0]], myHand = GetHand(_hands[item[0]], item[1]) }; });

            var myScore = games.Select(x => RoundScore(x.opponent, x.myHand)).Sum();

            Assert.Equal(12, myScore);
        }
    }
}
