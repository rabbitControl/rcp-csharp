using System;
using System.IO;
using Kaitai;
using System.Drawing;

namespace RCP.Model
{
    public class RGBADefinition : DefaultDefinition<Color>, IRGBADefinition
    {
        public RGBADefinition()
        : base(RcpTypes.Datatype.Rgba)
        { }

        public override Color ReadValue(KaitaiStream input)
        {
            var r = input.ReadU1();
            var g = input.ReadU1();
            var b = input.ReadU1();
            var a = input.ReadU1();
            return Color.FromArgb(a, r, g, b);
        }

        public override void WriteValue(BinaryWriter writer, Color value)
        {
            writer.Write((byte)value.R);
            writer.Write((byte)value.G);
            writer.Write((byte)value.B);
            writer.Write((byte)value.A);
        }
    	
        protected override void WriteOptions(BinaryWriter writer)
        {
        	if (Default != null)
        	{
        		writer.Write((byte)RcpTypes.ColorOptions.Default);
                writer.Write((byte)Default.R);
                writer.Write((byte)Default.G);
                writer.Write((byte)Default.B);
                writer.Write((byte)Default.A);
            }
        }

        protected override bool HandleOption(KaitaiStream input, byte code)
        {
            var option = (RcpTypes.ColorOptions)code;
            if (!Enum.IsDefined(typeof(RcpTypes.ColorOptions), option))
                throw new RCPDataErrorException();

            switch (option)
            {
                case RcpTypes.ColorOptions.Default:
                    Default = ReadValue(input);
                    return true;
            }

            return false;
        }
    }
}
