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

        // test sets

        /*static void Main(string[] args)
        {
            var tree = new BplusTree(3);
            tree.Insert(1, "one");
            tree.Insert(10, "ten");
            tree.Insert(5, "five");
            tree.Insert(3, "three");
            tree.Insert(8, "eight");
            tree.Insert(12, "twelve");
            tree.Insert(7, "seven");
            tree.Insert(9, "nine");
            tree.Insert(20, "twenty");
            tree.Insert(15, "5teen");
            tree.Insert(18, "8teen");
            tree.Insert(25, "twenty5");
            Console.WriteLine(tree.Dump());
        }*/

        /*static void Main(string[] args)
        {
            var tree = new BplusTree(4);
            tree.Insert(1, "one");
            tree.Insert(10, "ten");
            tree.Insert(5, "five");
            tree.Insert(3, "three");
            tree.Insert(8, "eight");
            tree.Insert(12, "twelve");
            tree.Insert(15, "5teen");
            tree.Insert(20, "twenty");
            tree.Insert(25, "twenty5");
            tree.Insert(30, "thirty");
            Console.WriteLine(tree.Dump());
        }*/
    }
}
