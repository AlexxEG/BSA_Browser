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
        /// Gets the <see cref="System.Windows.Forms.Form"/> name.
        /// </summary>
        public string FormName { get; private set; }
        /// <summary>
        /// Gets or sets the saved <see cref="System.Windows.Forms.Form"/> size.
        /// </summary>
        public Size Size { get; set; }
        /// <summary>
        /// Gets or sets the saved <see cref="System.Windows.Forms.Form"/> location.
        /// </summary>
        public Point Location { get; set; }
        /// <summary>
        /// Gets or sets the saved <see cref="System.Windows.Forms.Form"/> window state.
        /// </summary>
        public FormWindowState FormWindowState { get; set; }
        /// <summary>
        /// Gets list of saved <see cref="System.Windows.Forms.ColumnHeader"/> widths.
        /// </summary>
        public Dictionary<string, int> ColumnWidths { get; set; }
        /// <summary>
        /// Gets list of saved <see cref="System.Windows.Forms.SplitContainer"/> splitter distances.
        /// </summary>
        public Dictionary<string, int> SplitterDistances { get; set; }

        /// <summary>
        /// Used by IXmlSerializable, shouldn't be used.
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
        /// Makes sure the location is within the screen's working area.
        /// </summary>
        /// <param name="form">The <see cref="System.Windows.Forms.Form"/> to restore.</param>
        /// <param name="location">The location to check.</param>
        public Point CheckLocation(Form form, Point location)
        {
            Size screenSize = Screen.FromHandle(form.Handle).WorkingArea.Size;

            if (location.X > screenSize.Width)
                location.X = screenSize.Width - form.Size.Width;

            location.X = Math.Max(0, location.X);

            if (location.Y > screenSize.Height)
                location.Y = screenSize.Height - form.Size.Height;

            location.Y = Math.Max(0, location.Y);

            return location;
        }

        /// <summary>
        /// Restores <see cref="System.Windows.Forms.Form"/>'s
        /// size, location and window state. Also restores <see cref="System.Windows.Forms.ColumnHeader"/>
        /// width and <see cref="System.Windows.Forms.SplitContainer"/> splitter distance.
        /// </summary>
        /// <param name="form">The <see cref="System.Windows.Forms.Form"/> to restore.</param>
        public void RestoreForm(Form form, bool columns = true, bool splitContainers = true)
        {
            if (!this.Size.IsEmpty)
            {
                form.Size = this.Size;
            }

            if (!this.Location.IsEmpty)
            {
                form.Location = CheckLocation(form, this.Location);
            }

            form.WindowState = this.FormWindowState;

            if (columns) RestoreColumns(form);
            if (splitContainers) RestoreSplitContainers(form);
        }

        /// <summary>
        /// Saves <see cref="System.Windows.Forms.Form"/>'s
        /// size, location and window state. Also saves <see cref="System.Windows.Forms.ColumnHeader"/>
        /// width and <see cref="System.Windows.Forms.SplitContainer"/> splitter distance.
        /// </summary>
        /// <param name="form">The <see cref="System.Windows.Forms.Form"/> to save.</param>
        public void SaveForm(Form form, bool columns = true, bool splitContainers = true)
        {
            if (!(form.WindowState == FormWindowState.Maximized))
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

            if (columns)
            {
                foreach (ColumnHeader col in GetColumns(form.Controls))
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

            if (splitContainers)
            {
                foreach (SplitContainer splitContainer in GetSplitContainers(form.Controls))
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
            bool booIsEmpty = reader.IsEmptyElement;

            if (booIsEmpty)
                return;

            this.FormName = reader.GetAttribute("name");
            this.Location = new Point(int.Parse(reader.GetAttribute("x")), int.Parse(reader.GetAttribute("y")));
            this.Size = new Size(int.Parse(reader.GetAttribute("width")), int.Parse(reader.GetAttribute("height")));
            this.FormWindowState = (FormWindowState)Enum.Parse(typeof(FormWindowState), reader.GetAttribute("windowState"), true);

            while (reader.Read() && reader.NodeType == XmlNodeType.Element)
            {
                switch (reader.LocalName)
                {
                    case "column":
                        {
                            string name = reader.GetAttribute("name");
                            int width = int.Parse(reader.GetAttribute("width"));

                            this.ColumnWidths.Add(name, width);
                            break;
                        }
                    case "splitter":
                        {
                            string name = reader.GetAttribute("name");
                            int distance = int.Parse(reader.GetAttribute("distance"));

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
            writer.WriteAttributeString("name", this.FormName);
            writer.WriteAttributeString("x", this.Location.X.ToString());
            writer.WriteAttributeString("y", this.Location.Y.ToString());
            writer.WriteAttributeString("width", this.Size.Width.ToString());
            writer.WriteAttributeString("height", this.Size.Height.ToString());
            writer.WriteAttributeString("windowState", this.FormWindowState.ToString());

            foreach (KeyValuePair<string, int> pair in this.ColumnWidths)
            {
                writer.WriteStartElement("column");
                writer.WriteAttributeString("name", pair.Key);
                writer.WriteAttributeString("width", pair.Value.ToString());
                writer.WriteEndElement();
            }

            foreach (KeyValuePair<string, int> pair in this.SplitterDistances)
            {
                writer.WriteStartElement("splitter");
                writer.WriteAttributeString("name", pair.Key);
                writer.WriteAttributeString("distance", pair.Value.ToString());
                writer.WriteEndElement();
            }
        }

        #endregion

        /// <summary>
        /// Returns all <see cref="System.Windows.Forms.ColumnHeader"/> in
        /// <see cref="System.Windows.Forms.ListView"/> by looking through given
        /// <see cref="System.Windows.Forms.Control.ControlCollection"/>.
        /// </summary>
        /// <param name="controls">The <see cref="System.Windows.Forms.Control.ControlCollection"/> to look through.</param>
        private ICollection<ColumnHeader> GetColumns(Control.ControlCollection controls)
        {
            List<ColumnHeader> columns = new List<ColumnHeader>();

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
        /// Returns all <see cref="System.Windows.Forms.SplitContainer"/> in given
        /// <see cref="System.Windows.Forms.Control.ControlCollection"/>.
        /// </summary>
        /// <param name="controls">The <see cref="System.Windows.Forms.Control.ControlCollection"/> to look through.</param>
        private ICollection<SplitContainer> GetSplitContainers(Control.ControlCollection controls)
        {
            List<SplitContainer> columns = new List<SplitContainer>();

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
        /// Restores all <see cref="System.Windows.Forms.ColumnHeader.Width"/> in
        /// <see cref="System.Windows.Forms.Form"/>.
        /// </summary>
        /// <param name="form">The <see cref="System.Windows.Forms.Form"/> to restore columns from.</param>
        private void RestoreColumns(Form form)
        {
            foreach (ColumnHeader col in GetColumns(form.Controls))
            {
                string key = string.Format("{0} - {1}", col.ListView.Name, col.DisplayIndex);

                if (this.ColumnWidths.ContainsKey(key))
                {
                    col.Width = this.ColumnWidths[key];
                }
            }
        }

        /// <summary>
        /// Restores all <see cref="System.Windows.Forms.SplitContainer.SplitterDistance"/> in
        /// <see cref="System.Windows.Forms.Form"/>.
        /// </summary>
        /// <param name="form">The <see cref="System.Windows.Forms.Form"/> to restore columns from.</param>
        private void RestoreSplitContainers(Form form)
        {
            foreach (SplitContainer splitContainer in GetSplitContainers(form.Controls))
            {
                string key = splitContainer.Name;

                if (this.SplitterDistances.ContainsKey(key))
                {
                    splitContainer.SplitterDistance = this.SplitterDistances[key];
                }
            }
        }
    }
}