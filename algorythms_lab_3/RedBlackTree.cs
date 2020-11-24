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
                if (_root?.Parent is null)
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
            while (currentNode is not null)
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
            if ((isNext ? node?.Right : node?.Left) is not null)
                return (isNext ? node.Right.Min() : node.Left.Max());
            else
                while (currentNode is not null
                        && ((isNext ? currentNode : node).CompareTo(isNext ? node : currentNode) <= 0))
                    currentNode = currentNode.Parent;
            return currentNode;
        }

        public Node Min() => Root.Min();

        public Node Max() => Root.Max();

        public void Remove(T value) => Remove(Find(value));

        private void Remove(Node node)
        {
            if (IsBlack(node))
                RemoveBlack(node);
            else
                RemoveRed(node);
        }

        private void RemoveRed(Node node)
        {
            if (node.Left is not null && node.Right is not null)
            {
                var swappingNode = node.Left.Max();
                node.SwapValues(swappingNode);
                Remove(swappingNode);
            }
            else
                _ = IsLeft(node) ? node.Parent.Left = null : node.Parent.Right = null;
        }

        private void RemoveBlack(Node node)
        {
            if (node.Left is not null && node.Right is not null)
            {
                var swappingNode = node.Left.Max();
                node.SwapValues(swappingNode);
                Remove(swappingNode);
            }
            else if (node.Left is not null && node.Right is null)
            {
                var swappingNode = node.Left;
                node.SwapValues(swappingNode);
                Remove(swappingNode);
            }
            else if (node.Left is null && node.Right is not null)
            {
                var swappingNode = node.Right;
                node.SwapValues(swappingNode);
                Remove(swappingNode);
            }
            else if (node == Root && node.Left is null && node.Right is null)
                Root = null;
            else
            {
                var isLeft = IsLeft(node);
                FixRemovingBlackWithoutChildren(node, isLeft);
                _ = isLeft ? node.Parent.Left = null : node.Parent.Right = null;
            }
        }

        private void FixRemovingBlackWithoutChildren(Node fixingNode, bool isLeft)
        {
            var sibling = fixingNode.Sibling;
            var parent = fixingNode.Parent;
            isLeft = !isLeft;
            if (!IsBlack(parent)
                && IsBlack(sibling)
                && IsBlack(sibling.Left)
                && IsBlack(sibling.Right))
            {
                parent.Color = Color.Black;
                sibling.Color = Color.Red;
            }
            else if (!IsBlack(parent)
                && IsBlack(sibling)
                && (isLeft ? !IsBlack(sibling.Left) : !IsBlack(sibling.Right)))
            {
                parent.Color = Color.Black;
                sibling.Color = Color.Red;
                (isLeft ? sibling.Left : sibling.Right).Color = Color.Black;
                Rotate(parent, isLeft ? Direction.Right : Direction.Left);
            }
            else if (IsBlack(parent) 
                && !IsBlack(sibling) 
                && (isLeft ? sibling.Right is not null : sibling.Left is not null)
                && IsBlack(isLeft ? sibling.Right?.Left : sibling.Left?.Left)
                && IsBlack(isLeft ? sibling?.Right?.Right :sibling?.Left?.Right))
            {
                sibling.Color = Color.Black;
                (isLeft ? sibling.Right : sibling.Left).Color = Color.Red;
                Rotate(parent, isLeft ? Direction.Right : Direction.Left);
            }
            else if (IsBlack(parent)
                && !IsBlack(sibling)
                && (isLeft ? sibling.Right is not null : sibling.Left is not null)
                && (isLeft ? IsBlack(sibling.Right) : IsBlack(sibling.Left))
                && (isLeft ? !IsBlack(sibling.Right?.Left) : !IsBlack(sibling.Left?.Right)))
            {
                (isLeft ? sibling.Right.Left : sibling.Left.Right).Color = Color.Black;
                Rotate(sibling, isLeft ? Direction.Left : Direction.Right);
                Rotate(parent, isLeft ? Direction.Right : Direction.Left);
            }
            else if (IsBlack(parent) 
                && IsBlack(sibling)
                && (isLeft ? !IsBlack(sibling.Right) : !IsBlack(sibling.Left)))
            {
                (isLeft ? sibling.Right : sibling.Left).Color = Color.Black;
                Rotate(sibling, isLeft ? Direction.Left : Direction.Right);
                Rotate(parent, isLeft ? Direction.Right : Direction.Left);
            }
            else if (IsBlack(parent)
                && IsBlack(sibling)
                && !IsBlack(isLeft ? sibling.Left : sibling.Right))
            {
                (isLeft ? sibling.Left : sibling.Right).Color = Color.Black;
                Rotate(parent, isLeft ? Direction.Right : Direction.Left);
            }
            else if (IsBlack(parent)
                && IsBlack(sibling)
                && IsBlack(sibling.Left)
                && IsBlack(sibling.Right))
            {
                sibling.Color = Color.Red;
                if(parent != Root)
                    FixRemovingBlackWithoutChildren(parent, IsLeft(parent));
            }
        }

        public bool IsBlack(Node node) => node == null || node.Color == Color.Black;

        public bool IsLeft(Node node) => node == node.Parent.Left;

        private void InsertCase_1(Node node)
        {
            if (node.Parent is null)
                node.Color = Color.Black;
            else if (node.Parent.Color == Color.Red)
                InsertCase_2(node);
        }

        private void InsertCase_2(Node node)
        {
            var uncle = node.Uncle;
            if (uncle is not null && uncle.Color == Color.Red)
            {
                node.Parent.Color = Color.Black;
                uncle.Color = Color.Black;
                var grandParent = node.GrandParent;
                grandParent.Color = Color.Red;
                InsertCase_1(grandParent);
            }
            else
                InsertCase_3(node);
        }

        private void InsertCase_3(Node node)
        {
            var grandParent = node.GrandParent;
            if (node == node.Parent.Right && node.Parent == grandParent.Left)
            {
                Rotate(node.Parent, Direction.Left);
                node = node.Left;
            }
            else if (node == node.Parent.Left && node.Parent == grandParent.Right)
            {
                Rotate(node.Parent, Direction.Right);
                node = node.Right;
            }
            InsertCase_4(node);
        }

        private void InsertCase_4(Node node)
        {
            var grandParent = node.GrandParent;
            node.Parent.Color = Color.Black;
            grandParent.Color = Color.Red;
            Rotate(
                grandParent, 
                node == node.Parent.Left 
                && node.Parent == grandParent.Left 
                    ? Direction.Right 
                    : Direction.Left);
        }

        private void Rotate(Node node, Direction direction)
        {
            var isRight = direction == Direction.Right;
            var pivot = isRight ? node.Left : node.Right;
            if(pivot == null)
                pivot = isRight ? node.Parent.Left : node.Parent.Right;
            pivot.Parent = node.Parent;
            _ = isRight ? node.Left = pivot.Right : node.Right = pivot.Left;
            _ = isRight ? pivot.Right = node : pivot.Left = node;
        }

        public class Node : IComparable<Node>
        {
            public T Value { get; private set; }

            public Color Color { get; internal set; } //help

            private Node _Left;
            public Node Left
            {
                get => _Left;
                internal set
                {
                    _Left = value;
                    if (value is not null)
                        value._Parent = this;
                }
            }

            private Node _Right;
            public Node Right
            {
                get => _Right;
                internal set
                {
                    _Right = value;
                    if (value is not null)
                        value._Parent = this;
                }
            }

            private Node _Parent;
            public Node Parent
            {
                get => _Parent;
                internal set
                {
                    _Parent = value;
                    if (value is not null)
                        if (CompareTo(value) > 0)
                            value._Right = this;
                        else
                            value._Left = this;
                }
            }

            public Node(T value) 
            {
                Value = value;
                Color = Color.Red;
            }

            public Node GrandParent
                => Parent?.Parent;

            public Node Sibling
                //=> CompareTo(Parent) < 0 ? Parent?.Right : Parent?.Left;
                => this == Parent?.Left ? Parent?.Right : Parent?.Left;
            public Node Uncle
                => Parent?.Sibling;

            public Node Min()
            {
                var currentNode = this;
                while (currentNode.Left is not null)
                    currentNode = currentNode.Left;
                return currentNode;
            }

            public Node Max()
            {
                var currentNode = this;
                while (currentNode.Right is not null)
                    currentNode = currentNode.Right;
                return currentNode;
            }

            public void SwapValues(Node node)
            {
                var value = node.Value;
                node.Value = Value;
                Value = value;
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