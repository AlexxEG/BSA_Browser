using System.Collections.Generic;
using System.Windows.Forms;

namespace BSA_Browser.Sorting
{
    public class TreeNodeSorter : Comparer<TreeNode>
    {
        public override int Compare(TreeNode a, TreeNode b)
        {
            if (a == null)
            {
                return b == null ? 0 : -1;
            }
            else
            {
                // Sort in alphabetical order, except for "<Files>" node
                return b == null ? 1 : a.Text == "<Files>" ? 0 : a.Text.CompareTo(b.Text);
            }
        }
    }
}