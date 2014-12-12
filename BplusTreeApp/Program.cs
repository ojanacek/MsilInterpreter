using System;
using System.IO;

namespace BplusTreeApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines(args[0]);
            var tree = new BplusTree(3);

            for (int i = 0; i < lines.Length; i++)
            {
                var parts = lines[i].Split();
                int key = int.Parse(parts[0]);
                string value = parts[1];
                tree.Insert(key, value);
            }
            
            File.WriteAllText("output.txt", tree.Dump());
        }
    }
}
