using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Xunit;

namespace TestProject1
{
    public class Day20
    {
        List<(Guid, int)> items = default;
        Dictionary<Guid, int> pointers = default;

        [Fact]
        public void Day20_Part1()
        {
            //int expected = 3; var lines = File.ReadAllLines("Inputs/day20_sample.txt");
            int expected = 6712; var lines = File.ReadAllLines("Inputs/day20.txt");
            items = lines.Select(line => (Guid.NewGuid(), int.Parse(line))).ToList();
            pointers = items.Select((item, index) => new { addr = item.Item1, idx = index }).ToDictionary(k => k.addr, v => v.idx);

            for (int i = 0; i < items.Count; i++)
            {
                MoveOneItem(items[i]);
            }

            var targets = new[] { 1000, 2000, 3000 };
            int sum = targets.Select(x => GetNumberAt(x)).Sum();

            Assert.Equal(expected, sum); 
        }

        private int GetNumberAt(int target)
        {
            var zeroPointer = items.First(x => x.Item2 == 0).Item1;
            var zeroIndex = pointers[zeroPointer];

            int targetIndex = (zeroIndex + target) % items.Count;

            var itemPointer = pointers.First(x => x.Value == targetIndex).Key;
            return items.First(x => x.Item1 == itemPointer).Item2;
        }

        private void MoveOneItem((Guid pointer, int value) item)
        {
            if (item.value == 0) // uhhh... do nothing?
                return;

            var currentIndex = GetCurrentIndex(item.pointer);
            int newIndex = GetNewIndex(item, currentIndex);

            UpdateOtherItemsIndex(currentIndex, newIndex);
            pointers[item.pointer] = newIndex;
        }

        private int GetNewIndex((Guid pointer, int value) item, int currentIndex)
        {
            var newIndex = currentIndex + item.value;
            if (Math.Abs(newIndex) >= items.Count()-1)
                newIndex = newIndex % (items.Count() -1);

            if (newIndex < 0)
            {
                return items.Count() -1 - Math.Abs(newIndex);
            }

            if (newIndex == 0)
                newIndex = items.Count() - 1;

            return newIndex;
        }

        private void UpdateOtherItemsIndex(int currentIndex, int newIndex)
        {
            if (newIndex == currentIndex) // uhhhhh... do nothing?
                return;

            if (newIndex > currentIndex)
            {
                var itemsToUpdate = pointers.Where(x => x.Value > currentIndex && x.Value <= newIndex).Select(x => x.Key).ToList();
                foreach (var item in itemsToUpdate)
                {
                    pointers[item] = pointers[item] - 1;
                }
            }
            else
            {
                var itemsToUpdate = pointers.Where(x => x.Value >= newIndex && x.Value < currentIndex).Select(x => x.Key).ToList();
                foreach (var item in itemsToUpdate)
                {
                    pointers[item] = pointers[item] + 1;
                }
            }
        }

        private int GetCurrentIndex(Guid pointer) => pointers[pointer];

        public void Print(int amount = 0)
        {
            if (amount == 0)
                amount = items.Count();
            
            var sorted = pointers.OrderBy(x => x.Value).ToList();
            for (int i = 0; i < amount; i++)
            {
                var item = items.FirstOrDefault(x => x.Item1 == sorted[i].Key).Item2;
                Console.Write($"{item}, ");
            }
        }
    }
}
