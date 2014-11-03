using System.Collections.Generic;
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
        /// <param name="form">The <see cref="System.Windows.Forms.Form"/> to find.</param>
        public WindowState this[string form]
        {
            get
            {
                return windowStates[form];
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
            bool booIsEmpty = reader.IsEmptyElement;

            reader.ReadStartElement();

            if (booIsEmpty)
                return;

            while (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "WindowState")
            {
                string strWindowName = reader["name"];

                XmlSerializer xsrWindowState = new XmlSerializer(typeof(WindowState));
                windowStates.Add(strWindowName, (WindowState)xsrWindowState.Deserialize(reader));

                windowStates.ToString();
            }
        }

        /// <summary>
        /// Writes Xml articulating the current state of the <see cref="WindowStates"/>
        /// object.</summary>
        /// <param name="writer">The stream to which this object will be serialized.</param>
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            foreach (KeyValuePair<string, WindowState> kvpWindowState in windowStates)
            {
                XmlSerializer xsrLocationInfo = new XmlSerializer(typeof(WindowState));
                xsrLocationInfo.Serialize(writer, kvpWindowState.Value);
            }
        }

        #endregion

        /// <summary>
        /// Adds a new <see cref="WindowState"/> to handle <see cref="System.Windows.Forms.Form"/>.
        /// </summary>
        /// <param name="form">The <see cref="System.Windows.Forms.Form"/> name.</param>
        public void Add(string form)
        {
            this.windowStates.Add(form, new WindowState(form));
        }

        /// <summary>
        /// Determines whether an <see cref="WindowState"/> already exists for given <see cref="System.Windows.Forms.Form"/>.
        /// </summary>
        /// <param name="form">The <see cref="System.Windows.Forms.Form"/> name.</param>
        public bool Contains(string form)
        {
            return this.windowStates.ContainsKey(form);
        }
    }
}
