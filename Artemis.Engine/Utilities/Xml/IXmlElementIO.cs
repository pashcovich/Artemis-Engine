using System.Xml;

namespace Artemis.Engine.Utilities.Xml
{
    /// <summary>
    /// Implemented by classes that can parse XmlElements as objects, and write objects
    /// to an XmlWriter.
    /// </summary>
    public interface IXmlElementIO
    {
        /// <summary>
        /// Parse an XmlElement as an object.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        object Parse(XmlElement element);

        /// <summary>
        /// Write an object to an XmlWriter.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="writer"></param>
        void Write(object obj, XmlWriter writer);
    }
}
