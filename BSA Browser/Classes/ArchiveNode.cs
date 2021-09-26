using SharpBSABA2;
using System.Windows.Forms;

namespace BSA_Browser.Classes
{
    public class ArchiveNode : TreeNode
    {
        /// <summary>
        /// Gets or sets whether sub directories has been built.
        /// </summary>
        public bool Built { get; set; }
        /// <summary>
        /// Gets associated <see cref="SharpBSABA2.Archive"/>.
        /// </summary>
        public Archive Archive { get; private set; }
        /// <summary>
        /// Gets or sets all the files in the selected sub directory.
        /// </summary>
        public ArchiveEntry[] SubFiles { get; set; }

        public ArchiveNode(string text, Archive archive)
        {
            this.Text = text;
            this.Archive = archive;
        }
    }
}
