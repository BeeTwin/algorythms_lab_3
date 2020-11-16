using System;

namespace algorythms_lab_3
{
    class Program
    {
        static void Main(string[] args)
        {
            _Test();
        }

        private static void _Test()
        {
            var a = new RedBlackTree<int>();
            var b = a.Root.Color;
            
            a.Insert(14);
            a.Root.Color = RedBlackTree<int>.Color.Red;
            a.Insert(20);
            a.Insert(100);
            a.Insert(13);
            a.Insert(2);
            a.Insert(3);
            a.Insert(5);
            a.Insert(6);
            a.Insert(22);
            a.Insert(30);
            a.Insert(56);
            a.Remove(20);
            a.Insert(101);
            a.Insert(71);
            a.Insert(4);
            a.Insert(8);
            a.Insert(72);
        }
    }
}
