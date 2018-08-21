using System;
using System.IO;
using Kaitai;

using RCP.Protocol;
using System.Drawing;

namespace RCP.Parameter
{                           
    public class RGBADefinition : DefaultDefinition<Color>, IRGBADefinition
    {
        public RGBADefinition()
        : base(RcpTypes.Datatype.Rgba)
        {
            FDefault = Color.Black;
        }

        public override void ResetForInitialize()
        {
            base.ResetForInitialize();

            DefaultChanged = Default != Color.Black;
        }

        public override Color ReadValue(KaitaiStream input)
        {
            var a = input.ReadU1();
            var b = input.ReadU1();
            var g = input.ReadU1();
            var r = input.ReadU1();
            return Color.FromArgb(a, r, g, b);
        }

        public override void WriteValue(BinaryWriter writer, Color value)
        {
            writer.Write((byte)value.A);
            writer.Write((byte)value.B);
            writer.Write((byte)value.G);
            writer.Write((byte)value.R);
        }
    }
}
