using System;

namespace algorythms_lab_3
{
    public class RedBlackTree<T> where T : IComparable
    {
        public enum Color
        {
            Black,
            Red
        }

        public class Node : IComparable<Node>
        {
            public T Value { get; private set; }

            public Color Color { get; /*private :(*/set; }

            private Node _left;
            public Node Left
            {
                get => _left;
                set
                {
                    _left = value;
                    if (value != null)
                        value._parent = this;
                }
            }

            private Node _right;
            public Node Right
            {
                get => _right;
                set
                {
                    _right = value;
                    if (value != null)
                        value._parent = this;
                }
            }

            private Node _parent;
            public Node Parent
            {
                get => _parent;
                set
                {
                    _parent = value;
                    if (value != null)
                        if (CompareTo(value) > 0)
                            value._right = this;
                        else
                            value._left = this;
                }
            }

            public Node(T value, Node left, Node right, Color color)
            {
                Value = value;
                Left = left;
                Right = right;
                Color = color;
            }

            public Node(T value) : this(value, null, null, Color.Red) { }

            public Node Grandparent 
                => Parent?.Parent;

            public Node Sibling
                => this == Parent?.Left ? Parent?.Right : Parent?.Left;

            public Node Unckle 
                => Parent?.Sibling;

            public int CompareTo(Node node)
                => Value.CompareTo(node.Value);

            public override string ToString()
                => $"{Value} - {(Color == Color.Red ? "Red" : "Black")}";
        }

        public Node Root { get; private set; }

        public void Insert(T value)
        {
            var goal = new Node(value);
            var currentNode = Root ??= goal;
            while (currentNode != goal)
                if (goal.CompareTo(currentNode) >= 0)
                    currentNode = (currentNode.Right ??= goal);
                else
                    currentNode = (currentNode.Left ??= goal);
            InsertCase_1(currentNode);
        }

        public Node Find(T value)
        {
            var currentNode = Root;
            int cmp;
            while (currentNode != null)
                if ((cmp = value.CompareTo(currentNode.Value)) > 0)
                    currentNode = currentNode.Right;
                else if (cmp < 0)
                    currentNode = currentNode.Left;
                else
                    break;
            return currentNode;
        }

        public Node Min(Node node)
        {
            var currentNode = node;
            while (currentNode.Left != null)
                currentNode = currentNode.Left;
            return currentNode;
        }

        public Node Max(Node node)
        {
            var currentNode = node;
            while (currentNode.Right != null)
                currentNode = currentNode.Right;
            return currentNode;
        }

        public void Remove(T value)
        {
            var removing = Find(value);
            if (removing == null)
                return;

        }

        private void RotateLeft(Node node)
        {
            var pivot = node.Right;
            pivot.Parent = node.Parent;
            if (Root == node)
                Root = pivot;
            node.Right = pivot.Left;
            pivot.Left = node;
        }

        private void RotateRight(Node node)
        {
            var pivot = node.Left;
            pivot.Parent = node.Parent;
            if (Root == node)
                Root = pivot;
            node.Left = pivot.Right;
            pivot.Right = node;
        }

        private void InsertCase_1(Node node)
        {
            if (node.Parent == null)
                node.Color = Color.Black;
            else if (node.Parent.Color == Color.Red)
                InsertCase_2(node);
        }

        private void InsertCase_2(Node node)
        {
            var uncle = node.Unckle;
            if (uncle != null && uncle.Color == Color.Red)
            {
                node.Parent.Color = Color.Black;
                uncle.Color = Color.Black;
                var grandparent = node.Grandparent;
                grandparent.Color = Color.Red;
                InsertCase_1(grandparent);
            }
            else
                InsertCase_3(node);
        }

        private void InsertCase_3(Node node)
        {
            var grandparent = node.Grandparent;
            if(node == node.Parent.Right && node.Parent == grandparent.Left)
            {
                RotateLeft(node.Parent);
                node = node.Left;
            }
            else if (node == node.Parent.Left && node.Parent == grandparent.Right)
            {
                RotateRight(node.Parent);
                node = node.Right;
            }
            InsertCase_4(node);
        }

        private void InsertCase_4(Node node)
        {
            var grandparent = node.Grandparent;
            node.Parent.Color = Color.Black;
            grandparent.Color = Color.Red;
            if (node == node.Parent.Left && node.Parent == grandparent.Left)
                RotateRight(grandparent);
            else
                RotateLeft(grandparent);
        }
    }
}
