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
                    _ => throw new NotImplementedException()
                };
            }
        }

        public class MonkeyOperation
        {
            public string Name { get; private set; }
            BigInteger? Value;
            string[] OtherMonkeys = new string[0];
            public string Operation { get; set; }
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
                if(Value.HasValue)
                    return Value.Value;

                Value = BigIntExpressionEvaluator.Exec(Operation, Monkey1.GetValue(), Monkey2.GetValue());
                return Value.Value;
            }

            public void AddReferences(Dictionary<string, MonkeyOperation> allMonkeys)
            {
                if (OtherMonkeys.Length == 0)
                    return;

                Monkey1 = allMonkeys[OtherMonkeys[0]];
                Monkey2 = allMonkeys[OtherMonkeys[1]];
            }
        }

        [Fact]
        public void Day06_Part1()
        {
            //long expected = 152; var lines = File.ReadAllLines("Inputs/day21_sample.txt");
            long expected = 152; var lines = File.ReadAllLines("Inputs/day21.txt");

            var allMonkeys = lines.Select(line => new MonkeyOperation(line)).ToDictionary(k => k.Name, v => v);

            foreach(var (k, v) in allMonkeys)
                v.AddReferences(allMonkeys);

            Assert.Equal(expected, allMonkeys["root"].GetValue());
        }
    }
}
