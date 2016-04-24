#region Using Statements

using Artemis.Engine.Utilities.Dynamics;

using System.Collections.Generic;
using System.IO;
using System.Xml;

#endregion

namespace Artemis.Engine.Persistence
{
    public class OptionRecordService : DynamicPropertyCollection
    {

        private Dictionary<string, AbstractOptionRecord> OptionRecords
            = new Dictionary<string, AbstractOptionRecord>();

        internal OptionRecordService() : base() { }

        public void AddOptionRecord(AbstractOptionRecord record)
        {
            OptionRecords.Add(record.Name, record);
        }

        public void AddOptionRecord(AbstractOptionRecord record, Getter getter, Setter setter)
        {
            OptionRecords.Add(record.Name, record);
            Define(record.Name, getter, setter);
        }

        public void Read(string fileName)
        {
            var doc = new XmlDocument();

            try
            {
                doc.Load(fileName);
            }
            catch (IOException)
            {
                // If the file does not exist, then simply initialize to all
                // the default values.
                InitializeDefaults(new List<string>());
                return;
            }

            var root = doc.ChildNodes[1] as XmlElement;
            List<string> seen = new List<string>();
            foreach (var child in root.ChildNodes)
            {
                var element = child as XmlElement;
                if (element == null)
                    continue;
                var name = element.Name;

                // If we don't have a record for this option, just ignore it.
                if (!OptionRecords.ContainsKey(name))
                    continue;

                var record = OptionRecords[name];
                var obj = record.GetValueOrDefault(element);
                Set(name, obj);

                seen.Add(name);
            }

            InitializeDefaults(seen);
        }

        private void InitializeDefaults(List<string> nonDefaultOptions)
        {
            foreach (var optionRecord in OptionRecords.Values)
            {
                // If we've already seen this option, then we don't want to overwrite it with the
                // default value.
                if (nonDefaultOptions.Contains(optionRecord.Name))
                    continue;

                var obj = optionRecord.Default;
                Set(optionRecord.Name, obj);
            }
        }

        public void Write(string fileName)
        {
            using (var writer = XmlWriter.Create(fileName))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Options");

                foreach (var optionRecord in OptionRecords.Values)
                {
                    var obj = Get(optionRecord.Name);

                    optionRecord.Write(obj, writer);
                }

                writer.WriteEndElement();
            }
        }
    }
}
