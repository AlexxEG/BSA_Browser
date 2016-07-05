using System.IO;
using System.Windows.Forms;

namespace BSA_Browser
{
    public class BSATreeNode : TreeNode
    {
        public BSAFileEntry[] AllFiles { get; set; }
        public BSAFileEntry[] Files { get; set; }
        public BinaryReader BinaryReader { get; set; }

        public bool Compressed { get; set; }
        public bool ContainsFileNameBlobs { get; set; }

        public BSATreeNode(string text)
        {
            this.Text = text;
        }
    }
}