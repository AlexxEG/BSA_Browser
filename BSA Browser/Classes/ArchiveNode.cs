using System.Windows.Forms;
using BSA_Browser.Sorting;
using SharpBSABA2;

namespace BSA_Browser.Classes
{
    public class ArchiveNode : TreeNode
    {
        /// <summary>
        /// Gets or sets whether sub directories has been built.
        /// </summary>
        public bool Built { get; set; }
        /// <summary>
        /// Gets or sets whether <see cref="Archive"/> has been loaded.
        /// </summary>
        public bool Loaded { get; set; } = false;
        /// <summary>
        /// Gets the file path to <see cref="Archive"/>.
        /// </summary>
        public string FilePath { get; private set; }
        /// <summary>
        /// Gets associated <see cref="SharpBSABA2.Archive"/>.
        /// </summary>
        public Archive Archive { get; private set; }
        /// <summary>
        /// Gets or sets all the files in the selected sub directory.
        /// </summary>
        public ArchiveEntry[] SubFiles { get; set; }

        public SortingConfig? SortingConfig { get; set; } = null;

        public ArchiveNode(string text, Archive archive)
        {
            this.Text = text;
            this.Archive = archive;
            this.FilePath = archive?.FullPath;
            this.Loaded = archive != null;
        }

        public ArchiveNode(string text, string filePath)
        {
            this.Text = text;
            this.FilePath = filePath;
        }
    }
}
