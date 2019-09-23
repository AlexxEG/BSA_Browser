using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BSA_Browser.Classes
{
    public class WindowStates : IXmlSerializable
    {
        Dictionary<string, WindowState> windowStates;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowStates"/> class.
        /// </summary>
        public WindowStates()
        {
            windowStates = new Dictionary<string, WindowState>();
        }

        /// <summary>
        /// Indexer
        /// </summary>
        /// <param name="formName">The <see cref="Form"/> to find.</param>
        public WindowState this[string formName]
        {
            get
            {
                return windowStates[formName];
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
        /// Reads Xml when the <see cref="WindowStates"/> is to be deserialized 
        /// from a stream.</summary>
        /// <param name="reader">The stream from which the object will be deserialized.</param>
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            reader.ReadToFollowing("WindowState");
            do
            {
                string strWindowName = reader["name"];

                var xsrWindowState = new XmlSerializer(typeof(WindowState));
                windowStates.Add(strWindowName, (WindowState)xsrWindowState.Deserialize(reader));
            }
            while (reader.ReadToNextSibling("WindowState"));
        }

        /// <summary>
        /// Writes Xml articulating the current state of the <see cref="WindowStates"/>
        /// object.</summary>
        /// <param name="writer">The stream to which this object will be serialized.</param>
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            foreach (var kvpWindowState in windowStates)
            {
                var xsrLocationInfo = new XmlSerializer(typeof(WindowState));
                xsrLocationInfo.Serialize(writer, kvpWindowState.Value);
            }
        }

        #endregion

        /// <summary>
        /// Adds a new <see cref="WindowState"/> to handle <see cref="Form"/>.
        /// </summary>
        /// <param name="formName">The <see cref="Form"/> name.</param>
        public void Add(string formName)
        {
            this.windowStates.Add(formName, new WindowState(formName));
        }

        /// <summary>
        /// Determines whether an <see cref="WindowState"/> already exists for given <see cref="Form"/>.
        /// </summary>
        /// <param name="formName">The <see cref="Form"/> name.</param>
        public bool Contains(string formName)
        {
            return this.windowStates.ContainsKey(formName);
        }

        /// <summary>
        /// Restores <see cref="Form"/>, <see cref="ColumnHeader"/> and <see cref="SplitContainer"/> states.
        /// </summary>
        /// <param name="form">The <see cref="Form"/> to restore.</param>
        /// <param name="throwErrorIfNotFound">Throw <see cref="KeyNotFoundException"/> exception if <paramref name="form"/> is not found.</param>
        /// <param name="restoreColumns">True to restore all <see cref="ColumnHeader"/> found in <paramref name="form"/>.</param>
        /// <param name="restoreSplitContainers">True to restore all <see cref="SplitContainer"/> found in <paramref name="form"/>.</param>
        public void Restore(Form form, bool throwErrorIfNotFound, bool location = true, bool size = true, bool restoreColumns = true, bool restoreSplitContainers = true)
        {
            if (this.Contains(form.Name))
            {
                this[form.Name].RestoreForm(form,
                    location: location,
                    size: size,
                    restoreColumns: restoreColumns,
                    restoreSplitContainers: restoreSplitContainers);
            }
            else
            {
                if (throwErrorIfNotFound)
                    throw new KeyNotFoundException($"WindowState for Form '{form.Name}' not found.");
            }
        }

        /// <summary>
        /// Saves <see cref="Form"/>, <see cref="ColumnHeader"/> and <see cref="SplitContainer"/> states.
        /// </summary>
        /// <param name="form">The <see cref="Form"/> to save.</param>
        /// <param name="restoreColumns">True to save all <see cref="ColumnHeader"/> found in <paramref name="form"/>.</param>
        /// <param name="restoreSplitContainers">True to save all <see cref="SplitContainer"/> found in <paramref name="form"/>.</param>
        public void Save(Form form, bool saveColumns = true, bool saveSplitContainers = true)
        {
            if (!this.Contains(form.Name))
            {
                this.Add(form.Name);
            }

            this[form.Name].SaveForm(form, saveColumns, saveSplitContainers);
        }
    }
}
