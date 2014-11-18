using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BSA_Browser.Classes
{
    public class QuickExtractPath : IXmlSerializable
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public bool UseFolderPath { get; set; }

        /// <summary>
        /// Used by IXmlSerializable.
        /// </summary>
        public QuickExtractPath()
        {
        }

        public QuickExtractPath(string name, string path, bool useFolderPath)
        {
            this.Name = name;
            this.Path = path;
            this.UseFolderPath = useFolderPath;
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
        /// Reads Xml when the <see cref="QuickExtractPath"/> is to be deserialized 
        /// from a stream.</summary>
        /// <param name="reader">The stream from which the object will be deserialized.</param>
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            this.Name = reader.GetAttribute("name");
            this.Path = reader.GetAttribute("path");
            this.UseFolderPath = bool.Parse(reader.GetAttribute("useFolderPath"));

            reader.Read();
        }

        /// <summary>
        /// Writes Xml articulating the current state of the <see cref="QuickExtractPath"/>
        /// object.</summary>
        /// <param name="writer">The stream to which this object will be serialized.</param>
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("name", this.Name);
            writer.WriteAttributeString("path", this.Path);
            writer.WriteAttributeString("useFolderPath", this.UseFolderPath.ToString());
        }

        #endregion
    }
}
