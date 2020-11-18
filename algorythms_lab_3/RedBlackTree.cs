using System;

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

        public void Insert(params T[] values)
        {
            foreach (var value in values)
                Insert(value);
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
            if ((isNext ? node?.Right : node?.Left) != null)
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
            Node deletingNode = Find(value);
            bool isLeft = deletingNode?.Parent?.Left == deletingNode;
            Node nextValueNode = null;
            if (deletingNode.Left == null && deletingNode.Right == null) //haven't childs
                deletingNode = RemoveWithoutChilds(deletingNode, isLeft);
            if ((deletingNode.Left != null && deletingNode.Right == null) || (deletingNode.Left == null && deletingNode.Right != null)) // have one child
                deletingNode = RemoveWithOneChild(deletingNode, isLeft);
            else if(deletingNode.Left != null && deletingNode.Right != null)//have two childs
                nextValueNode = RemoveWithTwoChilds(deletingNode, isLeft);
            if (nextValueNode != null)
            {
                if (deletingNode != nextValueNode)
                {
                    _ = nextValueNode.Parent.Left == nextValueNode ?
                        nextValueNode.Parent.Left = null : nextValueNode.Parent.Right = null;
                    if (deletingNode != Root)
                    {
                        if (isLeft)
                            deletingNode.Parent.Left = nextValueNode;
                        else
                            deletingNode.Parent.Right = nextValueNode;
                        nextValueNode.Parent = deletingNode.Parent;
                    }

                    //deletingNode.Value = nextValueNode.Value;
                    //if (deletingNode != Root)
                    //    _ = deletingNode?.Parent?.Left != null ?
                    //        deletingNode = nextValueNode : deletingNode.Parent.Left = nextValueNode;

                    nextValueNode.Parent = null;
                    nextValueNode.Left = deletingNode.Left != null ? deletingNode.Left : null;
                    nextValueNode.Right = deletingNode.Right != null ? deletingNode.Right : null;
                    if (deletingNode == Root)
                    {
                        //Root.Color = Color.Black;
                        //_ = nextValueNode?.Parent?.Left == nextValueNode ?
                        // nextValueNode.Parent.Left = null : nextValueNode.Parent.Right = null;
                        Root = nextValueNode;
                        Root.Color = Color.Black;
                    }
                    else
                        deletingNode.Color = nextValueNode.Color;

                }
                if (nextValueNode.Color == Color.Black)
                    FixRemoving(nextValueNode);
            }
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
            Node nextValueNode = FindNear(deletingNode.Value, Direction.Left);
            if (nextValueNode?.Right != null)
                nextValueNode.Right.Parent = nextValueNode.Parent;
            if (deletingNode == Root)
            {
               // _ = nextValueNode?.Parent?.Left == nextValueNode ?
                // nextValueNode.Parent.Left = null : nextValueNode.Parent.Right = null;
                Root = nextValueNode;
            }
            else
            {
                if (isLeft)
                {
                   // _ = nextValueNode.Parent.Left == nextValueNode ?
                      //  nextValueNode.Parent.Left = null : nextValueNode.Parent.Right = null;
                    //deletingNode.Parent.Left = nextValueNode; // первого? ?.Right
                    //nextValueNode.Left = deletingNode.Left != null ? deletingNode.Left : null;
                    //nextValueNode.Right = deletingNode.Right != null ? deletingNode.Right : null;
                }
                else
                {
                   // _ = nextValueNode.Parent.Left == nextValueNode ?
                    //    nextValueNode.Parent.Left = null : nextValueNode.Parent.Right = null;
                    //deletingNode.Parent.Right = nextValueNode; // первого?
                    //nextValueNode.Left = deletingNode.Left != null ? deletingNode.Left : null;
                    //nextValueNode.Right = deletingNode.Right != null ? deletingNode.Right : null;
                }
            }
            return nextValueNode;
        }

        private void FixRemoving(Node currentNode)
        {
            if (currentNode == Root)
                currentNode.Color = Color.Black;
            else
            {
                bool isLeft = currentNode.Parent.Left == currentNode;
                while (currentNode.Color == Color.Black && currentNode != Root)
                    if (isLeft)
                        currentNode = FixRemovingIfLeftChild(currentNode);
                    else
                        currentNode = FixRemovingIfRightChild(currentNode);
                currentNode.Color = Color.Black;
                Root.Color = Color.Black;
            }
        }

        private Node FixRemovingIfLeftChild(Node currentNode)
        {
            if (currentNode.Sibling.Color == Color.Red)
            {
                currentNode.Sibling.Color = Color.Black;
                currentNode.Parent.Color = Color.Red;
                Rotate(currentNode.Parent, Direction.Left);
            }
            if (currentNode.Sibling.Left.Color == Color.Black
                && currentNode.Sibling.Right.Color == Color.Black)
                currentNode.Sibling.Color = Color.Red;
            else
            {
                if (currentNode.Sibling.Right.Color == Color.Black)
                {
                    currentNode.Sibling.Left.Color = Color.Black;
                    currentNode.Sibling.Color = Color.Red;
                    Rotate(currentNode.Sibling, Direction.Right);
                }
                currentNode.Sibling.Color = currentNode.Parent.Color;
                currentNode.Parent.Color = Color.Black;
                currentNode.Sibling.Right.Color = Color.Black;
                Rotate(currentNode.Parent, Direction.Left);
                currentNode.Color = Root.Color; //?
            }
            return currentNode;
        }

        private Node FixRemovingIfRightChild(Node currentNode)
        {
            if (currentNode.Sibling.Color == Color.Red)
            {
                currentNode.Sibling.Color = Color.Black;
                currentNode.Parent.Color = Color.Red;
                Rotate(currentNode.Parent, Direction.Right);
            }
            if (currentNode.Sibling.Left.Color == Color.Black
                && currentNode.Sibling.Right.Color == Color.Black)
                currentNode.Sibling.Color = Color.Red;
            else
            {
                if (currentNode.Sibling.Left.Color == Color.Black)
                {
                    currentNode.Sibling.Right.Color = Color.Black;
                    currentNode.Sibling.Color = Color.Red;
                    Rotate(currentNode.Sibling, Direction.Left);
                }
                currentNode.Sibling.Color = currentNode.Parent.Color;
                currentNode.Parent.Color = Color.Black;
                currentNode.Sibling.Left.Color = Color.Black;
                Rotate(currentNode.Parent, Direction.Right);
                currentNode = Root; //?
            }
            return currentNode;
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