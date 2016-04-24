using Artemis.Engine.Utilities.Serialize;
using System.Xml;

namespace Artemis.Engine.Persistence
{
    public class SimpleOptionRecord : AbstractOptionRecord
    {
        public AbstractStringSerializer Serializer { get; private set; }

        public SimpleOptionRecord(
            string name, object defaultVal, OptionValidator validator,
            OptionCoercer coercer, AbstractStringSerializer serializer)
            : base(name, defaultVal, validator, coercer)
        {
            Serializer = serializer;
        }

        public override object Parse(XmlElement element)
        {
            return Serializer.FromString(element.InnerText);
        }

        public override void Write(object obj, XmlWriter writer)
        {
            writer.WriteElementString(Name, Serializer.ToString(obj));
        }
    }
}
