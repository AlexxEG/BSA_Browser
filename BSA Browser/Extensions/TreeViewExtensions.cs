using BrightIdeasSoftware;
using System;
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
        public static TreeNode FindNodeByPath(this TreeView treeView, string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            string[] nodeNames = path.Split('/', '\\');
            TreeNodeCollection rootNodes;
            int index = 0;

            foreach (TreeNode node in treeView.Nodes)
            {
                rootNodes = node.Nodes;

                var foundNode = FindNodeInChildren(rootNodes, nodeNames[index]);

                while (true)
                {
                    if (foundNode == null)
                        break;

                    if (index == nodeNames.Length - 1)
                        return foundNode;

                    foundNode = FindNodeInChildren(foundNode.Nodes, nodeNames[++index]);
                }
            }

            return null;
        }

        private static TreeNode FindNodeInChildren(TreeNodeCollection nodes, string name)
        {
            foreach (TreeNode child in nodes)
            {
                if (string.Equals(child.Text, name, StringComparison.OrdinalIgnoreCase))
                    return child;
            }

            return null;
        }

        public static void TraverseNodes(this TreeView treeView, Action<TreeNode> action)
        {
            foreach (TreeNode node in treeView.Nodes)
            {
                TraverseNode(node, action);
            }
        }

        private static void TraverseNode(TreeNode node, Action<TreeNode> action)
        {
            action(node);

            foreach (TreeNode childNode in node.Nodes)
            {
                TraverseNode(childNode, action);
            }
        }

        public static void TraverseNodes(this TreeListView treeListView, Action<ArchiveNodeTree> action)
        {
            foreach (ArchiveNodeTree node in treeListView.Objects)
            {
                TraverseNode(node, action);
            }
        }

        private static void TraverseNode(ArchiveNodeTree node, Action<ArchiveNodeTree> action)
        {
            action(node);

            foreach (ArchiveNodeTree childNode in node.SubNodes)
            {
                TraverseNode(childNode, action);
            }
        }

        public static void TraverseNodes(this IEnumerable<ArchiveNodeTree> nodes, Action<ArchiveNodeTree> action)
        {
            foreach (ArchiveNodeTree node in nodes)
            {
                TraverseNode(node, action);
            }
        }

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
