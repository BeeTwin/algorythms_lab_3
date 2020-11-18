using System;
using System.Collections.Generic;
using static System.Console;
using System.Linq;

namespace algorythms_lab_3
{
    public class ConsoleRBT
    {
        private RedBlackTree<int> _tree = new RedBlackTree<int>();

        private readonly Dictionary<Command, Func<int, string>> _commands = new Dictionary<Command, Func<int, string>>();

        private readonly Dictionary<Message, Action<string>> _messages = new Dictionary<Message, Action<string>>();

        private bool _flag = true;
        private bool _isShowingNILs = false;

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
                AppendTree(25, 1, 999);
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
                    if (IsInputRight(command, args) && input.Length <= 2)
                    {
                        int intArgs = -1;
                        if (args == null)
                        {
                            if (command == Command.Min || command == Command.Max)
                                intArgs = _tree.Root.Value;
                            else if (command == Command.Help || command == Command.Clear)
                                intArgs = -1;
                        }
                        else
                            intArgs = int.Parse(args);
                        OutMessage(_commands[command](intArgs));
                    }
                    else
                        _messages[Message.IncorrectArguments](
                            $"{command}{(IsConsole(command) ? command == Command.Help ? " [(optional) page]" : "" : IsSingular(command) ? " [(optional) int]" : " [int]")}");
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
                Out("════", x + 3, y);
                y = DrawTree(node.Right, x + 7, y);
            }
            else if (_isShowingNILs)
            {
                Out("════", x + 3, y);
                ColorOut(ConsoleColor.Black, ConsoleColor.DarkGray, "NIL", x + 7, y);
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
            else if (_isShowingNILs)
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
                case Command.GenerateTree:
                    return !_keys.Any();
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

        private bool IsConsole(Command command) => command == Command.Help || command == Command.Clear;

        private bool IsInputRight(Command command, string args)
        { 
            switch (command)
            {
                case Command.Clear:
                case Command.GenerateTree:
                case Command.ShowNILs:
                case Command.HideNILs:
                    return args == null;
                case Command.Help:
                case Command.Min:
                case Command.Max:
                    return args == null || int.TryParse(args, out var _);
                case Command.Add: 
                case Command.Remove: 
                case Command.Find: 
                case Command.FindNext: 
                case Command.FindPrev:
                    return int.TryParse(args, out var intArgs);
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
                switch (value)
                {
                    case -1:
                    case 1:
                        _messages[Message.Help1](null);
                        return "";
                    case 2:
                        _messages[Message.Help2](null);
                        return "";
                    case 3:
                        _messages[Message.Help3](null);
                        return "";
                    default:
                        return $"Page {value} is not found.";
                }
            };

            _commands[Command.Clear] = (value) => 
            {
                _tree = new RedBlackTree<int>();
                _keys = new HashSet<int>();
                ClearTreeSpace();
                return "Tree is now blank."; 
            };   
            
            _commands[Command.Add] = (value) => 
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

            _commands[Command.Remove] = (value) =>
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

            _commands[Command.Find] = (value) => 
            {
                if (IsTreeValid(Command.Find, value))
                    return $"({_tree.Find(value)}) found successfully.";
                else
                    return $"The tree doesn`t contain {value}.";
            };

            _commands[Command.Min] = (value) => 
            {
                if (IsTreeValid(Command.Min, value))
                    return $"({_tree.Find(value).Min()}) found successfully.";
                else
                    return $"The tree doesn`t contain {value}.";
            };

            _commands[Command.Max] = (value) => 
            {
                if (IsTreeValid(Command.Max, value))
                    return $"({_tree.Find(value).Max()}) found successfully.";
                else
                    return $"The tree doesn`t contain {value}.";
            };

            _commands[Command.FindNext] = (value) =>
            {
                if (IsTreeValid(Command.FindNext, value))
                {
                    var node = _tree.FindNext(value);
                    return node == null ? $"{value} is the greatest." : $"({node}) found successfully.";
                }
                else
                    return $"The tree doesn`t contain {value}.";
            };

            _commands[Command.FindPrev] = (value) => 
            {
                if (IsTreeValid(Command.FindPrev, value))
                {
                    var node = _tree.FindPrevious(value);
                    return node == null ? $"{value} is the least." : $"({node}) found successfully.";
                }
                else
                    return $"The tree doesn`t contain {value}.";
            };

            _commands[Command.GenerateTree] = (value) =>
            {
                if (IsTreeValid(Command.GenerateTree, value))
                {
                    AppendTree(25, 1, 999);
                    DrawTree();
                    return "Added 25 values.";
                }
                else
                    return "The tree already contains a value.";
            };

            _commands[Command.ShowNILs] = (value) =>
            {
                if (_isShowingNILs)
                    return "NIL's are already visible.";
                _isShowingNILs = true;
                DrawTree();
                return "NIL's are now visible.";
            };

            _commands[Command.HideNILs] = (value) =>
            {
                if (!_isShowingNILs)
                    return "NIL's are already hided.";
                _isShowingNILs = false;
                DrawTree();
                return "NIL's are now hided.";
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
            _messages[Message.Help1] = (message) 
                => OutMessage($"Help [(optional) page], Clear, GenerateTree, ShowNILs, HideNILs, Add [int], Remove [int] ... Use \"Help 2\" ...");
            _messages[Message.Help2] = (message)
                => OutMessage($"Find [int], Min [(optional) int], Max [(optional) int], FindNext [int], FindPrev [int] ... Use \"Help 3\" ...");
            _messages[Message.Help3] = (message)
                => OutMessage($"All ints must be less than 1000.");
            _messages[Message.Start] = (message) 
                => OutMessage($"Use \"Help\" to see commands list.");       
        }

        private void Out(string str, int x, int y)
        {
            SetCursorPosition(x, y);
            Write(str);
        }

        private void OutMessage(string str) => Out("#: " + str, 2, 1);

        private void AppendTree(int count, int min, int max)
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
            //tree commands
            Add, Remove, Find, Min, Max, FindNext, FindPrev,
            //non tree commands
            Help, Clear, GenerateTree, ShowNILs, HideNILs
        }

        private enum Message
        {
            Welcome,
            Start,
            IncorrectInput,
            IncorrectArguments,
            Help1, Help2, Help3,
            Cleared
        }
    }
}