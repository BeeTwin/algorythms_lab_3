using System;
using System.Collections.Generic;
using static System.Console;
using System.Text;
using System.Linq;

namespace algorythms_lab_3
{
    public class ConsoleRBT
    {
        private readonly RedBlackTree<int> _tree = new RedBlackTree<int>();

        private readonly Dictionary<Command, Func<int, string>> _commands = new Dictionary<Command, Func<int, string>>();

        private readonly Dictionary<Message, Action<string>> _messages = new Dictionary<Message, Action<string>>();

        private bool _flag = true;

        private HashSet<int> _keys = new HashSet<int>();
        
        public ConsoleRBT()
        {
            InitializeCommands();
            InitializeMessages();
        }

        public void StartConsoleProccessing()
        {
            DrawBoxes();

            _messages[Message.Welcome](null);
            Out("$: ", 2, 3);
            SetCursorPosition(5, 3);
            if (ReadLine() == "y")
            {
                GenerateTree(25, 1, 100);
                DrawTree();
            }
            ClearBoxes();
            _messages[Message.Start](null);


            while (_flag)
            {
                Out("$: ", 2, 3);
                SetCursorPosition(5, 3);
                var input = ReadLine().Split(' ');
                ClearBoxes();
                Command command;
                var args = input.Length > 1 ? input[1] : null;
                if (!Enum.TryParse(input[0], out command))
                    _messages[Message.IncorrectInput](null);
                else
                {
                    if (IsInputRight(command, args))
                    {
                        int intArgs = -1;
                        if (args == null)
                        {
                            if (command == Command.Min || command == Command.Max)
                                intArgs = _tree.Root.Value;
                            else if (command == Command.Help || command == Command.Quit)
                                intArgs = -1;
                        }
                        else
                            intArgs = int.Parse(args);
                        OutMessage(_commands[command](intArgs));
                    }
                    else
                        _messages[Message.IncorrectArguments](
                            $"{command}{(IsConsole(command) ? "" : IsSingular(command) ? " [value (you can input w/o value)]" : " [value]")}");
                }
            }
        }

        private void DrawTree()
        {
            ClearTreeSpace();
            DrawTree(_tree.Root, 3, 6);
        }

        public int DrawTree(RedBlackTree<int>.Node node, int x, int y)
        {
            ColorOut(
                node.Color == RedBlackTree<int>.Color.Black
                    ? ConsoleColor.White : ConsoleColor.Black,
                node.Color == RedBlackTree<int>.Color.Black
                    ? ConsoleColor.DarkGray : ConsoleColor.Red,
                node.Value.ToString() + new string(' ', 3 - node.Value.ToString().Length), x, y);
            var loc = y;

            if (node.Right != null)
            {
                Out("══", x + 3, y);
                y = DrawTree(node.Right, x + 5, y);
            }

            if (node.Left != null)
            {
                while (loc <= y)
                {
                    Out(" ║", x, loc + 1);
                    loc++;
                }
                y = DrawTree(node.Left, x, y + 2);
            }

            SetCursorPosition(0, 0);
            return y;
        }

        private void ClearTreeSpace()
        {
            Out(new string(' ', WindowWidth * 64), 0, 6);
            SetCursorPosition(0, 0);
        }

        private void ColorOut(ConsoleColor fgColor, ConsoleColor bgColor, string str, int x, int y)
        {
            ForegroundColor = fgColor;
            BackgroundColor = bgColor;
            Out(str, x, y);
            ResetColor();
        }

        private bool IsTreeValid(Command command, int args)
        {
            switch (command)
            {
                case Command.Add:
                    return !_keys.Contains(args);
                case Command.Remove:
                case Command.Find:
                case Command.Min:
                case Command.Max:
                case Command.FindNext:
                case Command.FindPrev:
                    return _keys.Contains(args);
                default: 
                    return false;
            }
        }

        private void ClearBoxes()
        {
            var emptyLine = new string(' ', WindowWidth - 2);
            Out(emptyLine, 1, 1);
            Out(emptyLine, 1, 3);
        }

        private bool IsSingular(Command command) => command == Command.Min || command == Command.Max;

        private bool IsConsole(Command command) => command == Command.Help || command == Command.Quit;

        private bool IsInputRight(Command command, string args)
        { 
            switch (command)
            {
                case Command.Help: 
                case Command.Quit:
                    return args == null;
                case Command.Min:
                case Command.Max:
                    return args == null || int.TryParse(args, out var _);
                case Command.Add: 
                case Command.Remove: 
                case Command.Find: 
                case Command.FindNext: 
                case Command.FindPrev:
                    return int.TryParse(args, out var _);
                default:
                    return false;
            }
        }

