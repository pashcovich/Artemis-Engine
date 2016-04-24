#region Using Statements

using Artemis.Engine.Utilities.Xml;

using System;
using System.Xml;

#endregion

namespace Artemis.Engine.Persistence
{
    public abstract class AbstractOptionRecord : IXmlElementIO
    {
        /// <summary>
        /// The name of this record in UserOptions.
        /// </summary>
        public string Name { get; private set; }

        private object _default;

        /// <summary>
        /// The default value for this record.
        /// </summary>
        public object Default
        {
            get { return _default; }
            internal set
            {
                if (value == null)
                {
                    _default = null;
                }
                else
                {
                    if (Validator != null && !Validator(value))
                    {
                        if (Coercer != null)
                            _default = Coercer(value);
                        else
                            throw new OptionRecordException(
                                String.Format(
                                    "OptionRecord default value '{0}' doesn't satisfy given validator.",
                                    value.ToString()
                                    )
                                );
                    }
                    else if (Coercer != null)
                    {
                        _default = Coercer(value);
                    }
                    else
                    {
                        _default = value;
                    }
                }
            }
        }

        public OptionValidator Validator { get; private set; }

        public OptionCoercer Coercer { get; private set; }

        public AbstractOptionRecord(string name, object defaultVal, OptionValidator validator, OptionCoercer coercer)
        {
            Name = name;
            Validator = validator;
            Coercer = coercer;

            Default = defaultVal;
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
