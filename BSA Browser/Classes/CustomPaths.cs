using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BSA_Browser.Classes
{
    public class CustomPaths : List<CustomPath>, IXmlSerializable
    {
        public CustomPaths()
        {
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
        /// Reads Xml when the <see cref="CustomPaths"/> is to be deserialized 
        /// from a stream.</summary>
        /// <param name="reader">The stream from which the object will be deserialized.</param>
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            bool booIsEmpty = reader.IsEmptyElement;

            reader.ReadStartElement();

            if (booIsEmpty)
                return;

            while (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "CustomPath")
            {
                XmlSerializer xsrCustomPath = new XmlSerializer(typeof(CustomPath));

                this.Add((CustomPath)xsrCustomPath.Deserialize(reader));
            }
        }

        /// <summary>
        /// Writes Xml articulating the current state of the <see cref="CustomPaths"/>
        /// object.</summary>
        /// <param name="writer">The stream to which this object will be serialized.</param>
        void IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
        {
            foreach (CustomPath kvpCustomPath in this)
            {
                XmlSerializer xsrLocationInfo = new XmlSerializer(typeof(CustomPath));

                xsrLocationInfo.Serialize(writer, kvpCustomPath);
            }
        }

        #endregion
    }
}
