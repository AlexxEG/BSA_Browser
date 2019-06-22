using SharpBSABA2;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BSA_Browser.Classes
{
    public class ArchiveNode : TreeNode
    {
        public Archive Archive { get; private set; }
        /// <summary>
        /// Gets all files in the archive.
        /// </summary>
        public List<ArchiveEntry> AllFiles { get; set; }
        /// <summary>
        /// Gets all the files to be shown currently.
        /// </summary>
        public ArchiveEntry[] Files { get; set; }

        public ArchiveNode(string text, Archive archive)
        {
            this.Text = text;
            this.Archive = archive;
        }
    }
}
