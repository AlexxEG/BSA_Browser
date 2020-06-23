using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BSA_Browser.Classes
{
    public class WindowState : IXmlSerializable
    {
        /// <summary>
        /// Gets the <see cref="Form"/> name.
        /// </summary>
        public string FormName { get; private set; }
        /// <summary>
        /// Gets or sets the saved <see cref="Form"/> size.
        /// </summary>
        public Size Size { get; set; }
        /// <summary>
        /// Gets or sets the saved <see cref="Form"/> location.
        /// </summary>
        public Point? Location { get; set; } = null;
        /// <summary>
        /// Gets or sets the saved <see cref="Form"/> window state.
        /// </summary>
        public FormWindowState FormWindowState { get; set; }
        /// <summary>
        /// Gets list of saved <see cref="ColumnHeader"/> widths.
        /// </summary>
        public Dictionary<string, int> ColumnWidths { get; set; }
        /// <summary>
        /// Gets list of saved <see cref="SplitContainer"/> splitter distances.
        /// </summary>
        public Dictionary<string, int> SplitterDistances { get; set; }

        /// <summary>
        /// Class to hold all attributes for easy editing.
        /// </summary>
        private class Attributes
        {
            public const string Name = "name";
            public const string X = "x";
            public const string Y = "y";
            public const string Height = "height";
            public const string Width = "width";
            public const string WindowState = "windowState";

            public const string Column = "column";
            public const string ColumnName = "name";
            public const string ColumnWidth = "width";

            public const string Splitter = "splitter";
            public const string SplitterName = "name";
            public const string SplitterDistance = "distance";
        }

        /// <summary>
        /// Used by <see cref="IXmlSerializable"/>, shouldn't be used.
        /// </summary>
        private WindowState()
        {
            this.ColumnWidths = new Dictionary<string, int>();
            this.SplitterDistances = new Dictionary<string, int>();
            this.Location = Point.Empty;
            this.Size = Size.Empty;
            this.FormWindowState = FormWindowState.Normal;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowState"/> class.
        /// </summary>
        public WindowState(string formName)
        {
            this.FormName = formName;
            this.Size = Size.Empty;
            this.Location = Point.Empty;
            this.FormWindowState = FormWindowState.Normal;
            this.ColumnWidths = new Dictionary<string, int>();
            this.SplitterDistances = new Dictionary<string, int>();
        }

        /// <summary>
        /// Restores <see cref="Form"/>'s
        /// size, location and window state. Also restores <see cref="ColumnHeader"/>
        /// width and <see cref="SplitContainer"/> splitter distance.
        /// </summary>
        /// <param name="form">The <see cref="Form"/> to restore.</param>
        public void RestoreForm(Form form,
                                bool location = true,
                                bool size = true,
                                bool restoreColumns = true,
                                bool restoreSplitContainers = true)
        {
            // Check if Location has value, and if not let Windows chose starting location
            if (location && this.Location.HasValue)
            {
                form.Location = SanitizeLocation(this.Size, this.Location.Value);
            }

            if (size && !this.Size.IsEmpty)
            {
                form.Size = this.Size;
            }

            form.WindowState = this.FormWindowState;

            if (restoreColumns)
                this.RestoreColumns(form);

            if (restoreSplitContainers)
                this.RestoreSplitContainers(form);
        }

        /// <summary>
        /// Saves <see cref="Form"/>'s
        /// size, location and window state. Also saves <see cref="ColumnHeader"/>
        /// width and <see cref="SplitContainer"/> splitter distance.
        /// </summary>
        /// <param name="form">The <see cref="Form"/> to save.</param>
        public void SaveForm(Form form,
                             bool saveColumns = true,
                             bool saveSplitContainers = true)
        {
            if (form.WindowState == FormWindowState.Normal)
            {
                this.Location = form.Location;
                this.Size = form.Size;
            }
            else
            {
                this.Location = form.RestoreBounds.Location;
                this.Size = form.RestoreBounds.Size;
            }

            this.FormWindowState = form.WindowState;

            if (saveColumns)
                this.SaveColumns(form);

            if (saveSplitContainers)
                this.SaveSplitContainers(form);
        }

        #region IXmlSerializable Members

        /// <summary>
        /// Method that returns schema information.  Not implemented.
        /// </summary>
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Reads Xml when the <see cref="WindowState"/> is to be deserialized 
        /// from a stream.</summary>
        /// <param name="reader">The stream from which the object will be deserialized.</param>
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            this.FormName = reader.GetAttribute(Attributes.Name);
            this.Location = new Point(
                int.Parse(reader.GetAttribute(Attributes.X)),
                int.Parse(reader.GetAttribute(Attributes.Y))
            );
            this.Size = new Size(
                int.Parse(reader.GetAttribute(Attributes.Width)),
                int.Parse(reader.GetAttribute(Attributes.Height))
            );
            this.FormWindowState = (FormWindowState)Enum.Parse(typeof(FormWindowState), reader.GetAttribute(Attributes.WindowState), true);

            if (reader.IsEmptyElement)
                return;

            var subtree = reader.ReadSubtree();

            while (subtree.Read())
            {
                switch (reader.LocalName)
                {
                    case Attributes.Column:
                        {
                            string name = reader.GetAttribute(Attributes.ColumnName);
                            int width = int.Parse(reader.GetAttribute(Attributes.ColumnWidth));

                            this.ColumnWidths.Add(name, width);
                            break;
                        }
                    case Attributes.Splitter:
                        {
                            string name = reader.GetAttribute(Attributes.SplitterName);
                            int distance = int.Parse(reader.GetAttribute(Attributes.SplitterDistance));

                            this.SplitterDistances.Add(name, distance);
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// Writes Xml articulating the current state of the <see cref="WindowState"/>
        /// object.</summary>
        /// <param name="writer">The stream to which this object will be serialized.</param>
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString(Attributes.Name, this.FormName);
            writer.WriteAttributeString(Attributes.X, this.Location.Value.X.ToString());
            writer.WriteAttributeString(Attributes.Y, this.Location.Value.Y.ToString());
            writer.WriteAttributeString(Attributes.Width, this.Size.Width.ToString());
            writer.WriteAttributeString(Attributes.Height, this.Size.Height.ToString());
            writer.WriteAttributeString(Attributes.WindowState, this.FormWindowState.ToString());

            foreach (var pair in this.ColumnWidths)
            {
                writer.WriteStartElement(Attributes.Column);
                writer.WriteAttributeString(Attributes.ColumnName, pair.Key);
                writer.WriteAttributeString(Attributes.ColumnWidth, pair.Value.ToString());
                writer.WriteEndElement();
            }

            foreach (var pair in this.SplitterDistances)
            {
                writer.WriteStartElement(Attributes.Splitter);
                writer.WriteAttributeString(Attributes.SplitterName, pair.Key);
                writer.WriteAttributeString(Attributes.SplitterDistance, pair.Value.ToString());
                writer.WriteEndElement();
            }
        }

        #endregion

        /// <summary>
        /// Returns all <see cref="ColumnHeader"/> in
        /// <see cref="ListView"/> by looking through given
        /// <see cref="Control.ControlCollection"/>.
        /// </summary>
        /// <param name="controls">The <see cref="Control.ControlCollection"/> to look through.</param>
        private ICollection<ColumnHeader> GetColumns(Control.ControlCollection controls)
        {
            var columns = new List<ColumnHeader>();

            foreach (Control c in controls)
            {
                if (c is ListView)
                {
                    foreach (ColumnHeader col in (c as ListView).Columns)
                    {
                        columns.Add(col);
                    }
                }
                else if (c.Controls.Count > 0)
                {
                    columns.AddRange(GetColumns(c.Controls));
                }
            }

            return columns.ToArray();
        }

        /// <summary>
        /// Returns all <see cref="SplitContainer"/> in given
        /// <see cref="Control.ControlCollection"/>.
        /// </summary>
        /// <param name="controls">The <see cref="Control.ControlCollection"/> to look through.</param>
        private ICollection<SplitContainer> GetSplitContainers(Control.ControlCollection controls)
        {
            var columns = new List<SplitContainer>();

            foreach (Control c in controls)
            {
                if (c is SplitContainer)
                {
                    columns.Add((SplitContainer)c);
                }
                else if (c.Controls.Count > 0)
                {
                    columns.AddRange(GetSplitContainers(c.Controls));
                }
            }

            return columns.ToArray();
        }

        /// <summary>
        /// Makes sure <paramref name="location"/> is always within the viewable desktop area.
        /// </summary>
        private Point SanitizeLocation(Size formSize, Point location)
        {
            // Ensure a grabbable area of the title bar is visible
            int minX = (formSize.Width * -1) + 200;
            int minY = 0;
            return new Point(
                Math.Max(minX, location.X),
                Math.Max(minY, location.Y));
        }

        /// <summary>
        /// Restores all <see cref="ColumnHeader.Width"/> in
        /// <see cref="Form"/>.
        /// </summary>
        /// <param name="form">The <see cref="Form"/> to restore columns from.</param>
        private void RestoreColumns(Form form)
        {
            foreach (var col in GetColumns(form.Controls))
            {
                string key = string.Format("{0} - {1}", col.ListView.Name, col.DisplayIndex);

                if (this.ColumnWidths.ContainsKey(key))
                {
                    col.Width = this.ColumnWidths[key];
                }
            }
        }

        /// <summary>
        /// Restores all <see cref="SplitContainer.SplitterDistance"/> in
        /// <see cref="Form"/>.
        /// </summary>
        /// <param name="form">The <see cref="Form"/> to restore columns from.</param>
        private void RestoreSplitContainers(Form form)
        {
            foreach (var splitContainer in GetSplitContainers(form.Controls))
            {
                string key = splitContainer.Name;

                if (this.SplitterDistances.ContainsKey(key))
                {
                    int saved = this.SplitterDistances[key];
                    int distance = Math.Max(splitContainer.Panel1MinSize, Math.Min(saved, splitContainer.Width - splitContainer.Panel2MinSize));

                    splitContainer.SplitterDistance = distance;
                }
            }
        }

        /// <summary>
        /// Saves any <see cref="ColumnHeader"/>'s width found in <see cref="Form"/>.
        /// </summary>
        /// <param name="form"></param>
        private void SaveColumns(Form form)
        {
            foreach (var col in GetColumns(form.Controls))
            {
                // Skip column if Name is null or empty, since it can't be identified.
                if (string.IsNullOrEmpty(col.ListView.Name))
                    continue;

                string key = string.Format("{0} - {1}", col.ListView.Name, col.DisplayIndex);

                if (this.ColumnWidths.ContainsKey(key))
                {
                    this.ColumnWidths[key] = col.Width;
                }
                else
                {
                    this.ColumnWidths.Add(key, col.Width);
                }
            }
        }

        /// <summary>
        /// Saves any <see cref="SplitContainer"/>'s distance found in <see cref="Form"/>.
        /// </summary>
        /// <param name="form"></param>
        private void SaveSplitContainers(Form form)
        {
            foreach (var splitContainer in GetSplitContainers(form.Controls))
            {
                string key = splitContainer.Name;

                // Skip container if Name is null or empty, since it can't be identified.
                if (string.IsNullOrEmpty(key))
                    continue;

                if (this.SplitterDistances.ContainsKey(key))
                {
                    this.SplitterDistances[key] = splitContainer.SplitterDistance;
                }
                else
                {
                    this.SplitterDistances.Add(key, splitContainer.SplitterDistance);
                }
            }
        }
    }
}