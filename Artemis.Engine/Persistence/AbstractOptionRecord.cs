using Artemis.Engine.Utilities.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Artemis.Engine.Persistence
{
    public class AbstractOptionRecord : IXmlElementIO
    {
        public string Name { get; private set; }

        public object Default { get; private set; }

        public OptionValidator Validator { get; private set; }

        public OptionCoercer Coercer { get; private set; }

        public AbstractOptionRecord(string name, object defaultVal, OptionValidator validator, OptionCoercer coercer)
        {
            Name = name;
            Default = defaultVal;
            Validator = validator;
            Coercer = coercer;

            if (validator != null && !validator(defaultVal))
            {
                if (coercer != null)
                    Default = coercer(defaultVal);
                else
                    throw new OptionRecordException(
                        String.Format(
                            "OptionRecord default value '{0}' doesn't satisfy given validator.",
                            defaultVal.ToString()
                            )
                        );
            }
        }

        public abstract object Parse(XmlElement element);

        public abstract void Write(object obj, XmlWriter writer);

        public object GetValueOrDefault(XmlElement element)
        {
            object obj;
            try
            {
                obj = Parse(element);
            }
            catch (Exception)
            {
                // If anything goes wrong just return the default value.
                return Default;
            }
            if (Validator != null && !Validator(obj))
            {
                if (Coercer != null)
                {
                    return Coercer(obj);
                }
                return Default;
            }
            return obj;
        }
    }
}
