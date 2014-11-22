/*
 * This source file is subject to the Microsoft Public License (Ms-PL), which is 
 * available at http://www.opensource.org/licenses/ms-pl.html
 * 
 *  Author: Shawn Smith
 *   Email: mailto:gfreeman14094@users.sourceforge.net
 * Created: February 23, 2009
 * 
 * This code was originally based upon the ListViewEx class by mav.northwind from the 
 * following CodeProject article:
 * 
 *		Manual reordering of items inside a ListView:
 *		http://www.codeproject.com/KB/list/LVCustomReordering.aspx
 * 
 * */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace L0ki.Controls
{
    /// <summary>
    /// An extended version of the standard ListView designed to allow the user
    /// to reorder items in the control by drag and drop.
    /// </summary>
    /// <see cref="ListView"/>
    class ReordableItemListView : ListView
    {
        #region Constants

        // from WinUser.h
        private const int WM_PAINT = 0x000F;

        #endregion

        #region Constructor

        public ReordableItemListView()
            : base()
        {
            // Reduce flicker
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            // This listview was designed for a details view with gridlines enabled
            base.AllowDrop = true;
            this.FullRowSelect = true;
            this.ShowGroups = false;
            this.Sorting = SortOrder.None;
            this.View = View.Details;
        }

        #endregion

        #region Overridden WndProc

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            // We have to take this way (instead of overriding OnPaint()) because the ListView is 
            // just a wrapper around the common control ListView and unfortunately does not call the 
            // OnPaint overrides.
            if (m.Msg == WM_PAINT)
            {
                if (LineBefore >= 0 && LineBefore < Items.Count)
                {
                    Rectangle rc = Items[LineBefore].GetBounds(ItemBoundsPortion.Entire);
                    DrawInsertionLine(rc.Left, rc.Right, rc.Top);
                }
                if (LineAfter >= 0 && LineBefore < Items.Count)
                {
                    Rectangle rc = Items[LineAfter].GetBounds(ItemBoundsPortion.Entire);
                    DrawInsertionLine(rc.Left, rc.Right, rc.Bottom);
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Draw a line with insertion marks at each end
        /// </summary>
        /// <param name="X1">Starting position (X) of the line</param>
        /// <param name="X2">Ending position (X) of the line</param>
        /// <param name="Y">Position (Y) of the line</param>
        private void DrawInsertionLine(int X1, int X2, int Y)
        {
            using (Graphics g = this.CreateGraphics())
            {
                g.DrawLine(new Pen(Color.Red), X1, Y, X2 - 1, Y);

                Point[] leftTriangle = new Point[3] {
					new Point(X1,      Y-4),
					new Point(X1 + 7,  Y),
					new Point(X1,      Y+4)
				};
                Point[] rightTriangle = new Point[3] {
					new Point(X2,     Y-4),
					new Point(X2 - 8, Y),
					new Point(X2,     Y+4)
				};

                Brush br = new SolidBrush(Color.Red);
                g.FillPolygon(br, leftTriangle);
                g.FillPolygon(br, rightTriangle);
            }
        }

        /// <summary>
        /// Determines if the specified Point is in the top half of the specified ListViewItem
        /// </summary>
        /// <param name="location">The point to check</param>
        /// <param name="itemToCheck">The ListViewItem whose bounds are to be checked</param>
        /// <returns>true if location is in the top half of itemToCheck, otherwise false</returns>
        private bool IsPointInTopHalfOfItem(Point location, ListViewItem itemToCheck)
        {
            Point pt = this.PointToClient(location);
            Rectangle rc = itemToCheck.GetBounds(ItemBoundsPortion.Entire);
            return (pt.Y < (rc.Top + (rc.Height / 2)));
        }

        /// <summary>
        /// Wrapper function to get the ListViewItem at the specified Point
        /// </summary>
        /// <param name="location">The location to retrieve the ListViewItem from</param>
        /// <returns>
        /// The ListViewItem at the specified point, or the last item in the control if the
        /// point is lower than it
        /// </returns>
        private ListViewItem GetItemAtPoint(Point location)
        {
            Point pt = this.PointToClient(location);
            int lastItemBottom = Math.Min(pt.Y, this.Items[this.Items.Count - 1].GetBounds(ItemBoundsPortion.Entire).Bottom - 1);
            return this.GetItemAt(0, lastItemBottom);
        }

        /// <summary>
        /// Removes the drag indicator from being displayed
        /// </summary>
        private void ResetDragIndicator()
        {
            this.LineAfter = -1;
            this.LineBefore = -1;
            this.Invalidate();
        }

        /// <summary>
        /// Clears the SelectedItems that are being dragged
        /// </summary>
        private void ResetDragItems()
        {
            this._ItemsToMove.Clear();
            this.SelectedItems.Clear();
        }

        #endregion

        #region Properties
        private bool allowDrop = true;
        public override bool AllowDrop
        {
            get { return allowDrop; }
            set { allowDrop = value; }
        }

        #endregion

        #region Fields

        private int LineAfter = -1;
        private int LineBefore = -1;

        private List<ListViewItem> _ItemsToMove = new List<ListViewItem>();
        private bool _PauseItemDrag;
        private object _DragKey = new object();

        #endregion

        #region Overridden Events

        protected override void OnItemChecked(ItemCheckedEventArgs e)
        {
            if (_ItemsToMove.Count > 0) return;
            base.OnItemChecked(e);
        }

        protected override void OnItemDrag(ItemDragEventArgs e)
        {
            if ((this.SelectedItems.Count == 0) || (e.Button != MouseButtons.Left) || !allowDrop)
                return;

            ResetDragIndicator();

            this._ItemsToMove.Clear();
            for (int index = 0; index < this.SelectedItems.Count; index++)
                this._ItemsToMove.Add(this.SelectedItems[index]);

            base.DoDragDrop(this._DragKey, DragDropEffects.Move);
        }

        protected override void OnDragOver(DragEventArgs drgevent)
        {
            if (_ItemsToMove.Count == 0)
            {
                base.OnDragOver(drgevent);
                return;
            }

            Point pt = new Point(drgevent.X, drgevent.Y);
            ListViewItem itemOver = GetItemAtPoint(pt);
            if (itemOver == null)
            {
                drgevent.Effect = DragDropEffects.None;
                ResetDragIndicator();
                return;
            }

            if (IsPointInTopHalfOfItem(pt, itemOver))
            {
                if (this.LineBefore != itemOver.Index)
                {
                    this.LineBefore = itemOver.Index;
                    this.LineAfter = -1;
                    itemOver.EnsureVisible();
                    this.Invalidate();
                }
            }
            else
            {
                if (this.LineAfter != itemOver.Index)
                {
                    this.LineBefore = -1;
                    this.LineAfter = itemOver.Index;
                    itemOver.EnsureVisible();
                    this.Invalidate();
                }
            }

            drgevent.Effect = DragDropEffects.Move;
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            _PauseItemDrag = false;
            ResetDragIndicator();

            if (_ItemsToMove.Count == 0)
            {
                ResetDragItems();
                base.OnDragDrop(drgevent);
                return;
            }

            Point pt = new Point(drgevent.X, drgevent.Y);
            ListViewItem itemOver = GetItemAtPoint(pt);
            if ((itemOver == null) || (itemOver == _ItemsToMove[0]))
            {
                ResetDragItems();
                return;
            }

            int insertIndex;
            if (IsPointInTopHalfOfItem(pt, itemOver))
                insertIndex = itemOver.Index;
            else
                insertIndex = itemOver.Index + 1;

            Int32 intSelectionStartIndex = this.Items.IndexOf(_ItemsToMove[0]);
            Int32 intSelectionEndIndex = this.Items.IndexOf(_ItemsToMove[_ItemsToMove.Count - 1]);
            //if we are dragging to a point in the selection being dragged, don't bother as the order won't change
            if ((insertIndex >= intSelectionStartIndex) && (insertIndex <= intSelectionEndIndex))
                return;
            //if we are inserting at a point after the selection being dragged, offset the insert index to account
            // the the seleciton having been removed
            if (insertIndex > intSelectionEndIndex)
                insertIndex -= _ItemsToMove.Count;

            // Remove old items
            for (int index = 0; index < this._ItemsToMove.Count; index++)
                this.Items.Remove(this._ItemsToMove[index]);

            // Insert new items
            for (int index = 0; index < this._ItemsToMove.Count; index++)
                this.Items.Insert(Math.Min(insertIndex + index, this.Items.Count), _ItemsToMove[index]);

            ResetDragItems();

            base.OnDragDrop(drgevent);
        }

        protected override void OnDragLeave(EventArgs e)
        {
            _PauseItemDrag = true;
        }

        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            if ((this._ItemsToMove.Count > 0) && _PauseItemDrag)
            {
                ResetDragIndicator();
                drgevent.Effect = DragDropEffects.Move;
                _PauseItemDrag = false;
            }
            else
            {
                base.OnDragEnter(drgevent);
                return;
            }
        }

        #endregion
    }
}