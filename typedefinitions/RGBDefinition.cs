using System;
using System.IO;
using Kaitai;

using RCP.Protocol;
using System.Drawing;

namespace RCP.Parameter
{                           
    public class RGBDefinition : DefaultDefinition<Color>
    {
        public RGBDefinition()
            : base(RcpTypes.Datatype.Rgb)
        {
            FDefault = Color.Black;
        }

        public override Parameter CreateParameter(short id, IParameterManager manager) => new ValueParameter<Color>(id, manager, this);

        public override void ResetForInitialize()
        {
            base.ResetForInitialize();

            DefaultChanged = Default != Color.Black;
        }

        public override Color ReadValue(KaitaiStream input)
        {
            var b = input.ReadU1();
            var g = input.ReadU1();
            var r = input.ReadU1();
            var a = input.ReadU1();
            return Color.FromArgb(255, r, g, b);
        }

        public override void WriteValue(BinaryWriter writer, Color value)
        {
            writer.Write((byte)value.B);
            writer.Write((byte)value.G);
            writer.Write((byte)value.R);
            writer.Write((byte)255);
        }
    }
}
