using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestProject1
{
    public class Day09
    {
        public record RopePosition : IEqualityComparer<RopePosition>
        {
            public int Line;
            public int Column;

            public RopePosition(int line, int column) => (Line, Column) = (line, column);

            public bool IsTouching(RopePosition otherPosition)
            {
                int lineDiff = GetLineDiff(otherPosition);
                int columnDiff = GetColumnDiff(otherPosition);

                return (lineDiff >= -1 && lineDiff <= 1) && (columnDiff >= -1 && columnDiff <= 1);
            }

            public void Follow(RopePosition head)
            {
                if (IsTouching(head))
                    return;

                if (NeedsDiagonalMove(head))
                    EqualizeNearestAxis(head);

                MoveOneAxis(head);
            }

            private void EqualizeNearestAxis(RopePosition head)
            {
                if (GetLineDiff(head) == -1 || GetLineDiff(head) == 1)
                    Line = head.Line;
                
                if (GetColumnDiff(head) == -1 || GetColumnDiff(head) == 1)
                    Column = head.Column;
            }

            private void MoveOneAxis(RopePosition head)
            {
                if (head.Line.CompareTo(Line) != 0)
                {
                    if (head.Line.CompareTo(Line) > 0)
                        Up();
                    else
                        Down();
                }

                if (head.Column.CompareTo(Column) != 0)
                {
                    if (head.Column.CompareTo(Column) > 0)
                        Right();
                    else
                        Left();
                }
            }

            public bool NeedsDiagonalMove(RopePosition otherPos)
            {
                return
                ((GetLineDiff(otherPos) == -2 || GetLineDiff(otherPos) == 2) && otherPos.Column != Column) ||
                ((GetColumnDiff(otherPos) == -2 || GetColumnDiff(otherPos) == 2) && otherPos.Line != Line);
            }

            public void Up() => Line++;
            public void Down() => Line--;
            public void Right() => Column++;
            public void Left() => Column--;
            public int GetLineDiff(RopePosition otherPosition) => otherPosition.Line - Line;
            public int GetColumnDiff(RopePosition otherPosition) => otherPosition.Column - Column;

            public bool Equals(RopePosition x, RopePosition y) => x.Line == y.Line && x.Column == y.Column;

            public int GetHashCode([DisallowNull] RopePosition pos) => (pos.Line, pos.Column).GetHashCode();
        }

        [Fact]
        public void Day09_Part1()
        {
            //var input = System.IO.File.ReadAllLines("Inputs/day09.txt");
            var input = System.IO.File.ReadAllLines("Inputs/day09_sample.txt");

            RopePosition head = new(0, 0);
            RopePosition tail = new(0, 0);
            HashSet<RopePosition> tailPositions = new HashSet<RopePosition>(2048);

            foreach (string line in input)
            {
                var parts = line.Split(' ');
                for (int counter = 0; counter < Convert.ToInt32(parts[1].ToString()); counter++)
                {
                    MoveHead(head, parts[0]);

                    tail.Follow(head);
                    tailPositions.Add(tail);
                }
            }

            Assert.Equal(13, tailPositions.Count);
        }

        private static void MoveHead(RopePosition head, string direction)
        {
            if (direction == "R")
                head.Right();

            if (direction == "L")
                head.Left();

            if (direction == "U")
                head.Up();

            if (direction == "D")
                head.Down();
        }

        [Fact]
        public void Day09_Part2()
        {
            var input = System.IO.File.ReadAllLines("Inputs/day09_sample_2.txt");
            var knots = Enumerable.Range(0, 10).Select(x => new RopePosition(5, 11)).ToList();

            //var input = System.IO.File.ReadAllLines("Inputs/day09.txt");
            //var knots = Enumerable.Range(0, 10).Select(x => new RopePosition(0, 0)).ToList();

            HashSet<RopePosition> tailPositions = new HashSet<RopePosition>(2048);

            foreach (string line in input)
            {
                var parts = line.Split(' ');
                for (int counter = 0; counter < Convert.ToInt32(parts[1].ToString()); counter++)
                {
                    MoveHead(knots.First(), parts[0]);
                   
                    for (int i = 1; i < knots.Count; i++)
                    {
                        knots[i].Follow(knots[i - 1]);
                    }
                    tailPositions.Add(knots.Last());
                } 
            }

            Assert.Equal(36, tailPositions.Count);
        }
    }
}
