using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestProject1
{
    public class Day21
    {
        public class BigIntExpressionEvaluator
        {
            public static BigInteger Exec(string exp, BigInteger num1, BigInteger num2)
            {
                return exp switch
                {
                    "*" => BigInteger.Multiply(num1, num2),
                    "/" => BigInteger.Divide(num1, num2),
                    "-" => BigInteger.Subtract(num1, num2),
                    "+" => BigInteger.Add(num1, num2),
                    "=" => num1,
                    _ => throw new NotImplementedException()
                };
            }
        }

        public class MonkeyOperation
        {
            public string Name { get; private set; }
            public string Operation { get; set; }
            public MonkeyOperation Parent { get; set; }

            BigInteger? Value;
            string[] OtherMonkeys = new string[0];
            MonkeyOperation Monkey1;
            MonkeyOperation Monkey2;

            public MonkeyOperation(string line)
            {
                var parts = line.Split(": ");
                Name = parts[0];

                if (parts[1].Length < 8)
                    Value = long.Parse(parts[1]);

                else
                {
                    var exp = parts[1].Split(' ');
                    Operation = exp[1];
                    OtherMonkeys = new[] { exp[0], exp[2] };
                }
            }

            public BigInteger GetValue()
            {
                if (Value.HasValue)
                    return Value.Value;

                return BigIntExpressionEvaluator.Exec(Operation, Monkey1.GetValue(), Monkey2.GetValue());
            }

            public void AddReferences(Dictionary<string, MonkeyOperation> allMonkeys)
            {
                if (OtherMonkeys.Length == 0)
                    return;

                Monkey1 = allMonkeys[OtherMonkeys[0]];
                Monkey2 = allMonkeys[OtherMonkeys[1]];
                Monkey1.Parent = this;
                Monkey2.Parent = this;
            }

            public void Equalize(List<string> sequence)
            {
                //This method is only called at the root node
                var valueFrom = sequence.First() == OtherMonkeys.First() ? Monkey2 : Monkey1;
                Operation = "=";
                Equalize(sequence, valueFrom.GetValue());
            }

            public void Equalize(List<string> sequence, BigInteger expectedValue)
            {
                //Is this the final node?
                if (!sequence.Any())
                {
                    Value = expectedValue;
                    return;
                }

                (var nodeToNormalize, var valueFromNode, bool normalizingRight) = (Monkey2, Monkey1, true);
                if (sequence.First() == OtherMonkeys.First())
                    (nodeToNormalize, valueFromNode, normalizingRight) = (Monkey1, Monkey2, false);

                var otherHandValue = valueFromNode.GetValue();
                var valueNeeded = GetValueNeeded(expectedValue, normalizingRight, otherHandValue);

                sequence.RemoveAt(0);
                nodeToNormalize.Equalize(sequence, valueNeeded);
            }

            private BigInteger GetValueNeeded(BigInteger expectedValue, bool normalizingRight, BigInteger otherHandValue)
            {
                return Operation switch
                {
                    "+" => expectedValue - otherHandValue,
                    "-" => normalizingRight ? otherHandValue - expectedValue : expectedValue + otherHandValue,
                    "*" => expectedValue / otherHandValue,
                    "/" => normalizingRight ? otherHandValue / expectedValue : expectedValue * otherHandValue,
                    "=" => expectedValue,
                    _ => throw new Exception("This... is not supposed to happen")
                };
            }
        }

        [Fact]
        public void Day21_Part1()
        {
            //long expected = 152; var lines = File.ReadAllLines("Inputs/day21_sample.txt");
            long expected = 169525884255464; var lines = File.ReadAllLines("Inputs/day21.txt");

            var allMonkeys = lines.Select(line => new MonkeyOperation(line)).ToDictionary(k => k.Name, v => v);

            foreach(var (k, v) in allMonkeys)
                v.AddReferences(allMonkeys);

            Assert.Equal(expected, allMonkeys["root"].GetValue());
        }

        [Fact]
        public void Day21_Part2()
        {
            //long expected = 301; var lines = File.ReadAllLines("Inputs/day21_sample.txt");
            long expected = 3247317268284; var lines = File.ReadAllLines("Inputs/day21.txt");

            var allMonkeys = lines.Select(line => new MonkeyOperation(line)).ToDictionary(k => k.Name, v => v);

            foreach (var (k, v) in allMonkeys)
                v.AddReferences(allMonkeys);

            var theMonkeyIAm = allMonkeys["humn"];
            var normalizationSequence = GetNormalizationSequence(theMonkeyIAm);

            allMonkeys["root"].Equalize(normalizationSequence);

            Assert.Equal(expected, theMonkeyIAm.GetValue());
        }

        private static List<string> GetNormalizationSequence(MonkeyOperation theMonkeyIAm)
        {
            List<string> normalizationSequence = new();

            MonkeyOperation current = theMonkeyIAm;
            while (current.Parent != null)
            {
                normalizationSequence.Add(current.Name);
                current = current.Parent;
            }
            normalizationSequence.Reverse();
            return normalizationSequence;
        }
    }
}