        private void DrawBoxes()
        {
            var line = new string('═', WindowWidth - 2);
            var emptyLine = new string(' ', WindowWidth - 2);
            string str = "";
            str += '╔'     + line      +    '╗' + "\n";
            str += '║'     + emptyLine +    '║' + "\n";
            str += '╠'     + line      +    '╣' + "\n";
            str += '║'     + emptyLine +    '║' + "\n";
            str += '╚'     + line      +    '╝' + "\n";
            Out(str, 0, 0);
        }

        private void InitializeCommands()
        {
            _commands[Command.Help]     = (value) => 
            { 
                _messages[Message.Help](null); 
                return "";
            };
            _commands[Command.Quit]     = (value) => 
            { 
                _flag = false; 
                return ""; 
            };            
            _commands[Command.Add]      = (value) => 
            {
                if (IsTreeValid(Command.Add, value))
                {
                    _keys.Add(value);
                    _tree.Insert(value);
                    DrawTree();
                    return $"{value} added successfully.";
                }
                else
                    return $"The tree already contains {value}."; 
            };
            _commands[Command.Remove]   = (value) =>
            {
                if (IsTreeValid(Command.Remove, value))
                {
                    _keys.Remove(value);
                    _tree.Remove(value);
                    DrawTree();
                    return $"{value} removed successfully.";
                }
                else
                    return $"The tree doesn`t contain {value}.";
            };
            _commands[Command.Find]     = (value) => 
            {
                if (IsTreeValid(Command.Find, value))
                    return $"({_tree.Find(value)}) found successfully.";
                else
                    return $"The tree doesn`t contain {value}.";
            };
            _commands[Command.Min]      = (value) => 
            {
                if (IsTreeValid(Command.Min, value))
                    return $"({_tree.Find(value).Min()}) found successfully.";
                else
                    return $"The tree doesn`t contain {value}.";
            };
            _commands[Command.Max]      = (value) => 
            {
                if (IsTreeValid(Command.Max, value))
                    return $"({_tree.Find(value).Max()}) found successfully.";
                else
                    return $"The tree doesn`t contain {value}.";
            } ;
            _commands[Command.FindNext] = (value) => 
            {
                if (IsTreeValid(Command.FindNext, value))
                    return $"({_tree.FindNext(value)}) found successfully.";
                else
                    return $"The tree doesn`t contain {value}.";
            };
            _commands[Command.FindPrev] = (value) => 
            {
                if (IsTreeValid(Command.FindPrev, value))
                    return $"({_tree.FindPrevious(value)}) found successfully.";
                else
                    return $"The tree doesn`t contain {value}.";
            };
        }

        private void InitializeMessages()
        {
            _messages[Message.Welcome] = (message) 
                => OutMessage($"Welcome to Console Red Black Tree Viewer. Do you want to generate a starting tree? [\"y\" to yes / any to no]");
            _messages[Message.IncorrectInput] = (message) 
                => OutMessage($"Incorrect input. Use in form: [command] [value (if needed)]. Use \"Help\" to see commands list.");
            _messages[Message.IncorrectArguments] = (message) 
                => OutMessage($"Incorrect arguments. Use in form: {message}");
            _messages[Message.Help] = (message) 
                => OutMessage($"Commands: Help, Quit, Add x, Remove x, Find x, Min *x, Max *x, FindNext x, FindPrev x // (x - int, * - optional)");
            _messages[Message.Start] = (message) 
                => OutMessage($"Use \"Help\" to see commands list.");       
        }

        private void Out(string str, int x, int y)
        {
            SetCursorPosition(x, y);
            Write(str);
        }

        private void OutMessage(string str) => Out("#: " + str, 2, 1);

        private void GenerateTree(int count, int min, int max)
        {
            for(var i = 0; i < count; i++)
            {
                var rnd = new Random().Next(min, max + 1);
                if (_keys.Contains(rnd)) //unique numbers
                    i--;
                else
                {
                    _keys.Add(rnd);
                    _tree.Insert(rnd);
                }
            }
        }

        private enum Command
        {
            Help,
            Quit,
            Add,
            Remove,
            Find,
            Min,
            Max,
            FindNext,
            FindPrev
        }

        private enum Message
        {
            Welcome, // to the club body
            Start,
            IncorrectInput,
            IncorrectArguments,
            Help
        }
    }
}