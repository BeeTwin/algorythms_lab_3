using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace algorythms_lab_3
{
    public class RBTProcessor
    {
        private readonly IRBTViewer _rbtViewer;
        private RedBlackTree<int> _tree = new RedBlackTree<int>();
        private HashSet<int> _keys = new HashSet<int>();
        private readonly Dictionary<Command, Action<int>> _commands = new Dictionary<Command, Action<int>>();
        private bool _flag = true;
        private bool _isShowingNILs = false;

        public RBTProcessor(IRBTViewer rbtViewer)
        {
            _rbtViewer = rbtViewer;
            InitializeCommands();
        }

        private void _TestTree()
        {
            var a = new[] { 14, 20, 100, 13, 2, 3, 5, 6, 22, 30, 56, 101, 71, 4, 8, 72 };
            _tree.Insert(a);
            _keys = a.ToHashSet();
        }

        public void StartProcessing()
        {
            _rbtViewer.PrepareOutput();
            _rbtViewer.Out(Message.Welcome, null);
            if (_rbtViewer.GetInput()[0] == "y")
            {
                //AppendTree(25, 1, 999);
                _TestTree();
                _rbtViewer.ShowTree(_tree, _isShowingNILs);
            }
            _rbtViewer.Out(Message.Start, null);

            while (_flag)
            {
                var input = _rbtViewer.GetInput();
                Command command;
                var args = input.Length > 1 ? input[1] : null;
                if (!Enum.TryParse(input[0], out command))
                    _rbtViewer.Out(Message.IncorrectInput, null);
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
                        _commands[command](intArgs);
                    }
                    else
                    {
                        var output = $"{command} ";
                        if (IsConsole(command))
                            if (command == Command.Help)
                                output += "[(optional) page]";
                            else;
                        else if (IsSingular(command))
                            output += "[(optional)int]";
                        else if (command == Command.Add)
                            output += "[int] (<1000)";
                        else
                            output += "[int]";
                        _rbtViewer.Out(Message.IncorrectArguments, output);
                    }                  
                }
            }
        }

        private void InitializeCommands()
        {
            _commands[Command.Help] = (value) =>
            {
                _rbtViewer.Out(value switch
                {
                    -1 => Message.Help1,
                    1 => Message.Help1,
                    2 => Message.Help2,
                    3 => Message.Help3,
                    _ => Message.HelpPageNotFound
                }, value);
            };

            _commands[Command.Clear] = (value) =>
            {
                _tree = new RedBlackTree<int>();
                _keys = new HashSet<int>();
                _rbtViewer.ClearTree();
                _rbtViewer.Out(Message.Cleared, null);
            };

            _commands[Command.Add] = (value) =>
            {
                if (IsTreeValid(Command.Add, value))
                {
                    _keys.Add(value);
                    _tree.Insert(value);
                    _rbtViewer.ShowTree(_tree, _isShowingNILs);
                    _rbtViewer.Out(Message.AddSuccess, value);
                }
                else
                    _rbtViewer.Out(Message.AlreadyContains, value);
            };

            _commands[Command.Remove] = (value) =>
            {
                if (IsTreeValid(Command.Remove, value))
                {
                    _keys.Remove(value);
                    _tree.Remove(value);
                    _rbtViewer.ShowTree(_tree, _isShowingNILs);
                    _rbtViewer.Out(Message.RemoveSuccess, value);
                }
                else
                    _rbtViewer.Out(Message.DoesntContain, value);
            };

            _commands[Command.Find] = (value) =>
            {
                if (IsTreeValid(Command.Find, value))
                    _rbtViewer.Out(Message.FoundSuccess, _tree.Find(value));
                else
                    _rbtViewer.Out(Message.DoesntContain, value);
            };

            _commands[Command.Min] = (value) => 
            {
                if (IsTreeValid(Command.Min, value))
                    _rbtViewer.Out(Message.FoundSuccess, _tree.Find(value).Min());
                else
                    _rbtViewer.Out(Message.DoesntContain, value);
            };

            _commands[Command.Max] = (value) =>
            {
                if (IsTreeValid(Command.Max, value))
                    _rbtViewer.Out(Message.FoundSuccess, _tree.Find(value).Max());
                else
                    _rbtViewer.Out(Message.DoesntContain, value);
            };

            _commands[Command.FindNext] = (value) =>
            {
                if (IsTreeValid(Command.FindNext, value))
                {
                    var node = _tree.FindNext(value);
                    if (node is null)
                        _rbtViewer.Out(Message.NextGreatest, value);
                    else
                        _rbtViewer.Out(Message.FoundSuccess, node);
                }
                else
                    _rbtViewer.Out(Message.DoesntContain, value);
            };

            _commands[Command.FindPrev] = (value) =>
            {
                if (IsTreeValid(Command.FindPrev, value))
                {
                    var node = _tree.FindPrevious(value);
                    if (node is null)
                        _rbtViewer.Out(Message.PrevLeast, value);
                    else
                        _rbtViewer.Out(Message.FoundSuccess, node);
                }
                else
                    _rbtViewer.Out(Message.DoesntContain, value);
            };

            _commands[Command.GenerateTree] = (value) =>
            {
                if (IsTreeValid(Command.GenerateTree, value))
                {
                    AppendTree(25, 1, 999);
                    _rbtViewer.ShowTree(_tree, _isShowingNILs);
                    _rbtViewer.Out(Message.Added25, null);
                }
                else
                    _rbtViewer.Out(Message.AlreadyContains, "value");
            };

            _commands[Command.ShowNILs] = (value) =>
            {
                if (_isShowingNILs)
                    _rbtViewer.Out(Message.NILsAlready, "visible");
                else
                {
                    _isShowingNILs = true;
                    _rbtViewer.ShowTree(_tree, _isShowingNILs);
                    _rbtViewer.Out(Message.NILsNow, "visible");
                }
            };

            _commands[Command.HideNILs] = (value) =>
            {
                if (!_isShowingNILs)
                    _rbtViewer.Out(Message.NILsAlready, "hided");
                else
                {
                    _isShowingNILs = false;
                    _rbtViewer.ShowTree(_tree, _isShowingNILs);
                    _rbtViewer.Out(Message.NILsNow, "hided");
                }
            };
        }

        private void AppendTree(int count, int min, int max)
        {
            for (var i = 0; i < count; i++)
            {
                var rnd = new Random().Next(min, max + 1);
                if (_keys.Contains(rnd))
                    i--;
                else
                {
                    _keys.Add(rnd);
                    _tree.Insert(rnd);
                }
            }
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

        private bool IsInputRight(Command command, string args)
        {
            switch (command)
            {
                case Command.Clear:
                case Command.GenerateTree:
                case Command.ShowNILs:
                case Command.HideNILs:
                    return args is null;
                case Command.Help:
                case Command.Min:
                case Command.Max:
                    return args is null || int.TryParse(args, out var _);
                case Command.Add:
                    var b = int.TryParse(args, out var intArgs);
                    return b && intArgs < 1000;
                case Command.Remove:
                case Command.Find:
                case Command.FindNext:
                case Command.FindPrev:         
                    return int.TryParse(args, out var _);
                default:
                    return false;
            }
        }

        private bool IsSingular(Command command) 
            => command == Command.Min || command == Command.Max;

        private bool IsConsole(Command command) => 
            command switch
            {
                Command.Help => true, 
                Command.Clear => true,
                Command.GenerateTree => true,
                Command.HideNILs => true,
                Command.ShowNILs => true,
                _ => false
            };
    }
}
