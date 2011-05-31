using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace TellagoStudios.Hermes.Business.Model
{
    [TypeConverter(typeof(IdentityConverter))]
    public struct Identity : IComparable<Identity>, IEnumerable<byte>
    {
        #region Inner class

        public class IdentityConverter : TypeConverter
        {
            public override bool  CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
 	             return sourceType == typeof(string) || sourceType==typeof(byte[]);
            }

            public override bool  CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                return destinationType == typeof (string) || destinationType == typeof (IEnumerable<byte>);
            }

            public override object  ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
            {
                if (value is string)
                {
                    return new Identity(value as string);
                }

                if (value is IEnumerable<byte>)
                {
                    return new Identity(value as IEnumerable<byte>);
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

                if (destinationType == typeof(byte[]))
                {
                    return (byte[])id;
                }

                throw new InvalidCastException();
            }
        }

        #endregion

        private const string InvalidCharacter = "The character '{0}' at index {1} is invalid.";

        List<byte> _data;
        int? _hashCode;

        static public Identity Empty { get { return new Identity { _data = new List<byte>(), _hashCode = 0 }; } }

        static public Identity Random(int length = 16)
        {
            if (length <= 0) return Empty;

            IEnumerable<byte> bytes = Guid.NewGuid().ToByteArray();

            var count = (length / 16) ;
            if ((length % 16) > 0) count++; 
            
            for (int i = 0; i < count; i++)
            {
                bytes = bytes.Concat(Guid.NewGuid().ToByteArray());
            }

            return new Identity {_data = new List<byte>(bytes.Take(length))};
        }

        static public Identity Random(Func<IEnumerable<Byte>> generator)
        {
            return new Identity { _data = new List<byte>(generator()) };
        }


        #region Contructors

        public Identity(Identity id) 
        {
            _data = id.ValidatedBytes;
            _hashCode = id._hashCode;
        }

        public Identity(IEnumerable<byte> data)
        {
            _data = new List<byte>(data);
            _hashCode = _data.Count == 0 ? new int?(0) : null;
        }

        public Identity(string data)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(data))
                {
                    _data = new List<byte>();
                    _hashCode = 0;
                }
                else
                {
                    _data = new List<byte>(HexStringToBytes(data));
                    _hashCode = null;
                }
            }
            catch (Exception e)
            {
                throw new InvalidCastException("Invalid ID value}", e);
            }
        }

        public Identity(Guid data)
        {
            _data = new List<byte>(data.ToByteArray());
            _hashCode = null;
        }
        
        #endregion 

        #region Operators

        public static bool operator ==(Identity idOne, Identity idTwo)
        {
            return Compare(idOne.ValidatedBytes, idTwo.ValidatedBytes) == 0;
        }

        public static bool operator !=(Identity idOne, Identity idTwo)
        {
            return Compare(idOne.ValidatedBytes, idTwo.ValidatedBytes) != 0;
        }

        public static bool operator <(Identity idOne, Identity idTwo)
        {
            return Compare(idOne.ValidatedBytes, idTwo.ValidatedBytes) < 0;
        }

        public static bool operator <=(Identity idOne, Identity idTwo)
        {
            return Compare(idOne.ValidatedBytes, idTwo.ValidatedBytes) <= 0;
        }

        public static bool operator >(Identity idOne, Identity idTwo)
        {
            return Compare(idOne.ValidatedBytes, idTwo.ValidatedBytes) > 0;
        }

        public static bool operator >=(Identity idOne, Identity idTwo)
        {
            return Compare(idOne.ValidatedBytes, idTwo.ValidatedBytes) >= 0;
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
            return BytesToHexString(ValidatedBytes.ToArray());
        }

        public override int GetHashCode()
        {
            // TODO: review
            if (!_hashCode.HasValue)
            {
                unchecked
                {
                    const int p = 16777619;
                    var hash = _data.Aggregate((int)2166136261, (current, b) => (current ^ b) * p);

                    hash += hash << 13;
                    hash ^= hash >> 7;
                    hash += hash << 3;
                    hash ^= hash >> 17;
                    hash += hash << 5;
                    _hashCode = hash;
                }
            }
            return _hashCode.Value;
        }
        
        #endregion

        #region ICompare

        public int CompareTo(Identity other)
        {
            return Compare(ValidatedBytes, other.ValidatedBytes);
        }

        #endregion

        #region Casts

        static public explicit operator Identity(string value)
        {
            return new Identity(value);
        }

        static public explicit operator string(Identity value)
        {
            return value.ToString();
        }

        static public explicit operator Identity(byte[] value)
        {
            return new Identity(value);
        }

        static public explicit operator byte[](Identity value)
        {
            var current = value.ValidatedBytes;
            return current.ToArray();
        }

        static public explicit operator Identity(Guid value)
        {
            return new Identity(value);
        }

        static public explicit operator Guid(Identity value)
        {
            return new Guid(value.ValidatedBytes.ToArray());
        }

        #endregion

        public void Add(object data)
        {
            if (data == null || !(data is byte))
            {
                throw new InvalidCastException();
            }

            ValidatedBytes.Add((byte)data);
        }

        private List<byte> ValidatedBytes
        {
            get
            {
                if (_data == null)
                {
                    _data = new List<byte>();
                    _hashCode = 0;
                }
                return _data;
            }
        }

        private static int Compare(List<byte> left, List<byte> right)
        {
            // TODO: review
            var leftCount = left.Count;
            var rightCount = right.Count;

            var count = rightCount > leftCount ? leftCount : rightCount;
            for (int i = 0; i < count; i++)
            {
                var single = left[i] - right[i];
                if (single != 0) return single;
            }
            return leftCount - rightCount;
        }

        public static string BytesToHexString(byte[] p, bool lowerCase = true)
        {
            var len = p.Length;

            var c = new char[len * 2];
            var a = lowerCase ? 0x57 : 0x37;
            for (int y = 0, x = 0; y < len; ++y, ++x)
            {
                var b = ((byte)(p[y] >> 4));
                c[x] = (char)(b > 9 ? b + a : b + 0x30);
                b = ((byte)(p[y] & 0xF));
                c[++x] = (char)(b > 9 ? b + a : b + 0x30);
            }

            return new string(c);
        }

        public static byte[] HexStringToBytes(string hex)
        {
            var len = hex.Length;
            var bytes = new byte[len / 2];
            unchecked
            {
                var i = 0;
                var j = 0;
                while (i < len)
                {
                    var c = hex[i++];
                    var x = c & 0x000F;
                    switch (c & 0xFFF0)
                    {
                        case 48: // 0 - 9
                            if (x > 9)
                                throw new FormatException(string.Format(InvalidCharacter, c, i - 1));
                            break;

                        case 64: // A - F
                        case 96: // a - f
                            x += 9;
                            if (x > 15)
                                throw new FormatException(string.Format(InvalidCharacter, c, i - 1));
                            break;
                        default:
                            throw new FormatException(string.Format(InvalidCharacter, c, i - 1));
                    }
                    x <<= 4;

                    c = hex[i++];
                    var y = c & 0x000F;
                    switch (c & 0xFFF0)
                    {
                        case 48: // 0 - 9
                            if (y > 9)
                                throw new FormatException(string.Format(InvalidCharacter, c, i - 1));
                            break;

                        case 64: // A - F
                        case 96: // a - f
                            y += 9;
                            if (y > 15)
                                throw new FormatException(string.Format(InvalidCharacter, c, i - 1));
                            break;
                        default:
                            throw new FormatException(string.Format(InvalidCharacter, c, i - 1));
                    }
                    bytes[j++] = (byte)(x + y);
                }
            }
            return bytes;
        }

        public IEnumerator<byte> GetEnumerator()
        {
            var len = ValidatedBytes.Count;
            for (int i = 0; i < len; i++)
                yield return _data[i];
        }        

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ValidatedBytes.GetEnumerator();
        }
    }
}
