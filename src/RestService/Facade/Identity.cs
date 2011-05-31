using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace TellagoStudios.Hermes.RestService.Facade
{
    [TypeConverter(typeof(IdentityConverter))]
    public struct Identity : IComparable<Identity>, IXmlSerializable
    {
        #region Inner class

        public class IdentityConverter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                return sourceType == typeof(string);
            }

            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                return destinationType == typeof(string);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
            {
                if (value is string)
                {
                    return new Identity(value as string);
                }

                throw new InvalidCastException();
            }

            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                var id = (Identity)value;

                if (destinationType == typeof(string))
                {
                    return id.ToString();
                }

                throw new InvalidCastException();
            }
        }

        #endregion
        
        string _data;

        public Identity(string data)
        {
            _data = data;
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public bool IsEmpty
        {
            get { return string.IsNullOrWhiteSpace(_data); }
        }

        public static explicit operator  string(Identity from)
        {
            return from._data.ToString();
        }

        public static explicit operator Identity(string from)
        {
            var id = new Identity
                         {
                             _data = string.IsNullOrWhiteSpace(from) ? null : from
                         };
            return id;
        }

        static public Identity Random()
        {
            return new Identity { _data = Guid.NewGuid().ToString("N") };
        }

        static public Identity Empty
        {
            get { return new Identity {_data = null}; }
        }



        #region Operators

        public static bool operator ==(Identity idOne, Identity idTwo)
        {
            return Compare(idOne._data, idTwo._data) == 0;
        }

        public static bool operator !=(Identity idOne, Identity idTwo)
        {
            return Compare(idOne._data, idTwo._data) != 0;
        }

        public static bool operator <(Identity idOne, Identity idTwo)
        {
            return Compare(idOne._data, idTwo._data) < 0;
        }

        public static bool operator <=(Identity idOne, Identity idTwo)
        {
            return Compare(idOne._data, idTwo._data) <= 0;
        }

        public static bool operator >(Identity idOne, Identity idTwo)
        {
            return Compare(idOne._data, idTwo._data) > 0;
        }

        public static bool operator >=(Identity idOne, Identity idTwo)
        {
            return Compare(idOne._data, idTwo._data) >= 0;
        }

        #endregion

        #region Overrides

        public override bool Equals(object obj)
        {
            if (obj is Identity)
            {
                return this == (Identity)obj;
            }
            return false;
        }

        public override string ToString()
        {
            return _data;
        }

        public override int GetHashCode()
        {
            if (_data == null) return 0;
            return _data.GetHashCode();
        }

        #endregion

        #region ICompare

        public int CompareTo(Identity other)
        {
            return Compare(_data, other._data);
        }

        #endregion
        private static int Compare(string left, string right)
        {
            return string.Compare(left, right);
        }


        public void ReadXml(System.Xml.XmlReader reader)
        {
            _data = reader.ReadElementContentAsString();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteValue(_data);
        }
    }
}