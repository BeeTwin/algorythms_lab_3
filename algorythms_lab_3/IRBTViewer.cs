using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace algorythms_lab_3
{
    public interface IRBTViewer 
    {
        public string[] GetInput();

        public void PrepareOutput();

        public void Out(Message message, object args);

        public void ShowTree(RedBlackTree<int> tree, bool isShowingNILs);

        public void ClearTree();
    }

    public enum Message
    {
        Empty, Welcome, Start, Cleared,
        AddSuccess, RemoveSuccess, FoundSuccess,
        DoesntContain, AlreadyContains,     
        NextGreatest, PrevLeast,
        Added25,
        NILsNow, NILsAlready,
        IncorrectInput, IncorrectArguments,
        Help1, Help2, Help3, HelpPageNotFound
    }
}
