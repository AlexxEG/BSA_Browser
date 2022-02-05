using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using BSA_Browser.Extensions;

namespace BSA_Browser.Controls
{
    public class ReorderableTreeView : TreeView
    {
        private const int WM_PAINT = 0x000F;

        private int _lineAfter = -1;
        private int _lineBefore = -1;
        private bool _pauseNodeDrag;
        private TreeNode _nodeToMove;
        private object _dragData = new object();
        private bool _drewLine = false;
        private Rectangle? _drewLineRc = null;

        /// <summary>
        /// Gets or sets top allowed index of dragged <see cref="TreeNode"/>.
        /// </summary>
        [DefaultValue(0),
         EditorBrowsable(EditorBrowsableState.Always),
         Description("The top index allowed for items to be dragged to. For example, setting this to 1 will make the first node permanently pinned to top.")]
        public int TopAllowedDragIndex { get; set; } = 0;

        public ReorderableTreeView() : base()
        {
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg != WM_PAINT)
                return;

            // Check that only one line variable is set and that it's less than Nodes.Count
            if ((_lineBefore < 0 && _lineAfter < 0)
                    || !(_lineBefore >= 0 && _lineBefore < Nodes.Count)
                    && !(_lineAfter >= 0 && _lineAfter < Nodes.Count))
                return;

            // Only one of the _line variables will be higher than -1 at once
            Rectangle rc = Nodes[Math.Max(_lineBefore, _lineAfter)].Bounds;
            rc.X = 8;
            rc.Width = this.Width - 20;
            _drewLineRc = rc;
            DrawInsertionLine(rc.Left, rc.Right, _lineBefore >= 0 ? rc.Top : rc.Bottom);
        }

        private void DrawInsertionLine(int X1, int X2, int Y)
        {
            using (var g = this.CreateGraphics())
            {
                g.DrawLine(new Pen(Color.Red), X1, Y, X2 - 1, Y);
                _drewLine = true;
            }
        }

        private bool IsPointInTopHalfOfNode(Point location, TreeNode nodeToCheck)
        {
            var rc = nodeToCheck.Bounds;
            return location.Y < (rc.Top + (rc.Height / 2));
        }

        private void ResetDragIndicator()
        {
            this._lineAfter = -1;
            this._lineBefore = -1;
            if (_drewLine)
            {
                this.Invalidate();
                _drewLine = false;
            }
        }

        protected override void OnItemDrag(ItemDragEventArgs e)
        {
            if (e.Item == null || e.Button != MouseButtons.Left)
                return;

            if ((e.Item as TreeNode).Level > 0)
                return;

            if ((e.Item as TreeNode).Index < this.TopAllowedDragIndex)
                return;

            ResetDragIndicator();
            _nodeToMove = (TreeNode)e.Item;

            base.DoDragDrop(_dragData, DragDropEffects.Move);
        }

        protected override void OnDragOver(DragEventArgs drgevent)
        {
            if (_nodeToMove == null)
            {
                base.OnDragOver(drgevent);
                return;
            }

            var pt = PointToClient(new Point(drgevent.X, drgevent.Y));
            var nodeOver = GetNodeAt(pt)?.GetRootNode();
            if (nodeOver == null || nodeOver.Index < this.TopAllowedDragIndex)
            {
                drgevent.Effect = DragDropEffects.None;
                ResetDragIndicator();
                return;
            }

            if (IsPointInTopHalfOfNode(pt, nodeOver))
            {
                if (this._lineBefore != nodeOver.Index)
                {
                    this._lineBefore = nodeOver.Index;
                    this._lineAfter = -1;
                    nodeOver.EnsureVisible();
                    this.Invalidate();
                }
            }
            else
            {
                if (this._lineAfter != nodeOver.Index)
                {
                    this._lineBefore = -1;
                    this._lineAfter = nodeOver.Index;
                    nodeOver.EnsureVisible();
                    this.Invalidate();
                }
            }

            drgevent.Effect = DragDropEffects.Move;
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            _pauseNodeDrag = false;
            ResetDragIndicator();

            if (_nodeToMove == null)
            {
                _nodeToMove = null;
                base.OnDragDrop(drgevent);
                return;
            }

            var pt = PointToClient(new Point(drgevent.X, drgevent.Y));
            var nodeOver = GetNodeAt(pt);
            if (nodeOver == null || nodeOver == _nodeToMove)
            {
                _nodeToMove = null;
                return;
            }

            nodeOver = nodeOver.GetRootNode();

            int insertIndex = nodeOver.Index;
            if (!IsPointInTopHalfOfNode(pt, nodeOver))
                insertIndex++;

            if (insertIndex > _nodeToMove.Index)
                insertIndex--;

            Nodes.Remove(_nodeToMove);
            Nodes.Insert(insertIndex, _nodeToMove);

            _nodeToMove = null;

            base.OnDragDrop(drgevent);
        }

        protected override void OnDragLeave(EventArgs e)
        {
            _pauseNodeDrag = true;
        }

        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            if (_nodeToMove != null && _pauseNodeDrag)
            {
                ResetDragIndicator();
                drgevent.Effect = DragDropEffects.Move;
                _pauseNodeDrag = false;
            }
            else
            {
                base.OnDragEnter(drgevent);
            }
        }
    }
}
