using System.Windows.Forms;
using SharpBSABA2;

namespace BSA_Browser.Classes
{
    public class ArchiveNode : TreeNode
    {
        public Archive Archive { get; private set; }
        public ArchiveEntry[] AllFiles { get; set; }
        public ArchiveEntry[] Files { get; set; }

        public ArchiveNode(string text, Archive archive)
        {
            this.Text = text;
            this.Archive = archive;
        }
    }
}
