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
            Multiselect = 50,
        }

        public enum NumberboxOptions
        {
            Precision = 86,
            Format = 87,
            Stepsize = 88,
            Cyclic = 89,
        }

        public enum CustomtypeOptions
        {
            Default = 48,
            Uuid = 49,
            Config = 50,
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
            Userid = 40,
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
            Customwidget = 1,
            Info = 16,
            Textbox = 17,
            Bang = 18,
            Press = 19,
            Toggle = 20,
            Numberbox = 21,
            Dial = 22,
            Slider = 23,
            Slider2d = 24,
            Range = 25,
            Dropdown = 26,
            Radiobutton = 27,
            Colorbox = 28,
            Table = 29,
            Filechooser = 30,
            Directorychooser = 31,
            Ip = 32,
            List = 32768,
            Listpage = 32769,
            Tabs = 32770,
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

        public enum DialOptions
        {
            Cyclic = 86,
        }

        public enum RangeOptions
        {
            Default = 48,
        }

        public enum LabelPosition
        {
            Left = 1,
            Right = 2,
            Top = 3,
            Bottom = 4,
            Center = 5,
        }

        public enum UriOptions
        {
            Default = 48,
            Filter = 49,
            Schema = 50,
        }

        public enum SliderOptions
        {
            Horizontal = 86,
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
            RegularExpression = 49,
        }

        public enum ArrayOptions
        {
            Default = 48,
            Structure = 49,
        }

        public enum NumberboxFormat
        {
            Dec = 1,
            Hex = 2,
            Bin = 3,
        }

        public enum Datatype
        {
            Customtype = 1,
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
            Array = 37,
            List = 38,
            Bang = 39,
            Group = 40,
            Uri = 42,
            Ipv4 = 43,
            Ipv6 = 44,
            Range = 45,
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

        public enum TextboxOptions
        {
            Multiline = 86,
            Wordwrap = 87,
            Password = 88,
        }

        public enum ListOptions
        {
            Default = 48,
            Minimum = 49,
            Maximum = 50,
        }

        public enum CustomwidgetOptions
        {
            Uuid = 86,
            Config = 87,
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
        private RcpTypes m_root;
        private KaitaiStruct m_parent;
        public RcpTypes M_Root { get { return m_root; } }
        public KaitaiStruct M_Parent { get { return m_parent; } }
    }
}
