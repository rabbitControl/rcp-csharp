using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RCP.Model
{
    public partial class RcpTypes
    {
        public partial class TinyString
        {
            public static void Write(string text, BinaryWriter writer)
            {
                var bytes = Encoding.UTF8.GetBytes(text);
                var length = (byte)Math.Min(byte.MaxValue, bytes.Length);
                writer.Write(length);
                writer.Write(bytes, 0, length);
            }
        }
    }

    public partial class RcpTypes
    {
        public partial class ShortString
        {
            public static void Write(string text, BinaryWriter writer)
            {
                var bytes = Encoding.UTF8.GetBytes(text);
                var length = (ushort)Math.Min(ushort.MaxValue, bytes.Length);
                writer.Write(length, ByteOrder.BigEndian);
                writer.Write(bytes, 0, length);
            }
        }
    }
	
	public partial class RcpTypes
    {
        public partial class LongString
        {
            public static void Write(string text, BinaryWriter writer)
            {
                var bytes = Encoding.UTF8.GetBytes(text);
                var length = (int)Math.Min(int.MaxValue, bytes.Length);
                writer.Write(length, ByteOrder.BigEndian);
                writer.Write(bytes, 0, length);
            }
        }
    }
}
