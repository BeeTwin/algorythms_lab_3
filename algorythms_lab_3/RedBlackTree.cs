using System;

namespace algorythms_lab_3
{
    public class RedBlackTree<T> where T : IComparable
    {
        private Node _root;
        public Node Root
        {
            get
            {
                if (_root.Parent == null)
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
                        && (isNext ? currentNode : node).CompareTo(isNext ? node : currentNode) >= 0)
                    currentNode = currentNode.Parent;
            return currentNode;
        }
        public Node Min() => Root.Min();

        public Node Max() => Root.Max();

        public void Remove(T value)
        {
            Node deletingNode = Find(value);
            bool isLeft = deletingNode.Parent.Left == deletingNode;
            Node nextValueNode = null;
            if (deletingNode.Left.Value == null && deletingNode.Right.Value == null) //haven't childs
                deletingNode = RemoveWithoutChilds(deletingNode, isLeft);
            if (deletingNode.Left.Value != null || deletingNode.Right.Value != null) // have one child
                deletingNode = RemoveWithOneChild(deletingNode, isLeft);
            else //have two childs
                nextValueNode = RemoveWithTwoChilds(deletingNode, isLeft);
            if(deletingNode != nextValueNode)
            {
                deletingNode.Color = nextValueNode.Color;
               // deletingNode.Value = nextValueNode.Value;
            }
            if (nextValueNode.Color == Color.Black)
                FixRemoving(deletingNode);
        }

        private Node RemoveWithoutChilds(Node deletingNode, bool isLeft)
        {
            if (deletingNode == Root)
                Root = null;
            else
            {
                if (isLeft)
                    deletingNode.Parent.Left = null;
                else
                    deletingNode.Parent.Right = null;
            }
            return deletingNode;
        }

        private Node RemoveWithOneChild(Node deletingNode, bool isLeft)
        {
            if (isLeft)
                deletingNode.Parent.Left =
                    deletingNode.Left.Value != null ? deletingNode.Left : deletingNode.Right;
            else
                deletingNode.Parent.Right =
                    deletingNode.Left.Value != null ? deletingNode.Left : deletingNode.Right;
            return deletingNode;
        }

        private Node RemoveWithTwoChilds(Node deletingNode, bool isLeft)
        {
            Node nextValueNode = FindNear(deletingNode.Value, Direction.Right);
            if (nextValueNode.Right.Value != null)
                nextValueNode.Right.Parent = nextValueNode.Parent;
            if (nextValueNode == Root)
                Root = nextValueNode.Right;
            else
            {
                if (isLeft)
                    nextValueNode.Parent.Left = nextValueNode.Left;
                else
                    nextValueNode.Parent.Right = nextValueNode.Left;
            }
            return nextValueNode;
        }

        private void FixRemoving(Node currentNode)
        {
            bool isLeft = currentNode.Parent.Left == currentNode;
            while (currentNode.Color == Color.Black && currentNode != Root)
                if(isLeft)
                {
                    if(currentNode.Sibling.Color == Color.Red)
                    {
                        currentNode.Sibling.Color = Color.Black;
                        currentNode.Parent.Color = Color.Red;
                        Rotate(currentNode.Parent, Direction.Left);
                    }
                    if (currentNode.Sibling.Left.Color == Color.Black
                        && currentNode.Sibling.Right.Color == Color.Black)
                        currentNode.Sibling.Color = Color.Red;
                }
        }

        private void InsertCase_1(Node node)
        {
            if (node.Parent == null)
                node.SetColor(Color.Black);
            else if (node.Parent.Color == Color.Red)
                InsertCase_2(node);
        }

        private void InsertCase_2(Node node)
        {
            var uncle = node.Uncle;
            if (uncle != null && uncle.Color == Color.Red)
            {
                node.Parent.SetColor(Color.Black);
                uncle.SetColor(Color.Black);
                var grandparent = node.Grandparent;
                grandparent.SetColor(Color.Red);
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
            node.Parent.SetColor(Color.Black);
            grandparent.SetColor(Color.Red);
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

            internal Color Color { get; set; }

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

            public Node Uncle
                => Parent?.Sibling;

            public void SetColor(Color color)
                => Color = color;

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