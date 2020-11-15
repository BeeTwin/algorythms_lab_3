using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Runtime.CompilerServices;
using System.Text;

namespace algorythms_lab_3
{
    public class RedBlackTreeNode<T> where T : IComparable
    {
        public enum Color
        {
            Black,
            Red
        }

        public class Node : IComparable<Node>
        {
            public T Value;

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

            public Node _parent;
            public Node Parent
            {
                get => _parent;
                set
                {
                    _parent = value;
                    if (value != null)
                        if (Value.CompareTo(value) > 0)
                            value._right = this;
                        else
                            value._left = this;
                }
            }
            public Color Color;

            public Node(T value, Node left, Node right, Color color)
            {
                Value = value;
                Left = left;
                Right = right;
                Color = color;
            }

            public Node(T value) : this(value, null, null, Color.Red) { }

            public int CompareTo(Node node)
                => Value.CompareTo(node.Value);

            public override string ToString()
                => $"{Value} - {(Color == Color.Red ? "Red" : "Black")}";
        }

        public Node Root;

        private Node GetGrandparent(Node node) => node?.Parent?.Parent;

        private Node GetUnckle(Node node)
        {
            var grandparent = GetGrandparent(node);
            return node?.Parent == grandparent?.Left ? grandparent?.Right : grandparent?.Left;
        }

        public void RotateLeft(Node node)
        {
            /*    
            struct node *pivot = n->right;
            pivot->parent = n->parent; 
            if (n->parent != NULL)
            {
                if (n->parent->left == n)
                    n->parent->left = pivot;
                else
                    n->parent->right = pivot;
            }
            n->right = pivot->left;
            if (pivot->left != NULL)
                pivot->left->parent = n;
            n->parent = pivot;
            pivot->left = n;
            */

            var pivot = node.Right;
            pivot.Parent = node.Parent;
            if (node.Parent == null)
            {
                Root = pivot;

            }
        }

        private void RotateRight(Node node)
        {

        }

        public void Insert(T value)
        {
            var goal = new Node(value);
            var currentNode = Root ??= goal;
            while (currentNode != goal)
                if (goal.CompareTo(currentNode) >= 0)
                    currentNode = (currentNode.Right ??= goal);
                else
                    currentNode = (currentNode.Left ??= goal);
            CheckCase_1(currentNode);
        }

        private void CheckCase_1(Node node)
        {
            if (node.Parent == null)
            {
                Root = node;
                node.Color = Color.Black;
            }
            else if (node.Parent.Color == Color.Red)
                CheckCase_2(node);
        }

        private void CheckCase_2(Node node)
        {
            var uncle = GetUnckle(node);
            if (uncle != null && uncle.Color == Color.Red)
            {
                node.Parent.Color = Color.Black;
                uncle.Color = Color.Black;
                var grandparent = GetGrandparent(node);
                grandparent.Color = Color.Red;
                CheckCase_1(grandparent);
            }
            else
                CheckCase_3(node);
        }

        private void CheckCase_3(Node node)
        {
            var grandparent = GetGrandparent(node);
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
            CheckCase_4(node);
        }

        private void CheckCase_4(Node node)
        {
            var grandparent = GetGrandparent(node);
            node.Parent.Color = Color.Black;
            grandparent.Color = Color.Red;
            if (node == node.Parent.Left && node.Parent == grandparent.Left)
                RotateRight(grandparent);
            else
                RotateLeft(grandparent);
        }
    }
}
