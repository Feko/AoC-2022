using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestProject1
{
    public class Day07
    {
        interface INode
        {
            string Name { get; set; }
            bool IsFile();
            long GetSize();
        }

        class Directory : INode
        {
            public IList<INode> Subitems = new List<INode>();
            public string Name { get; set; }
            public Directory Parent;

            public Directory(string name, Directory parent)
            {
                Name = name;
                Parent = parent;
            }

            public long GetSize() => Subitems.Select(i => i.GetSize()).Sum();
            public void AddFile(string line) => Subitems.Add(new File(line));
            public void AddDirectory(string line) => Subitems.Add(new Directory(line.Split(' ')[1], this));
            public bool IsFile() => false;
        }

        class File : INode
        {
            public string Name { get; set; }
            public long Size;

            public long GetSize() => Size;
            public bool IsFile() => true;
            public File(string row)
            {
                var parts = row.Split(' ');
                Name = parts[1];
                Size = Convert.ToInt64(parts[0]);
            }
        }

        private Directory BuildTree(string[] lines)
        {
            Directory rootNode = new Directory("/", null);
            Directory currentNode = rootNode;

            foreach (string line in lines.Skip(1))
            {
                if (string.IsNullOrEmpty(line) || line.StartsWith("$ ls"))
                    continue;

                if (Char.IsNumber(line[0]))
                    currentNode.AddFile(line);

                if (line.StartsWith("dir "))
                    currentNode.AddDirectory(line);

                if (line.StartsWith("$ cd"))
                {
                    currentNode = line.Split(' ')[2] switch
                    {
                        ".." => currentNode.Parent,
                        "/" => rootNode,
                        _ => (Directory)currentNode.Subitems.First(sub => sub.Name == line.Split(' ')[2])
                    };
                }
            }
            return rootNode;
        }

        private IList<INode> ClimbTheTree(Directory node, long threshold)
        {
            var result = new List<INode>();
            foreach (var subdir in node.Subitems.Where(i => !i.IsFile()))
            {
                result.AddRange(ClimbTheTree((Directory)subdir, threshold));
            }

            if (node.GetSize() <= threshold)
                result.Add(node);
            return result;
        }


        [Fact]
        public void Test1()
        {
            var text = System.IO.File.ReadAllLines("Inputs/day07.txt");
            var tree = BuildTree(text);
            var result = ClimbTheTree(tree, 100_000);

            Assert.Equal(95437, result.Sum(x => x.GetSize()));

        }
    }
}
