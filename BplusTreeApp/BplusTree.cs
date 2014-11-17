using System;
using System.Collections.Generic;

namespace BplusTreeApp
{
    public class BplusTree
    {
        private int order;
        private int median;
        private Node root;

        public BplusTree(int order)
        {
            this.order = order;
            median = order / 2;
            root = new LeafNode(order);
        }

        public void Insert(int key, object value)
        {
            InnerInsert(key, value, root, true);
        }

        private void InnerInsert(int key, object value, Node currentNode, bool topToBottom)
        {
            if (currentNode.IsLeaf)
            {
                var leaf = currentNode as LeafNode;
                int keyIndex = leaf.InsertKey(key);
                leaf.InsertValue(keyIndex, value);
                SplitIfNeeded(leaf);
            }
            else
            {
                if (topToBottom) // finding a leaf to insert <key, value>
                {
                    var inner = currentNode as InnerNode;
                    int wayToGo = inner.PreviewInsert(key);
                    InnerInsert(key, value, inner.Children[wayToGo], true);
                }
                else // going back and doing necessary splitting
                {
                    currentNode.InsertKey(key);
                    SplitIfNeeded(currentNode);
                }
            }
        }

        private void SplitIfNeeded(Node node)
        {
            if (node.NeedsToBeSplit())
            {
                var newNode = node.Split(median);

                var parent = node.Parent;
                if (parent == null)
                {
                    parent = new InnerNode(order);
                    node.Parent = parent;
                    parent.Children.Add(node);
                    root = parent;
                }

                newNode.Parent = parent;

                int newKeyToParent;
                if (node.IsLeaf)
                {
                    // first key from a new node is copied to parent
                    newKeyToParent = newNode.Keys[0];
                }
                else
                {
                    // median is moved to parent
                    newKeyToParent = node.Keys[median];
                    node.Keys.RemoveAt(median);
                }

                parent.Children.Insert(parent.Children.IndexOf(node) + 1, newNode);
                InnerInsert(newKeyToParent, null, parent, false);
            }
        }

        public object Find(int key)
        {
            return InnerSearch(key, root);
        }

        private object InnerSearch(int key, Node currentNode)
        {
            if (currentNode.IsLeaf)
            {
                var leaf = currentNode as LeafNode;
                return leaf.GetValue(key);
            }

            var inner = currentNode as InnerNode;
            int wayToGo = inner.PreviewInsert(key);
            return InnerSearch(key, inner.Children[wayToGo]);
        }

        public string Dump()
        {
            var result = new string[20 / order];
            InnerDump(result, 0, root);
            return string.Join(Environment.NewLine, result);
        }

        private void InnerDump(string[] result, int depth, Node currentNode)
        {
            currentNode.Dump(result, depth);

            if (!currentNode.IsLeaf)
            {
                var inner = currentNode as InnerNode;
                for (int i = 0; i < inner.Children.Count; i++)
                {
                    InnerDump(result, depth + 1, inner.Children[i]);
                }
            }
        }

        #region Internal structure

        private abstract class Node
        {
            protected int order;
            public InnerNode Parent;
            public bool IsLeaf;
            public List<int> Keys;

            protected Node(int order, bool isLeaf)
            {
                this.order = order;
                IsLeaf = isLeaf;
                Keys = new List<int>(order); // one extra space
            }

            public int PreviewInsert(int key)
            {
                int i;
                for (i = 0; i < Keys.Count; i++)
                {
                    if (Keys[i] > key)
                        break;
                }
                
                return i;
            }

            public int InsertKey(int key)
            {
                var correctPosition = PreviewInsert(key);
                Keys.Insert(correctPosition, key);
                return correctPosition;
            }

            public bool NeedsToBeSplit()
            {
                return Keys.Count > order - 1;
            }

            public abstract Node Split(int median);

            public virtual void Dump(string[] result, int depth)
            {
                result[depth] += "    " + ToString();
            }

            public override string ToString()
            {
                return string.Format("({0})", string.Join(" | ", Keys));
            }
        }

        private class InnerNode : Node
        {
            public List<Node> Children;

            public InnerNode(int order) : base(order, false)
            {
                Children = new List<Node>(order + 1); // one extra space
            }

            public override Node Split(int median)
            {
                var newInner = new InnerNode(order);
                int medianCorrection = median + 1; // for inner nodes, median is moved to parent
                int moveCount = order - medianCorrection;

                var keyRange = Keys.GetRange(medianCorrection, moveCount);
                Keys.RemoveRange(medianCorrection, moveCount);
                newInner.Keys.AddRange(keyRange);

                if (Children.Count > order)
                {
                    int childrenMoveCount = Children.Count / 2;
                    int moveFromIndex = Children.Count - childrenMoveCount;

                    var childrenRange = Children.GetRange(moveFromIndex, childrenMoveCount);
                    newInner.Children.AddRange(childrenRange);
                    Children.RemoveRange(moveFromIndex, childrenMoveCount);

                    for (int i = 0; i < childrenRange.Count; i++)
                    {
                        childrenRange[i].Parent = newInner;
                    }
                }

                return newInner;
            }
        }

        private class LeafNode : Node
        {
            public List<object> Values;
            
            public LeafNode(int order) : base(order, true)
            {
                Values = new List<object>(order);
            }

            public void InsertValue(int keyIndex, object value)
            {
                Values.Insert(keyIndex, value);
            }

            public object GetValue(int key)
            {
                int keyIndex = Keys.IndexOf(key);
                return keyIndex == -1 ? null : Values[keyIndex];
            }

            public override Node Split(int median)
            {
                var newLeaf = new LeafNode(order);
                int moveCount = order - median;

                var keyRange = Keys.GetRange(median, moveCount);
                Keys.RemoveRange(median, moveCount);
                newLeaf.Keys.AddRange(keyRange);

                var valueRange = Values.GetRange(median, moveCount);
                newLeaf.Values.AddRange(valueRange);
                Values.RemoveRange(median, moveCount);

                return newLeaf;
            }

            public override void Dump(string[] result, int depth)
            {
                base.Dump(result, depth); // dump keys
                result[depth + 1] += string.Format("    [{0}]", string.Join(" | ", Values));
            }
        }

        #endregion
    }
}