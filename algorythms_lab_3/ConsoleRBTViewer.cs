using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace algorythms_lab_3
{
    class ConsoleRBTViewer : IRBTViewer
    {
        private readonly Dictionary<Message, Func<object, string>> _messages = new Dictionary<Message, Func<object, string>>();

        public ConsoleRBTViewer()
        {
            InitializeMessages();
        }

        public string[] GetInput()
        {
            Out("$: ", 2, 3);
            SetCursorPosition(5, 3);
            string input;
            if ((input = ReadLine()) != null)
                ClearBoxes();             
            return input.Split(' ');
        }

        public void PrepareOutput()
        {
            var line = new string('═', WindowWidth - 2);
            var emptyLine = new string(' ', WindowWidth - 2);
            Out('╔' + line + '╗', 0, 0);
            Out('║' + emptyLine + '║', 0, 1);
            Out('╠' + line + '╣', 0, 2);
            Out('║' + emptyLine + '║', 0, 3);
            Out('╚' + line + '╝', 0, 4);
        }

        public void Out(Message message, object args) 
            => Out("#: " + _messages[message](args), 2, 1);

        private void InitializeMessages()
        {
            _messages[Message.Welcome] = (message)
                => $"Welcome to Console Red Black Tree Viewer. Do you want to generate a starting tree? [\"y\" to yes / any to no]";
            _messages[Message.IncorrectInput] = (message)
                => $"Incorrect input. Use in form: [command] [value (if needed)]. Use \"Help\" to see commands list." ;
            _messages[Message.IncorrectArguments] = (message)
                => $"Incorrect arguments. Use in form: {message}";
            _messages[Message.Help1] = (message)
                => $"Help [(optional) page], Clear, GenerateTree, ShowNILs, HideNILs, Add [int], Remove [int] ... Use \"Help 2\" ...";
            _messages[Message.Help2] = (message)
                => $"Find [int], Min [(optional) int], Max [(optional) int], FindNext [int], FindPrev [int] ... Use \"Help 3\" ...";
            _messages[Message.Help3] = (message)
                => $"All ints must be less than 1000.";
            _messages[Message.Start] = (message)
                => $"Use \"Help\" to see commands list.";
            _messages[Message.HelpPageNotFound] = (message)
                => $"Page {message} is not found.";
            _messages[Message.Empty] = (message) 
                => "";
            _messages[Message.Cleared] = (message)
                => $"Tree is now blank.";
            _messages[Message.AddSuccess] = (message)
                => $"{message} added successfully.";
            _messages[Message.AlreadyContains] = (message)
                => $"The tree already contains {message}.";
            _messages[Message.RemoveSuccess] = (message)
                => $"{message} removed successfully.";
            _messages[Message.DoesntContain] = (message)
                => $"The tree doesn`t contain {message}.";
            _messages[Message.FoundSuccess] = (message)
                => $"({message}) found successfully.";
            _messages[Message.NextGreatest] = (message)
                => $"{message} is the greatest.";
            _messages[Message.PrevLeast] = (message)
                => $"{message} is the least.";
            _messages[Message.Added25] = (message)
                => $"Added 25 values.";
            _messages[Message.NILsAlready] = (message)
                => $"NIL's are already {message}.";
            _messages[Message.NILsNow] = (message)
                => $"NIL's are now {message}.";
        }


        private void ColorOut(ConsoleColor fgColor, ConsoleColor bgColor, string str, int x, int y)
        {
            ForegroundColor = fgColor;
            BackgroundColor = bgColor;
            Out(str, x, y);
            ResetColor();
        }

        private void ClearBoxes()
        {
            var emptyLine = new string(' ', WindowWidth - 2);
            Out(emptyLine, 1, 1);
            Out(emptyLine, 1, 3);
        }

        private void Out(string str, int x, int y)
        {
            SetCursorPosition(x, y);
            Write(str);
        }

        public int DrawTree(RedBlackTree<int>.Node node, int x, int y, bool isShowingNILs)
        {
            ColorOut(
                node.Color == RedBlackTree<int>.Color.Black
                    ? ConsoleColor.White : ConsoleColor.Black,
                node.Color == RedBlackTree<int>.Color.Black
                    ? ConsoleColor.DarkGray : ConsoleColor.Red,
                node.Value.ToString() + new string(' ', 3 - node.Value.ToString().Length), x, y);
            var loc = y;

            if (node.Right is not null)
            {
                Out("══", x + 3, y);
                y = DrawTree(node.Right, x + 5, y, isShowingNILs);
            }
            else if (isShowingNILs)
            {
                Out("══", x + 3, y);
                ColorOut(ConsoleColor.Black, ConsoleColor.DarkGray, "NIL", x + 5, y);
            }

            if (node.Left is not null)
            {
                while (loc <= y)
                {
                    Out(" ║", x, loc + 1);
                    loc++;
                }
                y = DrawTree(node.Left, x, y + 2, isShowingNILs);
            }
            else if (isShowingNILs)
            {
                while (loc <= y)
                {
                    Out(" ║", x, loc + 1);
                    loc++;
                }
                ColorOut(ConsoleColor.Black, ConsoleColor.DarkGray, "NIL", x, y + 2);
                y += 2;
            }

            SetCursorPosition(0, 0);
            return y;
        }

        public void ShowTree(RedBlackTree<int> tree, bool isShowingNILs)
        {
            ClearTree();
            DrawTree(tree.Root, 3, 6, isShowingNILs);
        }

        public void ClearTree()
        {
            Out(new string(' ', WindowWidth * 64), 0, 6);
            SetCursorPosition(0, 0);
        }
    }
}
