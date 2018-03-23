// This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild

using System;
using System.Collections.Generic;
using Kaitai;

namespace RCP.Protocol
{
    public partial class RcpTypes : KaitaiStruct
    {
        public static RcpTypes FromFile(string fileName)
        {
            return new RcpTypes(new KaitaiStream(fileName));
        }

        public enum EnumOptions
        {
            Default = 48,
            Entries = 49,
        }

        public enum WidgetOptions
        {
            Type = 80,
            Enabled = 81,
            Visible = 82,
            LabelVisible = 83,
            ValueVisible = 84,
            LabelPosition = 85,
        }

        public enum ColorOptions
        {
            Default = 48,
        }

        public enum ParameterOptions
        {
            Value = 32,
            Label = 33,
            Description = 34,
            Tags = 35,
            Order = 36,
            Parentid = 37,
            Widget = 38,
            Userdata = 39,
        }

        public enum Ipv4Options
        {
            Default = 48,
        }

        public enum VectorOptions
        {
            Default = 48,
            Minimum = 49,
            Maximum = 50,
            Multipleof = 51,
            Scale = 52,
            Unit = 53,
        }

        public enum BooleanOptions
        {
            Default = 48,
        }

        public enum Widgettype
        {
            Textbox = 16,
            Numberbox = 17,
            Button = 18,
            Checkbox = 19,
            Radiobutton = 20,
            Slider = 21,
            Dial = 22,
            Colorbox = 23,
            Table = 24,
            Treeview = 25,
            Dropdown = 26,
            Xyfield = 31,
        }

        public enum Command
        {
            Invalid = 0,
            Version = 1,
            Initialize = 2,
            Discover = 3,
            Update = 4,
            Remove = 5,
            Updatevalue = 6,
        }

        public enum NumberScale
        {
            Linear = 0,
            Logarithmic = 1,
            Exp2 = 2,
        }

        public enum DynamicArrayOptions
        {
            Default = 48,
        }

        public enum LabelPosition
        {
            Left = 0,
            Right = 1,
            Top = 2,
            Bottom = 3,
            Center = 4,
        }

        public enum UriOptions
        {
            Default = 48,
            Filter = 49,
            Schema = 50,
        }

        public enum ClientStatus
        {
            Disconnected = 0,
            Connected = 1,
            VersionMissmatch = 2,
            Ok = 3,
        }

        public enum StringOptions
        {
            Default = 48,
        }

        public enum Datatype
        {
            Boolean = 16,
            Int8 = 17,
            Uint8 = 18,
            Int16 = 19,
            Uint16 = 20,
            Int32 = 21,
            Uint32 = 22,
            Int64 = 23,
            Uint64 = 24,
            Float32 = 25,
            Float64 = 26,
            Vector2i32 = 27,
            Vector2f32 = 28,
            Vector3i32 = 29,
            Vector3f32 = 30,
            Vector4i32 = 31,
            Vector4f32 = 32,
            String = 33,
            Rgb = 34,
            Rgba = 35,
            Enum = 36,
            FixedArray = 37,
            DynamicArray = 38,
            Bang = 39,
            Group = 40,
            Uri = 42,
            Ipv4 = 43,
            Ipv6 = 44,
        }

        public enum NumberOptions
        {
            Default = 48,
            Minimum = 49,
            Maximum = 50,
            Multipleof = 51,
            Scale = 52,
            Unit = 53,
        }

        public enum Ipv6Options
        {
            Default = 48,
        }

        public enum MetadataOptions
        {
            Version = 26,
            Capabilities = 27,
            Commands = 28,
        }

        public enum PacketOptions
        {
            Timestamp = 17,
            Data = 18,
        }

        public enum FixedArrayOptions
        {
            Default = 48,
        }

        public RcpTypes(KaitaiStream io, KaitaiStruct parent = null, RcpTypes root = null) : base(io)
        {
            m_parent = parent;
            m_root = root ?? this;
            _parse();
        }

        private void _parse()
        {
        }
        public partial class ShortString : KaitaiStruct
        {
            public static ShortString FromFile(string fileName)
            {
                return new ShortString(new KaitaiStream(fileName));
            }

            public ShortString(KaitaiStream io, KaitaiStruct parent = null, RcpTypes root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _myLen = m_io.ReadU2be();
                _data = System.Text.Encoding.GetEncoding("UTF-8").GetString(m_io.ReadBytes(MyLen));
            }
            private ushort _myLen;
            private string _data;
            private RcpTypes m_root;
            private KaitaiStruct m_parent;
            public ushort MyLen { get { return _myLen; } }
            public string Data { get { return _data; } }
            public RcpTypes M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }
        }
        public partial class Userdata : KaitaiStruct
        {
            public static Userdata FromFile(string fileName)
            {
                return new Userdata(new KaitaiStream(fileName));
            }

            public Userdata(KaitaiStream io, KaitaiStruct parent = null, RcpTypes root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _myLen = m_io.ReadU4be();
                _data = m_io.ReadBytes(MyLen);
            }
            private uint _myLen;
            private byte[] _data;
            private RcpTypes m_root;
            private KaitaiStruct m_parent;
            public uint MyLen { get { return _myLen; } }
            public byte[] Data { get { return _data; } }
            public RcpTypes M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }
        }
        public partial class LongString : KaitaiStruct
        {
            public static LongString FromFile(string fileName)
            {
                return new LongString(new KaitaiStream(fileName));
            }

            public LongString(KaitaiStream io, KaitaiStruct parent = null, RcpTypes root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _myLen = m_io.ReadU4be();
                _data = System.Text.Encoding.GetEncoding("UTF-8").GetString(m_io.ReadBytes(MyLen));
            }
            private uint _myLen;
            private string _data;
            private RcpTypes m_root;
            private KaitaiStruct m_parent;
            public uint MyLen { get { return _myLen; } }
            public string Data { get { return _data; } }
            public RcpTypes M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }
        }
        public partial class Id : KaitaiStruct
        {
            public static Id FromFile(string fileName)
            {
                return new Id(new KaitaiStream(fileName));
            }

            public Id(KaitaiStream io, KaitaiStruct parent = null, RcpTypes root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _myLen = m_io.ReadU1();
                _data = m_io.ReadBytes(MyLen);
            }
            private byte _myLen;
            private byte[] _data;
            private RcpTypes m_root;
            private KaitaiStruct m_parent;
            public byte MyLen { get { return _myLen; } }
            public byte[] Data { get { return _data; } }
            public RcpTypes M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }
        }
        public partial class TinyString : KaitaiStruct
        {
            public static TinyString FromFile(string fileName)
            {
                return new TinyString(new KaitaiStream(fileName));
            }

            public TinyString(KaitaiStream io, KaitaiStruct parent = null, RcpTypes root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _myLen = m_io.ReadU1();
                _data = System.Text.Encoding.GetEncoding("UTF-8").GetString(m_io.ReadBytes(MyLen));
            }
            private byte _myLen;
            private string _data;
            private RcpTypes m_root;
            private KaitaiStruct m_parent;
            public byte MyLen { get { return _myLen; } }
            public string Data { get { return _data; } }
            public RcpTypes M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }
        }
        private RcpTypes m_root;
        private KaitaiStruct m_parent;
        public RcpTypes M_Root { get { return m_root; } }
        public KaitaiStruct M_Parent { get { return m_parent; } }
    }
}
