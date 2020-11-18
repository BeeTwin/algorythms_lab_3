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
            
            a.Insert(14);
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
            a.Insert(101);
            a.Insert(71);
            a.Insert(4);
            a.Insert(8);
            a.Insert(72);
            a.Remove(100);
            a.Remove(13);
            a.Remove(2);
            a.Remove(30);

        }
    }
}
