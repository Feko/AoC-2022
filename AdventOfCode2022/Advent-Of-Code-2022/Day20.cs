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
        List<(Guid, long)> items = default;
        Dictionary<Guid, int> pointers = default;

        [Fact]
        public void Day20_Part1()
        {
            //int expected = 3; var lines = File.ReadAllLines("Inputs/day20_sample.txt");
            int expected = 6712; var lines = File.ReadAllLines("Inputs/day20.txt");
            items = lines.Select(line => (Guid.NewGuid(), long.Parse(line))).ToList();
            pointers = items.Select((item, index) => new { addr = item.Item1, idx = index }).ToDictionary(k => k.addr, v => v.idx);

            for (int i = 0; i < items.Count; i++)
            {
                MoveOneItem(items[i]);
            }

            var targets = new[] { 1000, 2000, 3000 };
            long sum = targets.Select(x => GetNumberAt(x)).Sum();

            Assert.Equal(expected, sum); 
        }

        [Fact]
        public void Day20_Part2()
        {
            //long expected = 1623178306; var lines = File.ReadAllLines("Inputs/day20_sample.txt");
            long expected = 1595584274798; var lines = File.ReadAllLines("Inputs/day20.txt");
            long decryptKey = 811589153;
            items = lines.Select(line => (Guid.NewGuid(), long.Parse(line) * decryptKey)).ToList();
            pointers = items.Select((item, index) => new { addr = item.Item1, idx = index }).ToDictionary(k => k.addr, v => v.idx);

            for (int i = 0; i < 10; i++)
            {
                foreach (var item in items)
                {
                    MoveOneItem(item);
                }
            }

            var targets = new[] { 1000, 2000, 3000 };
            long sum = targets.Select(x => GetNumberAt(x)).Sum();

            Assert.Equal(expected, sum);
        }

        private long GetNumberAt(int target)
        {
            var zeroPointer = items.First(x => x.Item2 == 0).Item1;
            var zeroIndex = pointers[zeroPointer];

            int targetIndex = (zeroIndex + target) % items.Count;

            var itemPointer = pointers.First(x => x.Value == targetIndex).Key;
            return items.First(x => x.Item1 == itemPointer).Item2;
        }

        private void MoveOneItem((Guid pointer, long value) item)
        {
            if (item.value == 0) // uhhh... do nothing?
                return;

            var currentIndex = GetCurrentIndex(item.pointer);
            int newIndex = GetNewIndex(item, currentIndex);

            UpdateOtherItemsIndex(currentIndex, newIndex);
            pointers[item.pointer] = newIndex;
        }

        private int GetNewIndex((Guid pointer, long value) item, int currentIndex)
        {
            long newIndex = currentIndex + item.value;
            if (Math.Abs(newIndex) >= items.Count()-1)
                newIndex = newIndex % (items.Count() -1);

            if (newIndex < 0)
            {
                return Convert.ToInt32(items.Count() -1 - Math.Abs(newIndex));
            }

            if (newIndex == 0)
                newIndex = items.Count() - 1;

            return Convert.ToInt32(newIndex);
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
    }
}
