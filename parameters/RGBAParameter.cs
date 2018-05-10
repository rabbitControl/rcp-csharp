using System;
using System.IO;
using System.Drawing;
using Kaitai;

using RCP.Protocol;

namespace RCP.Parameter
{
    public class RGBAParameter : ValueParameter<Color>
    {
		public RGBAParameter(Int16 id, IParameterManager manager) : 
            base (id, RcpTypes.Datatype.Rgba, manager)
        { }

        public override void ResetForInitialize()
        {
            base.ResetForInitialize();

            FValueChanged = Value != Color.Black;
            FDefaultChanged = Default != Color.Black;
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
