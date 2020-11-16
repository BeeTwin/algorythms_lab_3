﻿using System;

namespace algorythms_lab_3
{
    public sealed class RedBlackTree<T> where T : IComparable
    {
        private Node _root;
        public Node Root
        {
            get
            {
                if (_root?.Parent == null)
                    return _root;
                else
                {
                    _root = _root.Parent;
                    return Root;
                }
            }
            private set 
            {
                _root = value; 
            }             
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

        public Node FindPrevious(T value)
            => FindNear(value, Direction.Left);

        public Node FindNext(T value)
            => FindNear(value, Direction.Right);

        private Node FindNear(T value, Direction direction)
        {
            var isNext = direction == Direction.Right;
            var node = Find(value);
            var currentNode = node;
            if ((isNext ? node.Right : node.Left) != null)
                return (isNext ? node.Right.Min() : node.Left.Max());
            else
                while (currentNode != null
                        && ((isNext ? currentNode : node).CompareTo(isNext ? node : currentNode) <= 0))
                    currentNode = currentNode.Parent;
            return currentNode;
        }

        public Node Min() => Root.Min();

        public Node Max() => Root.Max();

        public void Remove(T value)
        {
            var removing = Find(value);
            if (removing == null)
                return;

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
            var uncle = node.Uncle;
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
            if (node == node.Parent.Right && node.Parent == grandparent.Left)
            {
                Rotate(node.Parent, Direction.Left);
                node = node.Left;
            }
            else if (node == node.Parent.Left && node.Parent == grandparent.Right)
            {
                Rotate(node.Parent, Direction.Right);
                node = node.Right;
            }
            InsertCase_4(node);
        }

        private void InsertCase_4(Node node)
        {
            var grandparent = node.Grandparent;
            node.Parent.Color = Color.Black;
            grandparent.Color = Color.Red;
            Rotate(
                grandparent, 
                node == node.Parent.Left 
                && node.Parent == grandparent.Left 
                    ? Direction.Right 
                    : Direction.Left);
        }

        private void Rotate(Node node, Direction direction)
        {
            var isRight = direction == Direction.Right;
            var pivot = isRight ? node.Left : node.Right;
            pivot.Parent = node.Parent;
            _ = isRight ? node.Left = pivot.Right : node.Right = pivot.Left;
            _ = isRight ? pivot.Right = node : pivot.Left = node;
        }

        public class Node : IComparable<Node>
        {
            public T Value { get; private set; }

            public Color Color { get; internal set; } //help

            private Node _left;
            public Node Left
            {
                get => _left;
                internal set
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
                internal set
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
                internal set
                {
                    _parent = value;
                    if (value != null)
                        if (CompareTo(value) > 0)
                            value._right = this;
                        else
                            value._left = this;
                }
            }

            public Node(T value) 
            {
                Value = value;
                Color = Color.Red;
            }

            public Node Grandparent
                => Parent?.Parent;

            public Node Sibling
                => this == Parent?.Left ? Parent?.Right : Parent?.Left;

            public Node Uncle
                => Parent?.Sibling;

            public Node Min()
            {
                var currentNode = this;
                while (currentNode.Left != null)
                    currentNode = currentNode.Left;
                return currentNode;
            }

            public Node Max()
            {
                var currentNode = this;
                while (currentNode.Right != null)
                    currentNode = currentNode.Right;
                return currentNode;
            }

            public int CompareTo(Node node)
                => Value.CompareTo(node.Value);

            public override string ToString()
                => $"{Value} - {Color}";
        }

        public enum Color
        {
            Black,
            Red
        }

        private enum Direction
        {
            Left,
            Right
        }
    }
}