using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace BSA_Browser.Extensions
{
    /// <summary>
    /// Source: <see href="https://stackoverflow.com/a/20081867"/>
    /// </summary>
    public static class TreeViewExtensions
    {
        public static List<string> GetExpansionState(this TreeNodeCollection nodes)
        {
            return nodes.Descendants()
                        .Where(n => n.IsExpanded)
                        .Select(n => n.FullPath)
                        .ToList();
        }

        public static void SetExpansionState(this TreeNodeCollection nodes, List<string> savedExpansionState)
        {
            foreach (var node in nodes.Descendants()
                                      .Where(n => savedExpansionState.Contains(n.FullPath)))
            {
                node.Expand();
            }
        }

        public static IEnumerable<TreeNode> Descendants(this TreeNodeCollection c)
        {
            foreach (var node in c.OfType<TreeNode>())
            {
                yield return node;

                foreach (var child in node.Nodes.Descendants())
                {
                    yield return child;
                }
            }
        }

        /// <summary>
        /// Returns the root node of the given <see cref="TreeNode"/>.
        /// </summary>
        /// <param name="node">The <see cref="TreeNode"/> to get root node from.</param>
        public static TreeNode GetRootNode(this TreeNode node)
        {
            if (node == null)
                return null;

            var rootNode = node;
            while (rootNode.Parent != null)
                rootNode = rootNode.Parent;
            return rootNode;
        }
    }
}
