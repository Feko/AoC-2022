using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Xunit;

namespace TestProject1
{
    public class Day11
    {
        public class BigIntExpressionEvaluator
        {
            public static BigInteger Parse(string exp)
            {
                var parts = exp.Split(' ');
                return parts[1] switch
                {
                    "*" => BigInteger.Multiply(BigInteger.Parse(parts[0]), BigInteger.Parse(parts[2])),
                    "/" => BigInteger.Divide(BigInteger.Parse(parts[0]), BigInteger.Parse(parts[2])),
                    "-" => BigInteger.Subtract(BigInteger.Parse(parts[0]), BigInteger.Parse(parts[2])),
                    "+" => BigInteger.Add(BigInteger.Parse(parts[0]), BigInteger.Parse(parts[2])),
                    _ => throw new NotImplementedException()
                };
            }
        }

        public class ThrowOperation
        {
            private readonly IList<Monkey> _collectionPointer;
            private readonly int _trueTarget;
            private readonly int _divisible;
            private readonly int _falseTarget;

            public ThrowOperation(IList<Monkey> collectionPointer, int divisible, int trueTarget, int falseTarget)
            {
                _collectionPointer = collectionPointer;
                _divisible = divisible;
                _trueTarget = trueTarget;
                _falseTarget = falseTarget;
            }

            public void Throw(BigInteger item) => GetReceptor(item).Catch(item);

            private Monkey GetReceptor(BigInteger item)
                => item % _divisible == 0 ?
                    _collectionPointer[_trueTarget] :
                    _collectionPointer[_falseTarget];

        }

        public class MonkeyBuilder
        {
            private IList<Monkey> _collectionPointer;
            private Func<BigInteger, BigInteger>? _worryFactor;

            public MonkeyBuilder(IList<Monkey> collectionPointer)
            {
                _collectionPointer = collectionPointer;
            }

            public MonkeyBuilder WithWorryModifier(Func<BigInteger, BigInteger> worryFactor)
            {
                _worryFactor = worryFactor;
                return this;
            }

            public Monkey Build(string text)
            {
                var lines = text.Split('\n');
                return new Monkey(GetItems(lines), GetWorryLevelOperation(lines), GetThrowOperation(lines), _worryFactor);
            }

            private IList<BigInteger> GetItems(string[] lines) => lines[1].Split(':')[1].Split(',').Select(i => BigInteger.Parse(i.Trim())).ToList();
            private string GetWorryLevelOperation(string[] lines) => lines[2].Split(" = ")[1];
            private ThrowOperation GetThrowOperation(string[] lines)
            {
                return new ThrowOperation(
                    _collectionPointer,
                    Convert.ToInt32(lines[3].Split(' ').Last()),
                    Convert.ToInt32(lines[4].Split(' ').Last()),
                    Convert.ToInt32(lines[5].Split(' ').Last())
                );
            }
        }

        public class Monkey
        {
            private Func<BigInteger, BigInteger> _worryModfierFunc;
            private readonly IList<BigInteger> _items;
            private readonly string _worryExpression;
            private readonly ThrowOperation _throwOperation;
            public long InspectionCount { get; private set; } = 0;

            public Monkey(IList<BigInteger> items, string worryEval, ThrowOperation throwOperation, Func<BigInteger, BigInteger> worryOperation)
            {
                _items = items;
                _worryExpression = worryEval;
                _throwOperation = throwOperation;
                _worryModfierFunc = worryOperation;
            }

            public void ThrowAllItems()
            {
                if (!_items.Any())
                    return;

                int toInspect = _items.Count;
                for (int i = 0; i < toInspect; i++)
                    InspectNext();
            }

            public string ShowItems() => string.Join(", ", _items.ToArray());

            public void Catch(BigInteger item) => _items.Add(item);

            private BigInteger GetWorry(BigInteger item)
            {
                var worry = BigIntExpressionEvaluator.Parse(_worryExpression.Replace("old", item.ToString()));
                return _worryModfierFunc(worry);
            }

            private void InspectNext()
            {
                var item = _items[0];
                _items.RemoveAt(0);
                var worry = GetWorry(item);
                InspectionCount++;
                _throwOperation.Throw(worry);
            }
        }

        [Fact]
        public void Day11_Part1()
        {
            var text = System.IO.File.ReadAllText("Inputs/day11_sample.txt");
            // var text = System.IO.File.ReadAllText("/home/feko/src/dotnet/aoc/test/day11.txt");

            var definitions = text.Split("\n\n");
            var monkeys = new List<Monkey>(definitions.Length);

            foreach (string monkeyDefinition in definitions)
                monkeys.Add(new MonkeyBuilder(monkeys).WithWorryModifier(item => item / 3).Build(monkeyDefinition));

            for (int round = 0; round < 20; round++)
            {
                foreach (var monkey in monkeys)
                    monkey.ThrowAllItems();
            }

            var inspections = monkeys.Select(m => m.InspectionCount).OrderBy(m => m).ToList();
            Assert.Equal(10605, inspections[inspections.Count - 1] * inspections[inspections.Count - 2]);
        }

        private int GetGlobalMod(string[] monkeyDefs) =>
            monkeyDefs.SelectMany(def => def.Split('\n'))
            .Where(line => line.Contains("Test:"))
            .Select(line => Convert.ToInt32(line.Split(' ').Last()))
            .Aggregate((x, y) => x * y);


        [Fact]
        public void Test2()
        {
            var text = System.IO.File.ReadAllText("Inputs/day11_sample.txt");
            //var text = System.IO.File.ReadAllText("/home/feko/src/dotnet/aoc/test/day11.txt");

            var definitions = text.Split("\n\n");
            var monkeys = new List<Monkey>(definitions.Length);
            var mod = GetGlobalMod(definitions);

            foreach (string monkeyDefinition in definitions)
                monkeys.Add(new MonkeyBuilder(monkeys).WithWorryModifier(item => item % mod).Build(monkeyDefinition));

            for (int round = 0; round < 10_000; round++)
            {
                foreach (var monkey in monkeys)
                    monkey.ThrowAllItems();
            }

            var inspections = monkeys.Select(m => m.InspectionCount).OrderBy(m => m).ToList();
            Assert.Equal(2713310158, BigInteger.Multiply(Convert.ToInt64(inspections[inspections.Count - 1]), Convert.ToInt64(inspections[inspections.Count - 2])));
        }
    }
}